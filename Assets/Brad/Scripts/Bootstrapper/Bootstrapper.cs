using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Events;

public class Bootstrapper : Singleton<Bootstrapper>
{
    private const float pressedInputDamp = 0.05f;

    [SerializeField] private string defaultGameSceneName = "TestScene";
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent loadGamePressedEvent;
    [SerializeField] private EmptyPayloadEvent logoSplashFinishedEvent;
    [SerializeField] private EmptyPayloadEvent introCutsceneFinished;

    [SerializeField] private string introCutsceneName = string.Empty;

    [SerializeField] private bool skipSplash = false;
    [SerializeField] private bool skipIntroCutscene = false;

    private bool pressedInputDampened = false;
    private bool isFreshStart = true;

    private void Start()
    {
        isFreshStart = false;
        //UIController.Instance.ShowCanvas(EnumCanvasUIName.MAIN_MENU);
        if (skipSplash)
        {
            UIController.Instance.ShowCanvas(EnumCanvasUIName.MAIN_MENU);
        }
        else
        {
            UIController.Instance.ShowCanvas(EnumCanvasUIName.LOGO_SPLASH);
        }
       
    }
    private void OnEnable()
    {
        newGamePressedEvent.OnEventTriggered += HandleNewGamePressedEvent;
        loadGamePressedEvent.OnEventTriggered += HandleLoadGamePressedEvent;
        logoSplashFinishedEvent.OnEventTriggered += HandleOnLogoSplashFinished;
        introCutsceneFinished.OnEventTriggered += HandleIntroCutsceneFinished;
        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }
    private void OnDisable()
    {
        newGamePressedEvent.OnEventTriggered -= HandleNewGamePressedEvent;
        loadGamePressedEvent.OnEventTriggered -= HandleLoadGamePressedEvent;
        logoSplashFinishedEvent.OnEventTriggered -= HandleOnLogoSplashFinished;
        introCutsceneFinished.OnEventTriggered -= HandleIntroCutsceneFinished;
        SceneManager.sceneLoaded -= HandleOnSceneLoaded;
    }

    private void Update()
    {
        if (UIController.Instance == null) return;
        if (!UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.HUD)) return;
        if (pressedInputDampened) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pressedInputDampened = true;
            StartCoroutine(StartPressedInputCooldown());
            TogglePause();
        }
    }

    // API //
    public void PauseGame(bool paused)
    {
        if (paused) { Time.timeScale = 0f; Cursor.visible = true; }
        else { Time.timeScale = 1f; Cursor.visible = false; }
    }
    public void TogglePause()
    {

        if (DataController.Instance.PlayerRuntimeData.Value.Health <= 0) { return; }
        
        if (!UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.PAUSE))
        {
            UIController.Instance.ShowCanvas(EnumCanvasUIName.PAUSE);
        }
        else
        {
            UIController.Instance.RemoveCanvas(EnumCanvasUIName.PAUSE);
        }
    }
    public void ReturnToMain()
    {
        SceneManager.LoadScene("Bootstrap");
    }

    // Private //

    private void HandleOnLogoSplashFinished()
    {
        UIController.Instance.ClearCanvases();
        UIController.Instance.ShowCanvas(EnumCanvasUIName.MAIN_MENU);
    }

    private void HandleNewGamePressedEvent()
    {
        if (skipIntroCutscene)
        {
            EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent(); //Data controller initializes game data
            SceneManager.LoadScene(defaultGameSceneName);
        }
        else
        {
            SceneManager.LoadScene(introCutsceneName);
        }
        
    }
    private void HandleLoadGamePressedEvent()
    {
        SaveService.Load(); // Save service triggers SavedGameLoadedEvent, Datacontroller ingests it
        //SceneManager.LoadScene(DataController.Instance.ProgressionRuntimeData.Value.SceneName);
        //Debug.Log(DataController.Instance.ProgressionRuntimeData.Value.SceneName);
        //SceneManager.LoadScene("NewLevel");
    }

    private void HandleIntroCutsceneFinished()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent(); //Data controller initializes game data
        SceneManager.LoadScene(defaultGameSceneName);
    }

    private System.Collections.IEnumerator StartPressedInputCooldown()
    {
        yield return new WaitForSecondsRealtime(pressedInputDamp);
        pressedInputDampened = false;
    }

    private void HandleOnSceneLoaded(Scene scn, LoadSceneMode loadMode)
    {
        if (scn.name == "Bootstrap")
        {
            if (isFreshStart) return;
            UIController.Instance.ClearCanvases();
            UIController.Instance.ShowCanvas(EnumCanvasUIName.MAIN_MENU);
        }
    }

}
