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
    public Vector3 openOffset = new Vector3(0f, -5f, 0f);
    public float openCloseSpeed = 3f;

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


    private Vector3 _closedPos;
    private Vector3 _openPos;

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

        if(!isLocked) { SetOpen(true); }


        if (doorTransform == null)
            doorTransform = transform;

        _closedPos = doorTransform.position;
        _openPos = _closedPos + openOffset;
    }

    void Update()
    {
        Vector3 target = !isLocked ? _openPos : _closedPos;
        doorTransform.position = Vector3.MoveTowards(
            doorTransform.position, target, openCloseSpeed * Time.deltaTime);

        //For hiding door
        if (!isLocked && doorTransform.position.y < hideThresholdY)
        {
            if (doorTransform.gameObject.activeSelf)
                doorTransform.gameObject.SetActive(false);
        }
        else if (isLocked && doorTransform.position.y > hideThresholdY)
        {
            if (!doorTransform.gameObject.activeSelf)
                doorTransform.gameObject.SetActive(true);
        }
    }
    

    public void SetOpen(bool open)
    {
        if (!isLocked) return;
        isLocked = false;
    }

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

        //return (inventoryRuntime.Value.GetQuestItemIDs().Contains(keyID));

        
        InventoryData invData = inventoryRuntime.Value;
        List<string> questItemIDs = invData.GetQuestItemIDs();
        if (questItemIDs.Contains(keyID))
        {
            Debug.Log("I AM NOW UNLOCKED!");
            return true;
        }




        //DataController dataController = DataController.Instance;
        //if(dataController == null)
        Debug.Log("I AM STILL LOCKED :(");
        return false;
        
    }

    /*
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
    */

    public void UnlockAndBecomeFreeDoor()
    {
        UnlockDoor();
        lockType = DoorLockType.None;
        SetOpen(true);
    }
}
