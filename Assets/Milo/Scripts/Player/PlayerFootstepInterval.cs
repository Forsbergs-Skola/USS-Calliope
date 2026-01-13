using UnityEngine;

public class PlayerFootstepInterval : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Olle.Scripts.PlayerController playerController;
    [SerializeField] private PlayerFootstepAudio footstepAudio;

    [Header("Settings")]
    [Range(0.8f, 1.5f)]
    [SerializeField] private float speedScale = 1.15f;

    private float _stepTimer;
    private bool _wasMoving;

    private PlayerStamina playerStamina;

    public Olle.Scripts.PlayerController PlayerController { get => playerController; set => playerController = value; }
    public PlayerFootstepAudio FootstepAudio { get => footstepAudio; set => footstepAudio = value; }
    public float SpeedScale { get => speedScale; set => speedScale = value; }
    public float StepTimer { get => _stepTimer; set => _stepTimer = value; }
    public bool WasMoving { get => _wasMoving; set => _wasMoving = value; }
    public PlayerStamina PlayerStamina { get => playerStamina; set => playerStamina = value; }

    private void Awake()
    {
        PlayerStamina = PlayerController.GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        bool isMoving = PlayerController.IsMoving;

        if (!isMoving)
        {
            StepTimer = 0f;
            WasMoving = false;
            return;
        }

        if (!WasMoving)
        {
            StepTimer = GetCurrentInterval() * 0.2f;
            WasMoving = true;
        }

        StepTimer += Time.deltaTime;

        float currentInterval = GetCurrentInterval() * SpeedScale;

        if (StepTimer >= currentInterval)
        {
            FootstepAudio?.PlayFootstep();
            StepTimer -= currentInterval;
        }
    }

    private float GetCurrentInterval()
    {
        if (PlayerController.IsCrouching) return PlayerController.crouchStepInterval;
        if (PlayerController.IsSprinting && PlayerStamina.isTired == false) return PlayerController.runStepInterval;
        return PlayerController.walkStepInterval;
    }
}