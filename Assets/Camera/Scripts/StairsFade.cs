using UnityEngine;

public class StairFadeTrigger : MonoBehaviour
{
    [SerializeField] private Transform floorAboveRoot; // empty parent
    [SerializeField] private string playerTag = "Player";

    private FadeObject[] fadeTargets;

    void Awake()
    {
        fadeTargets = floorAboveRoot.GetComponentsInChildren<FadeObject>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        foreach (var f in fadeTargets) f.FadeOut();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        foreach (var f in fadeTargets) f.FadeIn();
    }
}