using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Movement")]
    public Transform doorTransform;               
    public Vector3 openOffset = new Vector3(2f, 0f, 0f);
    public float openCloseSpeed = 3f;

    [Header("Door State")]
    public bool isLocked = false;                  

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private bool _isOpen;

    void Start()
    {
        if (doorTransform == null)
            doorTransform = transform;

        _closedPos = doorTransform.position;
        _openPos   = _closedPos + openOffset;
    }

    void Update()
    {
        Vector3 target = _isOpen ? _openPos : _closedPos;
        doorTransform.position =
            Vector3.MoveTowards(doorTransform.position, target,
                openCloseSpeed * Time.deltaTime);
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
}