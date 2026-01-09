using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Combat/SO_FlashLight")]
public class SO_FlashLight : ScriptableObject
{
    [SerializeField] private string iD;
    [SerializeField] private GameObject flashLightPrefab;

    public string ID => iD;
    public GameObject FlashLightPrefab => flashLightPrefab;

}
