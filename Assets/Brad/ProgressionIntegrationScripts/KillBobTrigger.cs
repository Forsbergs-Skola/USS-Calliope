using UnityEngine;

public class KillBobTrigger : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionData;
    private Collider myCollider;
    private DialogueController dialogueController
    {
        get => DialogueController.Instance;
    }


    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        myCollider.enabled = false;

        progressionData.Value.NoiseInvestigated = true;

        if(dialogueController != null)
        {
            dialogueController.StartConvoWithID(IDConstants.CONVERSATION_BOB_FINAL);
        }


    }


}
