using UnityEngine;
using System.Collections;

public class WeaponCooldown : MonoBehaviour
{
    private float currentFireRate;
    private bool canShoot = true;

    // Initialization called by PlayerWeaponHandler.EquipWeapon
    public void InitializeCooldown(float fireRate)
    {
        currentFireRate = fireRate;
        canShoot = true; //  resets cooldown when a new weapon is equipped
    }

    public bool CanFire()
    {
        return canShoot;
    }

    public void StartCooldown(float fireRate)
    {
        if (canShoot)
        {
            StartCoroutine(FireRateTimer(fireRate));
        }
    }

    private IEnumerator FireRateTimer(float waitTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(waitTime);
        canShoot = true;
    }
}