using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class MapCameraSwitch : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera isoCam;
    public CinemachineCamera mapCam;
    
    public InputActionReference toggleMapAction;
    public Olle.Scripts.PlayerController player;

    bool mapOpen;
    public GameObject MinimapRoom;

    void Start()
    {
        mapOpen = false;
        isoCam.Priority = 10;
        mapCam.Priority = 0;
    }

    void OnEnable()
    {
        if (toggleMapAction == null) return;

        toggleMapAction.action.performed += OnToggleMap;
        toggleMapAction.action.Enable();
    }

    void OnDisable()
    {
        if (toggleMapAction == null) return;

        toggleMapAction.action.performed -= OnToggleMap;
        toggleMapAction.action.Disable();
    }

    void OnToggleMap(InputAction.CallbackContext ctx)
    {
        ToggleMap();
    }

    public void ToggleMap()
    {
        mapOpen = !mapOpen;

        if (mapOpen) EnterMap();
        else ExitMap();
    }

    public void EnterMap()
    {
        mapCam.Priority = 20;
        isoCam.Priority = 0;
        
        if (MinimapRoom) MinimapRoom.SetActive(true);
        if (player != null) player.LockMovement();
    }

    public void ExitMap()
    {
        isoCam.Priority = 20;
        mapCam.Priority = 0;
        
        if (MinimapRoom) MinimapRoom.SetActive(false);
        if (player != null) player.UnlockMovement();
    }
}