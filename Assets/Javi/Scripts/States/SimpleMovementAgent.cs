using System.Collections.Generic;
using UnityEngine;


public class SimpleMovementAgent : MonoBehaviour, IMovementAgent
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float stoppingDistance = 0.1f;
    
    [Header("Speed Multipliers")]
    [SerializeField] private float patrolMultiplier = 0.8f;
    [SerializeField] private float chaseMultiplier = 1.5f;
    [SerializeField] private float investigateMultiplier = 1.0f;

    private readonly Dictionary<object, float> speedModifiers = new();
    private float currentSpeedMultiplier = 1.0f;
    private MovementState currentState = MovementState.Idle;
    private Vector3 currentTarget;//

    public float BaseSpeed => baseSpeed;
    public float CurrentSpeed => baseSpeed * currentSpeedMultiplier * GetTotalModifiers();
    public MovementState CurrentState => currentState;
    public Vector3 CurrentTarget => currentTarget;//
    
    public void SetBaseSpeed(float newBaseSpeed) => baseSpeed = Mathf.Max(0, newBaseSpeed);
    public void SetPatrolMultiplier(float multiplier) => patrolMultiplier = multiplier;
    public void SetChaseMultiplier(float multiplier) => chaseMultiplier = multiplier;

    public enum MovementState
    {
        Idle,
        Patrolling,
        Chasing,
        Investigating,
        Custom
    }
    
    private void Awake()
    {
        SetMovementState(MovementState.Patrolling); // Default state
    }

    public void SetMovementState(MovementState state)
    {
        currentState = state;
        
        switch (state)
        {
            case MovementState.Patrolling:
                currentSpeedMultiplier = patrolMultiplier;
                break;
            case MovementState.Chasing:
                currentSpeedMultiplier = chaseMultiplier;
                break;
            case MovementState.Investigating:
                currentSpeedMultiplier = investigateMultiplier;
                break;
            case MovementState.Idle:
                currentSpeedMultiplier = 0f;
                break;
            case MovementState.Custom:
                // This doesnt change the modifier
                break;
        }
    }
    
    public void SetCustomSpeedMultiplier(float multiplier)
    {
        currentState = MovementState.Custom;
        currentSpeedMultiplier = Mathf.Max(0, multiplier);
    }

    public void AddSpeedModifier(object source, float multiplier)
    {
        if (speedModifiers.ContainsKey(source))
            speedModifiers[source] = multiplier;
        else
            speedModifiers.Add(source, multiplier);
    }

    public void RemoveSpeedModifier(object source)
    {
        speedModifiers.Remove(source);
    }

    private float GetTotalModifiers()
    {
        float total = 1.0f;
        foreach (var modifier in speedModifiers.Values)
        {
            total *= modifier;
        }
        return total;
    }

    public void MoveTo(Vector3 target)
    {
        //Debug.Log($"[MovementAgent] {name}: Moving to {target}");
        if (currentState == MovementState.Idle)
            return;
        //Debug.Log("[MovementAgent]: after if currentState");
        currentTarget = target;
        
        /*if (Vector3.Distance(transform.position, target) <= stoppingDistance)
            return;*/
        Vector3 flatPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(target.x, 0f, target.z);

        if (Vector3.Distance(flatPos, flatTarget) <= stoppingDistance)
            return;
        //Debug.Log($"[MovementAgent] {name}: Moving to {target}, dist:{Vector3.Distance(flatPos, flatTarget)}");
        
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        
        transform.position += direction * CurrentSpeed * Time.deltaTime;
        //Debug.Log("[MovementAgent]: after transform");
        if (direction != Vector3.zero)
        {
            //Debug.Log("[MovementAgent]: inside if direction");
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
        //Debug.Log("[MovementAgent]: outside if direction");
    }
    
    // Debugging
    public string GetSpeedInfo()
    {
        return $"State: {currentState}, Base: {baseSpeed}, Multiplier: {currentSpeedMultiplier}, Current: {CurrentSpeed}";
    }
}

