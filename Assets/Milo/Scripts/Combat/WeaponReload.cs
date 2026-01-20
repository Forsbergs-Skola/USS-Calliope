using System.Collections;
using UnityEngine;

public class WeaponReload : MonoBehaviour
{
    public Coroutine reloadCoroutine { get; set; }


    public void TryReload
        (SO_WeaponType data, AmmoModel ammo, InventoryData invData)
    {
        if (!data || !data.HasAmmo) return;
        if (reloadCoroutine != null) return;
        reloadCoroutine = StartCoroutine(ReloadRoutine(ammo, data, invData));
    }

    private IEnumerator ReloadRoutine(AmmoModel ammo, SO_WeaponType data, InventoryData invData)
    {
        if (data == null) yield break;

        if (ammo.CurrentAmmo >= ammo.MaxAmmo)
        {
            reloadCoroutine = null;
            yield break;
        }
      
        string ammoID = data.AmmoType.AmmoID;
        yield return new WaitForSeconds(data.ReloadTime);

        if (data == null) yield break;

        if (invData.GetConsumableIDsAndQuantities().TryGetValue(ammoID, out int ammoAvailable) && ammoAvailable > 0)
        {
            var ammoNeeded = ammo.MaxAmmo - ammo.CurrentAmmo;
            var ammoToLoad = Mathf.Min(ammoAvailable, ammoNeeded);

            invData.DepleteConsumable(ammoID, ammoToLoad);
            ammo.AddAmmo(ammo.CurrentAmmoType, ammoToLoad);
        }

        reloadCoroutine = null;
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }
}
