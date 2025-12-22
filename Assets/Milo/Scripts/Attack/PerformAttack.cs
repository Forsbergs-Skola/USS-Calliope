using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerformAttack : MonoBehaviour
{
    
    [Header("Input Action Reference")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    
    [Header("Dependencies")]
    [SerializeField] private PlayerAimController aimController;
    [SerializeField] private ImpactProcessor impactProcessor;
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugTrajectory = true;
    [SerializeField] private float debugLifetime = 0.05f;
    [SerializeField] private Color debugColor = Color.red;
    
    private float movementTimer;
    private SO_WeaponType weapon;
    private Transform currentMuzzle; 

    private void Update()
    {
        UpdateMovementTracking();
    }

    private void UpdateMovementTracking()
    {
        if (moveAction.action.ReadValue<Vector2>().sqrMagnitude > 0.01f)
        {
            movementTimer += Time.deltaTime;
        }
        else
        {
            movementTimer = 0f;
        }
    }

    public void SetWeapon(SO_WeaponType weapon, Transform muzzle)
    {
        this.weapon = weapon;
        this.currentMuzzle = muzzle; 
    
        movementTimer = 0f; 
        impactProcessor.InitializeProcessor(weapon);
    }
    
    public void Fire()
    {
        if (!CanFire(out Vector3 aimDirection)) return;

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
    
    private bool CanFire(out Vector3 aimDirection)
    {
        aimDirection = Vector3.zero;
        if (!weapon || !firePoint) return false;
        
        return aimController.TryGetAimDirection(currentMuzzle.position, out aimDirection);
    }
    
    private void PerformPelletShot(Vector3 aimDirection)
    {
        float standardDeviation = CalculateSpreadIntensity(); 
        Vector3 finalDirection = BallisticsUtility.GetGaussianSpread(aimDirection, standardDeviation);
    
        Vector3 origin = firePoint.position;
        Vector3 endPoint = origin + finalDirection * weapon.ImpactRange;
    
        if (Physics.Raycast(origin, finalDirection, out RaycastHit hitInfo, weapon.ImpactRange, impactProcessor.HitMask))
        {
            impactProcessor.ProcessHit(hitInfo);
            endPoint = hitInfo.point;
        }

        if (showDebugTrajectory) 
        {
            Debug.DrawLine(origin, endPoint, debugColor, debugLifetime);
        }
    }

    private float CalculateSpreadIntensity()
    {
        if (!weapon) return 0f;
        
        var isSprinting = sprintAction.action.IsPressed();
    
        return weapon.GetBaseSpreadIntensity(movementTimer, isSprinting);
    }
    
    private void FireHitscan(Vector3 aimDirection)
    {
        audioSource.PlayOneShot(weapon.FireSound);

        for (int i = 0; i < weapon.PelletCount; i++)
        {
            PerformPelletShot(aimDirection);
        }
    }

    private void MeleeAttack(Vector3 aimDirection)
    {
        
    }

    private void TaserAttack(Vector3 aimDirection)
    {
        audioSource.PlayOneShot(weapon.FireSound);
        PerformPelletShot(aimDirection);
    }   
}