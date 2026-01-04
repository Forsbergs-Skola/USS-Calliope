using UnityEngine;

[CreateAssetMenu(fileName = "SO_AmmoType", menuName = "Player/Player Combat/SO_AmmoType")]
public class SO_AmmoType : ScriptableObject
{
    [SerializeField] private string ammoID = string.Empty;    
    [SerializeField] private Sprite ammoIcon;     
    [SerializeField] private int maxStack = 64;  

    public string AmmoID => ammoID;
    public Sprite AmmoIcon => ammoIcon;
    public int MaxStack => maxStack;
    
    public string GetWeaponID()
    {
        return ammoID;
    }
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(ammoID)) return;
        ammoID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}