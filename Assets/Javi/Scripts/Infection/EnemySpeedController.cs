using UnityEngine;

public class EnemySpeedController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InfectionController infectionController;
    [SerializeField] private SimpleMovementAgent movementAgent;

    [Header("Speed Settings by Infection")]
    [SerializeField] private float minBaseSpeed = 1.5f;
    [SerializeField] private float maxBaseSpeed = 4.0f;
    
    [SerializeField] private float minPatrolMultiplier = 0.7f;
    [SerializeField] private float maxPatrolMultiplier = 1.2f;
    
    [SerializeField] private float minChaseMultiplier = 1.2f;
    [SerializeField] private float maxChaseMultiplier = 2.0f;

    private void Awake()
    {
        if (infectionController == null)
            infectionController = GetComponent<InfectionController>();
            
        if (movementAgent == null)
            movementAgent = GetComponent<SimpleMovementAgent>();
    }

    private void OnEnable()
    {
        if (infectionController != null)
            infectionController.OnInfectionChanged.AddListener(UpdateSpeedByInfection);
    }

    private void OnDisable()
    {
        if (infectionController != null)
            infectionController.OnInfectionChanged.RemoveListener(UpdateSpeedByInfection);
    }

    private void UpdateSpeedByInfection(float infectionPercentage)
    {
        if (movementAgent == null) return;
        
        float infectionNormalized = infectionPercentage / 100f;
        
        // Actualiza velocidades basadas en infección
        float baseSpeed = Mathf.Lerp(minBaseSpeed, maxBaseSpeed, infectionNormalized);
        float patrolMultiplier = Mathf.Lerp(minPatrolMultiplier, maxPatrolMultiplier, infectionNormalized);
        float chaseMultiplier = Mathf.Lerp(minChaseMultiplier, maxChaseMultiplier, infectionNormalized);
        
        movementAgent.SetBaseSpeed(baseSpeed);
        movementAgent.SetPatrolMultiplier(patrolMultiplier);
        movementAgent.SetChaseMultiplier(chaseMultiplier);
        
        Debug.Log($"[SpeedController] Infection: {infectionPercentage}% -> BaseSpeed: {baseSpeed}");
    }

    private void Start()
    {
        // Inicializa con la infección actual
        if (infectionController != null)
            UpdateSpeedByInfection(infectionController.InfectionPercentage);
    }
}