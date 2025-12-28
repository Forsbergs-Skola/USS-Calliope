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
    private SO_WeaponType currentWeapon;
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
        if (currentWeapon && currentWeapon.ShouldTrackMovement())
            UpdateMovementTracking();    
    }

    public void SetCurrentWeapon(SO_WeaponType equippedWeapon)
    {
        currentWeapon = equippedWeapon;
        Debug.Log($"Current Weapon set to: {currentWeapon?.name}");
        movementTimer = 0f;
        impactProcessor.InitializeProcessor(equippedWeapon);
    }

    public void Execute()
    {
        if (!CanAttack(out Vector3 aimDirection)) return;

        switch (currentWeapon.AttackCategories)
        {
            case SO_WeaponType.AttackCategory.Hitscan:
                GunAttack(aimDirection);
                break;
            case SO_WeaponType.AttackCategory.NonLethal:
                TaserAttack(aimDirection);
                break;
            case SO_WeaponType.AttackCategory.Melee:
                MeleeAttack(aimDirection);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //// ATTACK TYPES
    private void GunAttack(Vector3 aimDirection)
    {
        PlayAttackSound();
        for (int i = 0; i < currentWeapon.PelletCount; i++)
        {
            PerformRaycastShot(aimDirection, impactProcessor.ProcessHit);
        }
    }

    private void TaserAttack(Vector3 aimDirection)
    {
        PlayAttackSound();
        PerformRaycastShot(aimDirection, impactProcessor.ProcessTase);
    }

    private void MeleeAttack(Vector3 aimDirection)
    {
        Debug.Log("MeleeAttack Called");
        PlayAttackSound();
        if (aimDirection == Vector3.zero) aimDirection = transform.forward;

        var radius = currentWeapon.MeleeHitRadius;
        var reach = currentWeapon.MeleeReach; 
    
        Vector3 origin = firePoint.position;
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, aimDirection, reach, impactProcessor.HitMask);
        
        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root) continue;

            impactProcessor.ProcessMeleeHit(hit, aimDirection, currentWeapon.MeleeHitForce);
        }
        if (showDebugTrajectory)
        {
            Debug.DrawRay(origin, aimDirection * reach, Color.red, 0.5f);
        }
    }

    
    private bool CanAttack(out Vector3 aimDirection)
    {
        aimDirection = Vector3.zero;

        if (!currentWeapon || !firePoint) 
        {
            return false;
        }

        var hasAim = aimController.TryGetAimDirection(firePoint.position, out aimDirection);
    
        if (currentWeapon.AttackCategories != SO_WeaponType.AttackCategory.Melee) 
        {
            return hasAim;
        }

        if (!hasAim)
        {
            aimDirection = transform.forward;
        }
        return true; 
    }

    private void PerformRaycastShot(Vector3 aimDirection, Action<RaycastHit> onHit)
    {
        var finalDirection = currentWeapon.HasBallistics
            ? BallisticsUtility.GetGaussianSpread(aimDirection, CalculateSpreadIntensity())
            : aimDirection;

        var origin = firePoint.position;
        var endPoint = origin + finalDirection * currentWeapon.ImpactRange;

        if (Physics.Raycast(origin, finalDirection, out var hitInfo, currentWeapon.ImpactRange, impactProcessor.HitMask))
        {
            onHit?.Invoke(hitInfo);
            endPoint = hitInfo.point;
        }

        if (showDebugTrajectory) Debug.DrawLine(origin, endPoint, debugColor, DebugLifetime);
    }
    
    // Refactor to another script
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

    private float CalculateSpreadIntensity()
    {
        if (!currentWeapon) return 0f;
        var isSprinting = attackInput.SprintAction.action.IsPressed();
        return currentWeapon.GetBaseSpreadIntensity(movementTimer, isSprinting);
    }

    private void PlayAttackSound()
    {
        audioSource.PlayOneShot(currentWeapon.AttackSound);
    }
}
