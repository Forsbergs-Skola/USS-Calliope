using UnityEngine;

public class CargoEntranceTrigger : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionData;
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

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
            myCollider.enabled = false;
            dialogueController.StartConvoWithID(IDConstants.CONVERSATION_WHAT_THAT_NOISE);
        }

    }



}
