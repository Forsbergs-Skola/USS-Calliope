using UnityEngine;
using UnityEngine.SceneManagement;
using Events;

public class Bootstrapper : Singleton<Bootstrapper>
{
    [SerializeField] private string defaultGameSceneName = "TestScene";
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent loadGamePressedEvent;

    //public string DefaultGameSceneName { get => defaultGameSceneName; }


    private void Start()
    {
        UIController.Instance.ShowCanvas(EnumCanvasUIName.MAIN_MENU);
    }

    private void OnEnable()
    {
        newGamePressedEvent.OnEventTriggered += HandleNewGamePressedEvent;
        loadGamePressedEvent.OnEventTriggered += HandleLoadGamePressedEvent;
    }
    private void OnDisable()
    {
        newGamePressedEvent.OnEventTriggered -= HandleNewGamePressedEvent;
        loadGamePressedEvent.OnEventTriggered -= HandleLoadGamePressedEvent;
    }

    private void HandleNewGamePressedEvent()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent(); //Data controller initializes game data
        SceneManager.LoadScene(defaultGameSceneName);
    }
    private void HandleLoadGamePressedEvent()
    {
        SaveService.Load(); // Save service triggers SavedGameLoadedEvent, Datacontroller ingests it
        SceneManager.LoadScene(DataController.Instance.ProgressionRuntimeData.Value.SceneName);
    }

}
