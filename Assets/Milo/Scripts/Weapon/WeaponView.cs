    using UnityEngine;

    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private SO_WeaponType weaponType;

        public SO_WeaponType WeaponType => weaponType;
    }