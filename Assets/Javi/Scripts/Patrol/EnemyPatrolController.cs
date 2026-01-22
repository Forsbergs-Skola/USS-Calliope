using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatrolController : MonoBehaviour
{
    [Header("Main Patrol Zones")]
    [SerializeField] private List<PatrolZone> mainZones = new();
    
    [Header("Allowed Patrol Zones")]
    [SerializeField] private List<PatrolZone> allowedZones = new List<PatrolZone>();

    [Header("Patrol Settings")]
    [SerializeField] private float waitTimeAtPoint = 10f;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float reachTolerance = 0.6f;
    [SerializeField] private float retryDelay = 2f; //Time if there aren't points
    
    [Header("Look Around Settings")]
    [SerializeField] private float lookAngle = 60f; // Rotation Angle
    [SerializeField] private float lookPauseTime = 1.5f;

    private SimpleMovementAgent movement;
    private Coroutine patrolRoutine;
    private Coroutine lookAroundRoutine;
    private Coroutine watchInPlaceRoutine;
    private bool isWatchingInPlace = false;

    private PatrolZone currentZone;
    private Transform currentPoint;
    private bool isPatrolling = false;
    
    private PatrolMode currentMode = PatrolMode.Main;
    private PatrolZone forcedZone; // Zone where the player was lost
    
    public PatrolZone GetCurrentZone() => currentZone;

    private void Awake()
    {
        movement = GetComponent<SimpleMovementAgent>();
    }
    
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }
    
    public enum PatrolMode
    {
        Main,
        AllowedSingleZone
    }
    
    private IEnumerator DelayedStart()
    {
        yield return null; // We wait 1 frame
        
        if (allowedZones.Count > 0)
        {
            StartPatrol();
        }
        else
        {
            Debug.LogWarning($"{name} has no patrol zones assigned. Patrolling disabled.");
        }
    }
    public void StartPatrol()
    {
        StopPatrol();

        if (mainZones.Count == 0)
        {
            Debug.LogWarning($"{name} has no patrol zones assigned");
            return;
        }

        isPatrolling = true;
        patrolRoutine = StartCoroutine(PatrolRoutine());
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        
        if (lookAroundRoutine != null)
        {
            StopCoroutine(lookAroundRoutine);
            lookAroundRoutine = null;
        }
        
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        
        if (watchInPlaceRoutine != null)
        {
            StopCoroutine(watchInPlaceRoutine);
            watchInPlaceRoutine = null;
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (isPatrolling)
        {
            //currentZone = allowedZones[Random.Range(0, allowedZones.Count)];
            switch (currentMode)
            {
                case PatrolMode.Main:
                    if (mainZones.Count == 0) yield break;
                    currentZone = mainZones[Random.Range(0, mainZones.Count)];
                    break;

                case PatrolMode.AllowedSingleZone:
                    if (forcedZone == null)
                    {
                        Debug.LogWarning($"{name}: Forced zone is null, falling back to main patrol");
                        currentMode = PatrolMode.Main;
                        continue;
                    }
                    currentZone = forcedZone;
                    break;
            }
            
            List<Transform> points = PatrolPointRegistry.GetPointsForZone(currentZone);

            if (points == null || points.Count == 0)
            {
                Debug.LogWarning($"{name}: No points found for zone {currentZone.name}");
                yield return new WaitForSeconds(retryDelay);
                continue;
            }

            Transform nextPoint = GetRandomPoint(points);

            // Moving to the point
            while (Vector3.Distance(transform.position, nextPoint.position) > reachTolerance)
            {
                if (movement == null) yield break;
                
                movement.MoveTo(nextPoint.position);
                yield return null;
            }
            
            Debug.Log($"{name}: Reached point {nextPoint.name}. Starting look around...");
            
            // Save rotation
            Vector3 lookPosition = transform.position;
            Quaternion initialRotation = transform.rotation;
            
            // Idle Mode
            movement.SetMovementState(SimpleMovementAgent.MovementState.Idle);
            
            // Start coroutine
            lookAroundRoutine = StartCoroutine(LookAroundRoutine(initialRotation));
            
            // Waiting for the time
            yield return new WaitForSeconds(waitTimeAtPoint);
            
            // stop rotation coroutine 
            if (lookAroundRoutine != null)
            {
                StopCoroutine(lookAroundRoutine);
                lookAroundRoutine = null;
            }
            
            transform.rotation = initialRotation;
            movement.SetMovementState(SimpleMovementAgent.MovementState.Patrolling);
            
            Debug.Log($"{name}: Finished waiting at point {nextPoint.name}");

            /*float timer = 0f;
            while (timer < waitTimeAtPoint)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }*/
        }
    }
    
    private IEnumerator LookAroundRoutine(Quaternion initialRotation)
    {
        bool LookLeft = true; // Starting 
        
        while (true)
        {
            // Target angle
            float targetAngle = LookLeft ? lookAngle : -lookAngle;
            
            // Rotation
            Quaternion targetRotation = initialRotation * Quaternion.Euler(0, targetAngle, 0);
            
            float rotationTime = Mathf.Abs(targetAngle) / rotationSpeed;
            float elapsedTime = 0f;
            
            while (elapsedTime < rotationTime)
            {
                if (!isPatrolling && !isWatchingInPlace) yield break;
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    elapsedTime / rotationTime
                );
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.rotation = targetRotation;
            
            // waiting
            yield return new WaitForSeconds(lookPauseTime);
            
            initialRotation = transform.rotation;
        }
    }

    private Transform GetRandomPoint(List<Transform> points)
    {
        Transform selected;

        do
        {
            selected = points[Random.Range(0, points.Count)];
        }
        while (selected == currentPoint && points.Count > 1);

        currentPoint = selected;
        return selected;
    }
    
    public void PatrolMainZones()
    {
        currentMode = PatrolMode.Main;
        forcedZone = null;
        Debug.Log($"{name} returning to MAIN patrol zones");
        RestartPatrol();
    }

    public void PatrolSingleZone(PatrolZone zone)
    {
        if (zone == null)
        {
            Debug.LogWarning($"{name}: PatrolSingleZone called with null zone");
            return;
        }

        if (!allowedZones.Contains(zone))
        {
            Debug.LogWarning($"{name}: Zone {zone.name} is not in allowed zones");
            return;
        }

        currentMode = PatrolMode.AllowedSingleZone;
        forcedZone = zone;
        RestartPatrol();
    }
    
    private void RestartPatrol()
    {
        StopPatrol();
        
        currentPoint = null;
        movement.SetMovementState(SimpleMovementAgent.MovementState.Patrolling);
        
        StartPatrol();
    }
    
    public bool CanPatrolZone(PatrolZone zone)
    {
        if (zone == null) return false;
        return allowedZones.Contains(zone);
    }
    
    public void WatchInPlace(float duration)
    {
        StopPatrol();

        if (watchInPlaceRoutine != null)
            StopCoroutine(watchInPlaceRoutine);
        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);
        if (lookAroundRoutine != null)
            StopCoroutine(lookAroundRoutine);
        
        isWatchingInPlace = true;
        watchInPlaceRoutine = StartCoroutine(WatchInPlaceRoutine(duration));
    }

    private IEnumerator WatchInPlaceRoutine(float duration)
    {
        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Idle);

        Quaternion initialRotation = transform.rotation;

        lookAroundRoutine = StartCoroutine(LookAroundRoutine(initialRotation));

        yield return new WaitForSeconds(duration);

        if (lookAroundRoutine != null)
        {
            StopCoroutine(lookAroundRoutine);
            lookAroundRoutine = null;
        }
        
        isWatchingInPlace = false;
        //transform.rotation = initialRotation;
        
        PatrolMainZones();
    }
    
    public void GoToPoint(Transform point)
    {
        if (movement == null || point == null) return;

        movement.MoveTo(point.position);
    }
}
