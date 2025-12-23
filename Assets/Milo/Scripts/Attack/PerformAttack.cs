using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PerformAttack : MonoBehaviour
{
    
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private bool showDebugTrajectory = true;

    private const float DebugLifetime = 0.05f;
    private readonly Color debugColor = Color.green;

    private float movementTimer;
    private SO_WeaponType weapon;
    private AttackInput attackInput;
    private PlayerAimController aimController;
    private ImpactProcessor impactProcessor;

    private void Awake()
    {
        attackInput = GetComponent<AttackInput>();
        aimController = GetComponent<PlayerAimController>();
        impactProcessor = GetComponent<ImpactProcessor>();
    }
    
    private void Update()
    {
        UpdateMovementTracking();
    }

    private void UpdateMovementTracking()
    {
        if (attackInput.MoveAction.action.ReadValue<Vector2>().sqrMagnitude > 0.01f)
        {
            movementTimer += Time.deltaTime;
        }
        else
        {
            movementTimer = 0f;
        }
    }

    public void SetWeapon(SO_WeaponType weapon)
    {
        this.weapon = weapon;
    
        movementTimer = 0f; 
        impactProcessor.InitializeProcessor(weapon);
    }
    
    public void Execute()
    {
        if (!CanAttack(out Vector3 aimDirection)) return;

        switch (weapon.AttackCategories)
        {
            case SO_WeaponType.AttackCategory.Hitscan:
                FireHitscan(aimDirection);
                break;
            case SO_WeaponType.AttackCategory.Taser:
                TaserAttack(aimDirection);
                break;
            case SO_WeaponType.AttackCategory.Melee:
                MeleeAttack(aimDirection);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private bool CanAttack(out Vector3 aimDirection)
    {
        aimDirection = Vector3.zero;
        if (!weapon || !firePoint) return false;
        
        return aimController.TryGetAimDirection(firePoint.position, out aimDirection);
    }

    private void PerformRaycastShot(Vector3 aimDirection, Action<RaycastHit> onHit)
    {
        var standardDeviation = CalculateSpreadIntensity();
        var finalDirection = BallisticsUtility.GetGaussianSpread(aimDirection, standardDeviation);

        var origin = firePoint.position;
        var endPoint = origin + finalDirection * weapon.ImpactRange;

        if (Physics.Raycast(origin, finalDirection, out var hitInfo, weapon.ImpactRange, impactProcessor.HitMask))
        {
            onHit?.Invoke(hitInfo);
            endPoint = hitInfo.point;
        }

        if (showDebugTrajectory) Debug.DrawLine(origin, endPoint, debugColor, DebugLifetime);
    }
    
    private float CalculateSpreadIntensity()
    {
        if (!weapon) return 0f;
        
        var isSprinting = attackInput.SprintAction.action.IsPressed();
    
        return weapon.GetBaseSpreadIntensity(movementTimer, isSprinting);
    }
    
    private void FireHitscan(Vector3 aimDirection)
    {
        audioSource.PlayOneShot(weapon.FireSound);

        for (int i = 0; i < weapon.PelletCount; i++)
        {
            PerformRaycastShot(aimDirection, impactProcessor.ProcessHit);
        }
    }

    private void MeleeAttack(Vector3 aimDirection)
    {
        PerformRaycastShot(aimDirection, impactProcessor.ProcessMeleeHit);
    }

    private void TaserAttack(Vector3 aimDirection)
    {
        audioSource.PlayOneShot(weapon.FireSound);
        PerformRaycastShot(aimDirection, impactProcessor.ProcessTase);
    }
}