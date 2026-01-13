using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseCanvas : MonoBehaviour, ICanvasUI
{
    [SerializeField] private EnumPausePanel defaultPanel;
    [SerializeField] private Button mainButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button toggleViewButton;
    [SerializeField] private TMP_Text toggleButtonText;
    [SerializeField] private ObjectivesPanel objectivesPanel;
    [SerializeField] private InventoryPanel inventoryPanel;
    

    private enum EnumPausePanel
    {
        NONE,
        OBJECTIVES,
        INVENTORY
    }
    private EnumPausePanel _currentPanel = EnumPausePanel.NONE;
    private EnumPausePanel currentPanel
    {
        get => _currentPanel;
        set
        {
            if (value == _currentPanel) return;
            _currentPanel = value;
            switch (_currentPanel)
            {
                case EnumPausePanel.OBJECTIVES:
                    inventoryPanel.gameObject.SetActive(false);
                    objectivesPanel.gameObject.SetActive(true);
                    toggleButtonText.text = "View Inventory";
                    break;
                case EnumPausePanel.INVENTORY:
                    inventoryPanel.gameObject.SetActive(true);
                    objectivesPanel.gameObject.SetActive(false);
                    toggleButtonText.text = "View Objectives";
                    break;
                default:
                    inventoryPanel.gameObject.SetActive(false);
                    objectivesPanel.gameObject.SetActive(false);
                    toggleButtonText.text = "  ";
                    break;
            }
        }
    }

    private void OnEnable()
    {
        objectivesPanel.gameObject.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);

        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(true);
        }
        mainButton.onClick.AddListener(HandleMainButtonPressed);
        saveButton.onClick.AddListener(HandleSaveButtonPressed);
        backButton.onClick.AddListener(HandleBackButtonPressed);
        toggleViewButton.onClick.AddListener(HandleToggleViewButtonPressed);

        currentPanel = defaultPanel;
        

    }
    private void OnDisable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(false);
        }
        mainButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        toggleViewButton.onClick.RemoveAllListeners();
    }

    private void HandleMainButtonPressed()
    {
        if (Bootstrapper.Instance != null) { Bootstrapper.Instance.ReturnToMain(); }
    }
    private void HandleSaveButtonPressed()
    {
        if (DataController.Instance == null) return;
        Debug.Log("SAVING GAME");
        PlayerData playerData = DataController.Instance.PlayerRuntimeData.Value;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;


        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerData.LastPosition = playerObj.transform.position;

        SaveService.Save(playerData, invData, progData);
    }
    private void HandleBackButtonPressed()
    {
        if (Bootstrapper.Instance != null) { Bootstrapper.Instance.TogglePause(); }
    }
    private void HandleToggleViewButtonPressed()
    {
        switch (currentPanel)
        {
            case EnumPausePanel.OBJECTIVES:
                currentPanel = EnumPausePanel.INVENTORY;
                break;
            case EnumPausePanel.INVENTORY:
                currentPanel = EnumPausePanel.OBJECTIVES;
                break;
        }
    }

    private void HandleOnPointerEnter()
    {
        Debug.Log("PointerEnter");
    }


    // Interface Methods //
    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.PAUSE;
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
