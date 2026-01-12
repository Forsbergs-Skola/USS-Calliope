using UnityEngine;
using UnityEngine.UI;
using Events;

public class MainMenu : MonoBehaviour, ICanvasUI
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent loadGamePressedEvent;

    private void OnEnable()
    {
        Cursor.visible = true;
        continueButton.gameObject.SetActive(SaveService.SaveExists());

        newGameButton.onClick.AddListener(HandleNewGamePressed);
        continueButton.onClick.AddListener(HandleContinuePressed);
        quitButton.onClick.AddListener(HandleQuit);
    }
    private void OnDisable()
    {
        newGameButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    private void HandleNewGamePressed()
    {
        newGamePressedEvent.TriggerEvent();
    }

    private void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    private void HandleContinuePressed()
    {
        loadGamePressedEvent.TriggerEvent();
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
