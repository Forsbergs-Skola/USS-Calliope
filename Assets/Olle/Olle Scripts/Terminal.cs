using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    [Header("Door + Indicator")]
    public SlidingDoor lockedDoor;
    public Renderer lockIndicator;

    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    [Header("UI")] public GameObject pressEText;

    bool playerInRange;
    bool activated;

    InputAction interactAction;

    [SerializeField] private string terminalWorldID;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;

    void Awake()
    {
        var actionsAsset = inputActionsAsset;
        if (actionsAsset == null)
        {
            Debug.LogError("InputActionsAsset reference not set on Terminal!");
            return;
        }

        interactAction = actionsAsset.FindAction("Player/Interact", throwIfNotFound: false);
        if (interactAction == null)
            interactAction = actionsAsset.FindAction("Interact", throwIfNotFound: false);

        if (interactAction == null)
            Debug.LogError("Could not find action 'Player/Interact' or 'Interact' in the asset.");
        
        audioSource = GetComponent<AudioSource>();
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

        //Debug.Log("FOO");
        //Debug.Log($"First condition: {!playerInRange}");
        //Debug.Log($"Second condition: {activated}");


        if (!playerInRange || activated) return;

        activated = true;

        if (pressEText != null)
        {
            Destroy(pressEText);
        }

        if (lockIndicator != null)
            lockIndicator.material.color = unlockedColor;

        if (lockedDoor != null)
        {
            if (audioSource != null && audioClip != null)
                audioSource.PlayOneShot(audioClip);
            else

            lockedDoor.UnlockAndBecomeFreeDoor();
            progressionRuntimeData.Value.AddUnlockedTerminalWorldID(terminalWorldID);
            Debug.Log("Didnt write terminal ID");
            
        }

        //Debug.Log("BAR");

        Debug.Log("Terminal activated! Door unlocked and behaves like a normal door.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("The player is in range.");
            playerInRange = true;
        }
            

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("The player is out of range");
            playerInRange = false;
        }
            
    }
}
