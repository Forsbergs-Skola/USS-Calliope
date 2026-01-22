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

        /*
        if (isLocked)
        {

            //doorTransform.position = ClosedMarker.position;
            float distanceToClosedMarker = Vector3.Distance(ClosedMarker.position, doorTransform.position);
            if (distanceToClosedMarker > 1f)
            {
                Vector3 newPos = Vector3.Lerp(doorTransform.position, ClosedMarker.position, Time.deltaTime);
                doorTransform.position = newPos;
            }
        }
        else
        {
            //doorTransform.position = OpenMarker.position;
            float distanceToOpenMarker = Vector3.Distance(OpenMarker.position, doorTransform.position);
            if (distanceToOpenMarker > 1f)
            {
                Vector3 newPos = Vector3.Lerp(doorTransform.position, OpenMarker.position, Time.deltaTime);
                doorTransform.position = newPos;
            }
        }

        /*
        Vector3 target = !isLocked ? _openPos : _closedPos;
        doorTransform.position = Vector3.MoveTowards(
            doorTransform.position, target, openCloseSpeed * Time.deltaTime);
        Debug.Log(Vector3.Distance(doorTransform.position, target));



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
        */
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
            UnlockDoor();
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
        //SetOpen(true);
    }
}
