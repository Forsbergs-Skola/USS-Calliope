using UnityEngine;

public class CargoEntranceTrigger : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionData;
    private DialogueController dialogueController
    {
        get => DialogueController.Instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        progressionData.Value.CargoBayEntered = true;
        if (dialogueController != null)
        {
            dialogueController.StartConvoWithID(IDConstants.CONVERSATION_WHAT_THAT_NOISE);
        }

    }



}
