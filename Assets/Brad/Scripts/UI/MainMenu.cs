using UnityEngine;
using UnityEngine.UI;
using Events;

public class MainMenu : MonoBehaviour, ICanvasUI
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(HandleNewGamePressed);
        quitButton.onClick.AddListener(HandleQuit);
    }
    private void OnDisable()
    {
        newGameButton.onClick.RemoveAllListeners();
    }

    private void HandleNewGamePressed()
    {
        newGamePressedEvent.TriggerEvent();
    }

    private void HandleQuit()
    {
        Application.Quit();
    }

    // Interface Methods //

    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.MAIN_MENU;
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
