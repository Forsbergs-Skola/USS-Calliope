using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/Weapons")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private List<SO_WeaponType> weapons;

    private Dictionary<string, SO_WeaponType> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<string, SO_WeaponType>();

        foreach (SO_WeaponType weapon in weapons)
        {
            if (weapon == null) continue;

            if (lookup.TryAdd(weapon.WeaponID, weapon)) continue;
            Debug.LogError($"Duplicate weapon GUID: {weapon.WeaponID}");
            continue;
        }
    }

    public SO_WeaponType GetWeapon(string guid)
    {
        if (lookup.TryGetValue(guid, out var weapon))
            return weapon;

        Debug.LogError($"Weapon with GUID {guid} not found in WeaponDatabase");
        return null;
    }
}