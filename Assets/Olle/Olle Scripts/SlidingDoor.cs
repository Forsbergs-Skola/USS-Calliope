using UnityEngine;
using System.Collections.Generic;

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
    public float lerpSpeed = 3f;

    [Header("Door Visual")] 
    public float hideThresholdY = -1F;
    

    [Header("Locking")]
    public DoorLockType lockType = DoorLockType.None;

    public bool isLocked = false;

    //public int keyId = 0;   // Which keycard opens this door
    [SerializeField] private string keyID = string.Empty;
    [SerializeField] private InventoryRuntimeData inventoryRuntime;

    [SerializeField] private string openedWithTerminalWorldID;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;

    [SerializeField] private Transform OpenMarker;
    [SerializeField] private Transform ClosedMarker;

    private bool _playerInTrigger = false;

    //private Vector3 _closedPos;
    //private Vector3 _openPos;

    //private bool _isOpen;

    void Start()
    {

        if (lockType == DoorLockType.Terminal)
        {
            bool alreadyGot = progressionRuntimeData.Value.GetUnlockedTerminalWorldIDs().Contains(openedWithTerminalWorldID);
            if (alreadyGot)
            {
                UnlockAndBecomeFreeDoor();
            }

            return;
        }


        if (string.IsNullOrEmpty(keyID))
        {
            Debug.LogWarning("You forgot to put the key id in!");
        }

        //if(!isLocked) { SetOpen(true); }
        
        if (doorTransform == null)
            doorTransform = transform;

       // _closedPos = doorTransform.localPosition;
       // _openPos = _closedPos + openOffset;
       //_closedPos = ClosedMarker.position;
       //_openPos = OpenMarker.position;
    }

    void Update()
    {
        bool shouldBeOpen = !isLocked && _playerInTrigger;
        Transform targetMarker = shouldBeOpen ? OpenMarker : ClosedMarker;

        float distance = Vector3.Distance(doorTransform.position, targetMarker.position);

        if (distance > 0.01f)
        {
            doorTransform.position =
                Vector3.Lerp(doorTransform.position, targetMarker.position, lerpSpeed * Time.deltaTime);
        }

        if (shouldBeOpen && doorTransform.position.y < hideThresholdY && doorTransform.gameObject.activeSelf)
        {
            doorTransform.gameObject.SetActive(false);
        }
        else if (!shouldBeOpen && doorTransform.position.y > hideThresholdY && !doorTransform.gameObject.activeSelf)
        {
            doorTransform.gameObject.SetActive(true);
        }
    }

    public void SetPlayerInTrigger(bool InTrigger)
    {
        _playerInTrigger = InTrigger;
    }

/*
    public void SetOpen(bool open)
    {
        if (!isLocked) return;
        isLocked = false;
    }
*/
    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void LockDoor()
    {
        isLocked = true;
    }

    public bool TryUnlock()
    {
        if (inventoryRuntime?.Value == null)
        {
            return false;
        }

        //return (inventoryRuntime.Value.GetQuestItemIDs().Contains(keyID));

        
        InventoryData invData = inventoryRuntime.Value;
        List<string> questItemIDs = invData.GetQuestItemIDs();
        if (questItemIDs.Contains(keyID))
        {
            Debug.Log("I AM NOW UNLOCKED!");

            KeyedDoorProgressionHandler.HandleDoorUnlocked(keyID);

            UnlockDoor();
            return true;
        }

        Debug.Log("I AM STILL LOCKED :(");
        return false;
        
    }

    public void UnlockAndBecomeFreeDoor()
    {
        UnlockDoor();
        lockType = DoorLockType.None;
        //SetOpen(true);
    }
}
