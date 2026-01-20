using UnityEngine;
using TMPro;

public class CurrentAmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData playerRuntimeData;
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    [SerializeField] private TMP_Text currentAmmoText;



    private void OnEnable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeDataUpdate;
    }
    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeDataUpdate;
    }

    private void Start()
    {
        UpdateUI(playerRuntimeData.Value.PlayerCurrentAmmo);
    }


    private void HandleRuntimeDataUpdate(IRuntimeData data)
    {
        if (!(data is PlayerData)) return;
        int currentAmmo = playerRuntimeData.Value.PlayerCurrentAmmo;
        UpdateUI(currentAmmo);
    }

    private void UpdateUI(int currentAmmo)
    {

        //Debug.Log(inventoryRuntimeData.Value.GetWeaponItemIDs().Count > 0);

        //string ammoStr = inventoryRuntimeData.Value.GetWeaponItemIDs().Count > 0 ? currentAmmo.ToString() : string.Empty;
        //currentAmmoText.text = ammoStr;
        currentAmmoText.text = currentAmmo.ToString();
    }
}
