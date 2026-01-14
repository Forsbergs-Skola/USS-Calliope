using System;
using UnityEngine;
using Unity.Cinemachine;
public class BobDialogueCollider : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        bool investigateMedbayStarted = progressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_04_ID] == EnumObjectiveStatus.STARTED;
        
        
        
        
        if (investigateMedbayStarted)
        {
            DialogueController.Instance.StartConvoWithID(IDConstants.CONVERSATION_BOB_00);
            return;
        }

        if (inventoryRuntimeData.Value.GetQuestItemIDs().Contains(IDConstants.INFECTED_SAMPLE))
        {
            DialogueController.Instance.StartConvoWithID(IDConstants.CONVERSATION_BOB_01);
            return;
        }
        
    }
    
}
