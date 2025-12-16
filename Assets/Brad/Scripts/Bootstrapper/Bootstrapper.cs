using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : Singleton<Bootstrapper>
{
    [SerializeField] private string defaultGameSceneName = "TestScene";
    public string DefaultGameSceneName { get => defaultGameSceneName; }

    //private void Start()
    //{
    //    EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += LoadDefaultScene;
    //}
    //private void OnDestroy()
    //{
    //    EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= LoadDefaultScene;
    //}

    //private void LoadDefaultScene()
    //{
    //    SceneManager.LoadScene(defaultGameSceneName);
    //}

}
