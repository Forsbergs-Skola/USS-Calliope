using UnityEngine;

public class BreakableProgressionDataHandler : MonoBehaviour
{
    [SerializeField] private string worldID = string.Empty;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;

    private void OnEnable()
    {
        if (progressionRuntimeData.Value.GetExhaustedBreakablesList().Contains(worldID))
        {
            // If I've already beed broken then die gracefully...
            Destroy(gameObject);
        }
    }

    public void UpdateBackend()
    {
        // add my unique ID to the list of already-broken breakables
        progressionRuntimeData.Value.AddExhaustedBreakable(worldID);
    }
}
