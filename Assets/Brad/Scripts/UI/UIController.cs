using UnityEngine;
using Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public interface ICanvasUI
{
    public EnumCanvasUIName GetCanvasName();
    public Canvas GetCanvas();
    public void ForegroundCanvas(bool foregrounded);
    public int GetSortingOrder();
}
public enum EnumCanvasUIName
{
    MAIN_MENU,
    HUD,
    DIALOGUE,
    TRANSITION_SCREEN,
    PAUSE,
    OBJECTIVES,
    INVENTORY
}
[System.Serializable]
public struct StructCanvasUIPrefab
{
    public EnumCanvasUIName canvasName;
    public GameObject canvasPrefab;
}

public class UIController : Singleton<UIController>
{

    [Header("Event Channels")]
    [SerializeField] private IRuntimeDataPayloadEvent runtimeDataUpdatedEvent;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;

    [Header("Prefabs")]
    [SerializeField] private List<StructCanvasUIPrefab> canvasPrefabs;


    //private List<EnumCanvasUIName> activeCanvases = new List<EnumCanvasUIName>();
    private List<ICanvasUI> activeCanvases = new List<ICanvasUI>();

    public const int FOREGROUND_SORT_ORDER = 10;
    public const int BACKGROUND_SORT_ORDER = 0;

    private void Start()
    {
        ShowCanvas(EnumCanvasUIName.MAIN_MENU);
    }


    private void OnEnable()
    {
        newGamePressedEvent.OnEventTriggered += HandleNewGamePressedEvent;
    }

    private void OnDisable()
    {
        newGamePressedEvent.OnEventTriggered -= HandleNewGamePressedEvent;
    }

    private void HandleNewGamePressedEvent()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent(); //Data controller initializes game data
        SceneManager.LoadScene(Bootstrapper.Instance.DefaultGameSceneName);
    }

    private void HandleLoadGamePressedEvent()
    {
        SaveService.Load(); // Save service triggers SavedGameLoadedEvent, Datacontroller ingests it
        SceneManager.LoadScene(DataController.Instance.ProgressionRuntimeData.Value.SceneName);
    }

    
    
    private bool GetIsCanvasActive(EnumCanvasUIName canvName)
    {
        foreach (ICanvasUI canvasUI in activeCanvases)
        {
            if (canvasUI.GetCanvasName() == canvName) { return true; }
        }
        return false;
    }

    private GameObject? GetCanvasPrefab(EnumCanvasUIName canvasName)
    {
        foreach(StructCanvasUIPrefab prefab in canvasPrefabs)
        {
            if (prefab.canvasName == canvasName) { return prefab.canvasPrefab; }
        }
        return null;
    }
    private ICanvasUI? GetActiveCanvas(EnumCanvasUIName canvasName)
    {
        if (!GetIsCanvasActive(canvasName)) { Debug.LogError($"{canvasName.ToString()} is not active"); return null; }
        ICanvasUI canvasUI = activeCanvases.Find(canv => canv.GetCanvasName() == canvasName);
        return canvasUI;
    }

    /////////
    // API //
    /////////
    public void ShowCanvas(EnumCanvasUIName canvasName)
    {
        if (GetIsCanvasActive(canvasName)) { Debug.LogWarning($"{canvasName.ToString()} is already active"); return; }
        if (GetCanvasPrefab(canvasName) == null) { Debug.LogError($"No prefab for canvas: {canvasName.ToString()}"); return; }
        GameObject canvasObj = GetCanvasPrefab(canvasName);
        Instantiate(canvasObj);
        activeCanvases.Add(canvasObj.GetComponent<ICanvasUI>());
        ForegroundCanvas(canvasName);
    }
    public void ForegroundCanvas(EnumCanvasUIName canvasName)
    {
        if (!GetIsCanvasActive(canvasName)) { Debug.LogError($"Cannot foreground. {canvasName.ToString()} is not active"); return; }
        foreach (ICanvasUI canvasUI in activeCanvases)
        {
            //Canvas canv = canvasUI.GetCanvas();
            if (canvasUI.GetCanvasName() == canvasName) { canvasUI.ForegroundCanvas(true); }
            else { canvasUI.ForegroundCanvas(false); }
        }
    }
    public void RemoveCanvas(EnumCanvasUIName canvasName)
    {
        ICanvasUI canvasUI = GetActiveCanvas(canvasName);
        if (canvasUI == null) { Debug.LogError($"Cannot remove {canvasName.ToString()} is not active"); return; }
        activeCanvases.Remove(canvasUI);
        Destroy(canvasUI.GetCanvas().gameObject);

        if (activeCanvases.Count <= 0) return;

        // if no canvas is foregrounded, foreground the top one
        bool oneIsForegrounded = false;
        foreach(ICanvasUI _canvasUI in activeCanvases)
        {
            if (_canvasUI.GetSortingOrder() >= FOREGROUND_SORT_ORDER) { oneIsForegrounded = true; }
        }
        if (!oneIsForegrounded)
        {
            ForegroundCanvas(activeCanvases[activeCanvases.Count - 1].GetCanvasName());
        }


    }
}
