using UnityEngine;

public class LinaxTrigger : MonoBehaviour
{
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (LinaxFinder.TrySetLinaxFound(true))
        {
            myCollider.enabled = false;
            if (DialogueController.Instance != null)
            {
                DialogueController.Instance.StartConvoWithID(IDConstants.LINAX_MONOLOGUE);
            }

        }
    }
}
