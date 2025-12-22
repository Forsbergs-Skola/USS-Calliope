using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActionsAsset;   // drag PlayerInputActions here in Inspector

    [Header("Door + Indicator")]
    public SlidingDoor lockedDoor;
    public Renderer lockIndicator;

    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    bool playerInRange;
    bool activated;

    InputAction interactAction;

    void Awake()
    {
        var actionsAsset = inputActionsAsset;
        if (actionsAsset == null)
        {
            Debug.LogError("InputActionsAsset reference not set on Terminal!");
            return;
        }

        // Try "Player/Interact" first; if null, try just "Interact"
        interactAction = actionsAsset.FindAction("Player/Interact", throwIfNotFound: false);
        if (interactAction == null)
            interactAction = actionsAsset.FindAction("Interact", throwIfNotFound: false);

        if (interactAction == null)
            Debug.LogError("Could not find action 'Player/Interact' or 'Interact' in the asset.");
    }

    void OnEnable()
    {
        if (interactAction == null) return;
        interactAction.performed += OnInteract;
        interactAction.Enable();
    }

    void OnDisable()
    {
        if (interactAction == null) return;
        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    void Start()
    {
        if (lockIndicator != null)
            lockIndicator.material.color = lockedColor;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange || activated) return;

        activated = true;

        if (lockIndicator != null)
            lockIndicator.material.color = unlockedColor;

        if (lockedDoor != null)
            lockedDoor.UnlockDoor();

        Debug.Log("Terminal activated! Door unlocked.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
