using UnityEngine;

public class AmmoPickUpView : MonoBehaviour
{
    [SerializeField] private SO_AmmoType ammoType;

    public SO_AmmoType AmmoType => ammoType;
}
