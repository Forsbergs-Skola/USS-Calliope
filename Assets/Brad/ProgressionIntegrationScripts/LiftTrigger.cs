using UnityEngine;

public class LiftTrigger : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionData;

    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        myCollider.enabled = false;

        progressionData.Value.LiftAccessed = true;


    }

}
