using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Light lightPrefab; // same Light prefab that the flashlight uses

    private void OnEnable() // when the game is stared or continued
    {
        if (TryGetInventoryData(out InventoryData data))
        {
            bool hasFlashlight = data.GetQuestItemIDs().Contains(IDConstants.FLASHLIGHT_INV_ID);
            if (hasFlashlight)
            {
                // Equip the light (the flashlight prefab is gone from the level)
                Light lightInstance = Instantiate(lightPrefab, transform);
                lightInstance.transform.localPosition = new Vector3(0f, 1.5f, 0f);
                lightInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // do nothing. The flashlight prefab will give me a light
                // when I pick it up.
            }
        }
    }
    private bool TryGetInventoryData(out InventoryData invData)
    {
        invData = null;

        if (TryGetComponent<PlayerDataHandler>(out PlayerDataHandler handler))
        {
            invData = handler.InvRuntimeData.Value;
        }
        return (invData != null);
    }
}
