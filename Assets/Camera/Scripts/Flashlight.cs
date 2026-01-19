using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;

    [SerializeField] Transform player;
    [SerializeField] private Light lightSource;

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

          
            Transform targetParent = other.transform;

            Light lightInstance = Instantiate(lightSource);
            lightInstance.transform.SetParent(targetParent, false); 
            lightInstance.transform.localPosition = new Vector3(0f, 1.5f,0f);
            lightInstance.transform.localRotation = Quaternion.identity;

            gameObject.SetActive(false);
        }
    }

}
