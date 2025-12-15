using UnityEngine;

[CreateAssetMenu(fileName = "SO_AmmoType", menuName = "Player Combat/SO_AmmoType")]
public class SO_AmmoType : ScriptableObject
{
    [SerializeField] private string ammoId;       
    [SerializeField] private Sprite ammoIcon;     
    [SerializeField] private int maxStack = 64;  

    public string AmmoId => ammoId;
    public Sprite AmmoIcon => ammoIcon;
    public int MaxStack => maxStack;
}