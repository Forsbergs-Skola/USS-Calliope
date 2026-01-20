using UnityEngine;

public enum DoorLockType
{
    None,       // Open
    Keycard,    // Keycard
    Terminal,   // Terminal
    KeypadCode  // Keypad 
}

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Movement")]
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0f, -5f, 0f);
    public float openCloseSpeed = 3f;

    [Header("Door Visual")] 
    public float hideThresholdY = -1F;
    

    [Header("Locking")]
    public DoorLockType lockType = DoorLockType.None;
    public bool isLocked = false;
    public int keyId = 0;   // Which keycard opens this door

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private bool _isOpen;

    void Start()
    {
        if (doorTransform == null)
            doorTransform = transform;

        _closedPos = doorTransform.position;
        _openPos = _closedPos + openOffset;
    }

    void Update()
    {
        Vector3 target = _isOpen ? _openPos : _closedPos;
        doorTransform.position = Vector3.MoveTowards(
            doorTransform.position, target, openCloseSpeed * Time.deltaTime);

        //For hiding door
        if (_isOpen && doorTransform.position.y < hideThresholdY)
        {
            if (doorTransform.gameObject.activeSelf)
                doorTransform.gameObject.SetActive(false);
        }
        else if (!_isOpen && doorTransform.position.y > hideThresholdY)
        {
            if (!doorTransform.gameObject.activeSelf)
                doorTransform.gameObject.SetActive(true);
        }
    }
    

    public void SetOpen(bool open)
    {
        if (isLocked) return;
        _isOpen = open;
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void LockDoor()
    {
        isLocked = true;
        _isOpen = false;
    }
    
    public bool TryUnlockWithKeycard(int cardKeyId)
    {
        if (!isLocked || lockType != DoorLockType.Keycard) return false;

        if (cardKeyId == keyId)
        {
            UnlockDoor();
            
            lockType = DoorLockType.None;

            SetOpen(true);
            return true;
        }
        return false;
    }
    
    public void UnlockAndBecomeFreeDoor()
    {
        UnlockDoor();
        lockType = DoorLockType.None;
        SetOpen(true);
    }
}
