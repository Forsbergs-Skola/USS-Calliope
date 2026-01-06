using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    
    private EventRelay eventRelay = EventRelay.Instance;


    void Start()
    {
        gameObject.SetActive(!inventoryRuntimeData.Value.GetQuestItemIDs().Contains(IDConstants.FLASHLIGHT_INV_ID));
    }

    void OnEnable()
    {
        if (eventRelay != null) return;
        eventRelay.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleDataUpdate;
    }

    void OnDisable()
    {
        if (eventRelay != null) return;
        eventRelay.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleDataUpdate;
    }

    private void HandleDataUpdate()
    {
        gameObject.SetActive(!inventoryRuntimeData.Value.GetQuestItemIDs().Contains(IDConstants.FLASHLIGHT_INV_ID));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           inventoryRuntimeData.Value.AddQuestItem(IDConstants.FLASHLIGHT_INV_ID);
           gameObject.SetActive(false);
        }
    }
    
}
