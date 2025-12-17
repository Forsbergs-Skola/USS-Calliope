using UnityEngine;

public class AmmoPickUpView : MonoBehaviour
{
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField] private int ammoAmount;

    public SO_AmmoType AmmoType => ammoType;
    public int AmmoAmount => ammoAmount;
}
