using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

public class MapCameraSwitch : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera isoCam;
    public CinemachineCamera mapCam;
    
    public InputActionReference toggleMapAction;
    public InputActionReference toggleLevelAction;
    
    public Olle.Scripts.PlayerController player;

    [SerializeField] private GameObject LevelOne;
    [SerializeField] private GameObject LevelTwo;
    private bool LevelOneActive = true;
    

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
        if (toggleMapAction != null)
        {
            toggleMapAction.action.performed += OnToggleMap;
            if (!toggleMapAction.action.enabled) toggleMapAction.action.Enable();
        }

        if (toggleLevelAction != null)
        {
            toggleLevelAction.action.performed += OnToggleLevel;
            if (!toggleLevelAction.action.enabled) toggleLevelAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (toggleMapAction != null)
            toggleMapAction.action.performed -= OnToggleMap;

        if (toggleLevelAction != null)
            toggleLevelAction.action.performed -= OnToggleLevel;
        
    }

    void OnToggleMap(InputAction.CallbackContext ctx)
    {
        ToggleMap();
    }

    void OnToggleLevel(InputAction.CallbackContext ctx)
    {
        if (!mapOpen) return;
        
        LevelOneActive = !LevelOneActive;
        ApplyLevelToggle();
    }

    public void ToggleMap()
    {
        mapOpen = !mapOpen;

        if (mapOpen) EnterMap();
        else ExitMap();
    }

    void ApplyLevelToggle()
    {
        if (LevelOne) LevelOne.SetActive(LevelOneActive);
        if(LevelTwo) LevelTwo.SetActive(!LevelOneActive);
    }

    public void EnterMap()
    {
        mapCam.Priority = 20;
        isoCam.Priority = 0;
        
        if (MinimapRoom) MinimapRoom.SetActive(true);
        if (player != null) player.LockMovement();
        
        player.transform.rotation = Quaternion.LookRotation(Vector3.right);
        
        ApplyLevelToggle();
    }

    public void ExitMap()
    {
        isoCam.Priority = 20;
        mapCam.Priority = 0;
        
        if (MinimapRoom) MinimapRoom.SetActive(false);
        StartCoroutine(UnlockDelay());
    }

    private IEnumerator UnlockDelay()
    {
        yield return new WaitForSeconds(0.8f);
        if (player != null) player.UnlockMovement();
    }
}