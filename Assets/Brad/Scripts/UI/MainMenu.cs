using UnityEngine;
using UnityEngine.UI;
using Events;

public class MainMenu : MonoBehaviour, ICanvasUI
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(HandleNewGamePressed);
    }
    private void OnDisable()
    {
        newGameButton.onClick.RemoveAllListeners();
    }

    private void HandleNewGamePressed()
    {
        newGamePressedEvent.TriggerEvent();
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
        if (foregrounded) { GetComponent<Canvas>().sortingOrder = 10; }
        else { GetComponent<Canvas>().sortingOrder = 0; }
    }
    public int GetSortingOrder()
    {
        return GetComponent<Canvas>().sortingOrder;
    }


}
