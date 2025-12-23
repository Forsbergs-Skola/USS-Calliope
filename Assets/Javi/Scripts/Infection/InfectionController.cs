using UnityEngine;
using UnityEngine.Events;

public class InfectionController : MonoBehaviour, IInfectedEntity
{
    [Header("Infection Settings")]
    [SerializeField] private float infectionPercentage;
    [SerializeField] private bool showDebugGizmos;

    [Header("Events")]
    public UnityEvent<float> OnInfectionChanged;
    public UnityEvent OnPhaseChanged;

    public float InfectionPercentage => infectionPercentage;

    public void IncreaseInfection(float amount)
    {
        infectionPercentage = Mathf.Clamp(infectionPercentage + amount, 0f, 100f);
        OnInfectionChanged?.Invoke(infectionPercentage);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.5f + infectionPercentage * 0.01f);
    }
#endif
}