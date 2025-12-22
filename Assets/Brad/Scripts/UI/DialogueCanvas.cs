using UnityEngine;

public class DialogueCanvas : MonoBehaviour, ICanvasUI
{
    private void OnEnable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(true);
        }
    }
    private void OnDisable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(false);
        }
    }

    // Interface Methods //
    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.DIALOGUE;
    }
    public Canvas GetCanvas()
    {
        return GetComponent<Canvas>();
    }
    public void ForegroundCanvas(bool foregrounded)
    {
        if (foregrounded) { GetComponent<Canvas>().sortingOrder = UIController.FOREGROUND_SORT_ORDER; }
        else { GetComponent<Canvas>().sortingOrder = UIController.BACKGROUND_SORT_ORDER; }
    }
    public int GetSortingOrder()
    {
        return GetComponent<Canvas>().sortingOrder;
    }
}
