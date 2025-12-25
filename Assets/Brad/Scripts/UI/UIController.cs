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
    PAUSE,
    LOGO_SPLASH,
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

    [Header("Prefabs")]
    [SerializeField] private List<StructCanvasUIPrefab> canvasPrefabs;

    public const int FOREGROUND_SORT_ORDER = 10;
    public const int BACKGROUND_SORT_ORDER = 0;

    private List<ICanvasUI> GetActiveCanvases()
    {
        List<ICanvasUI> canvasUIs = new List<ICanvasUI>();
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            if (root.GetComponent<ICanvasUI>() != null)
            {
                canvasUIs.Add(root.GetComponent<ICanvasUI>());
            }
        }
        return canvasUIs;
    }
    
    private bool GetIsCanvasActive(EnumCanvasUIName canvName)
    {
        List<ICanvasUI> canvases = GetActiveCanvases();
        foreach (ICanvasUI canvasUI in canvases)
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
        ICanvasUI canvasUI = GetActiveCanvases().Find(canv => canv.GetCanvasName() == canvasName);
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
        ForegroundCanvas(canvasName);
    }
    public void ForegroundCanvas(EnumCanvasUIName canvasName)
    {
        if (!GetIsCanvasActive(canvasName)) { Debug.LogError($"Cannot foreground. {canvasName.ToString()} is not active"); return; }
        foreach (ICanvasUI canvasUI in GetActiveCanvases())
        {
            if (canvasUI.GetCanvasName() == canvasName) { canvasUI.ForegroundCanvas(true); }
            else { canvasUI.ForegroundCanvas(false); }
        }
    }
    public void RemoveCanvas(EnumCanvasUIName canvasName)
    {
        ICanvasUI canvasUI = GetActiveCanvas(canvasName);
        if (canvasUI == null) { Debug.LogError($"Cannot remove {canvasName.ToString()} is not active"); return; }
        Destroy(canvasUI.GetCanvas().gameObject);

        int canvCount = GetActiveCanvases().Count;
        if (canvCount <= 0) return;

        // if no canvas is foregrounded, foreground the top one
        bool oneIsForegrounded = false;
        List<ICanvasUI> canvases = GetActiveCanvases();

        foreach(ICanvasUI _canvasUI in canvases)
        {
            if (_canvasUI.GetSortingOrder() >= FOREGROUND_SORT_ORDER) { oneIsForegrounded = true; }
        }
        if (!oneIsForegrounded)
        {
            ForegroundCanvas(canvases[canvases.Count - 1].GetCanvasName());
        }
    }

    public bool GetIsCanvasUp(EnumCanvasUIName canvasName)
    {
        return GetIsCanvasActive(canvasName);
    }
    public void ClearCanvases()
    {
        List<ICanvasUI> canvases = GetActiveCanvases();
        if (canvases.Count > 0)
        {
            foreach(ICanvasUI canvasUI in canvases)
            {
                Destroy(canvasUI.GetCanvas().gameObject);
            }
        }
    }

    public ICanvasUI GetReferenceToCanvas(EnumCanvasUIName canvasName)
    {
        ICanvasUI canv = null;
        canv = GetActiveCanvas(canvasName);
        return canv;
    }
}
