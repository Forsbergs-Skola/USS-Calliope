using UnityEngine;

public class DataUpdateHandler : MonoBehaviour
{
   [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
   [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
   
   private EventRelay relay = EventRelay.Instance;

   void OnEnable()
   {
      if (relay == null) return;
      relay.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleDataUpdate;
   }

   void OnDisable()
   {
      if (relay == null) return;
      relay.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleDataUpdate;
   }

   void HandleDataUpdate()
   {
      bool investigateMedbayFinished = progressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_04_ID] == EnumObjectiveStatus.FINISHED;
      if (investigateMedbayFinished)
      {
         bool hasPistol = inventoryRuntimeData.Value.GetWeaponItemIDs().Contains(IDConstants.PISTOL);
         if (!hasPistol)
         {
            inventoryRuntimeData.Value.AddWeaponItem(IDConstants.PISTOL);
            inventoryRuntimeData.Value.AddNewConsumable(IDConstants.PISTOL_AMMO, 10);
         }
      }
   }
   
}
