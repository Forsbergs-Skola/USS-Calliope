using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemDDOL : Singleton<EventSystemDDOL>
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleOnSceneLoaded;
    }

    private void HandleOnSceneLoaded(Scene _scene, LoadSceneMode _loadMode)
    {
        EventSystem[] eventSystems = GameObject.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (EventSystem es in eventSystems)
        {
            if(es.gameObject != gameObject) { Destroy(es.gameObject); }
        }
    }
}
