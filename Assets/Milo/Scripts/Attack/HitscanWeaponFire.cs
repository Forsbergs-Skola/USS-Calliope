using UnityEngine;

public class HitscanWeaponFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAimController aimController;
    [SerializeField] private ImpactProcessor impactProcessor;
    [SerializeField] private Transform firePoint;

    [Header("Debug")]
    [SerializeField] private bool showDebugTrajectory = true; 
    [SerializeField] private float debugLifetime = 0.05f;     
    [SerializeField] private Color debugColor = Color.red;    
    [SerializeField] private float hitMarkerSize = 0.1f;      

    private SO_WeaponType weapon;
    
    public void SetWeapon(SO_WeaponType weapon)
    {
        this.weapon = weapon;
        impactProcessor.InitializeProcessor(weapon);
    }

    public void Fire()
    {
        if (weapon == null || firePoint == null)
            return;

        Vector3 origin = firePoint.position;

        if (!aimController.TryGetAimDirection(origin, out Vector3 aimDirection))
            return;

        for (int i = 0; i < weapon.PelletCount; i++)
        {
            Vector3 finalDirection = BallisticsUtility.GetGaussianSpread(
                aimDirection,
                weapon.SpreadStandardDeviation
            );

            Vector3 endPoint = origin + finalDirection * weapon.ImpactRange;

            if (Physics.Raycast(origin, finalDirection, out RaycastHit hitInfo, 
                    weapon.ImpactRange, impactProcessor.HitMask))
            {
                try
                {
                    impactProcessor.ProcessHit(hitInfo);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error processing hit on {hitInfo.collider.name}: {e}");
                }

                endPoint = hitInfo.point;
            }

            if (!showDebugTrajectory) continue;
            Debug.DrawLine(origin, endPoint, debugColor, debugLifetime);
            Debug.DrawRay(endPoint, Vector3.up * hitMarkerSize, debugColor, debugLifetime);
            Debug.DrawRay(endPoint, Vector3.right * hitMarkerSize, debugColor, debugLifetime);
        }
    }
}
