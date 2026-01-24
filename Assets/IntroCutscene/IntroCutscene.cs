using System;
using UnityEngine;
using Events;
using TMPro;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent introCutsceneFinishedEvent;
    [SerializeField] private PlayableDirector introCutscenePlayableDirector;
    //[SerializeField] private TMP_Text countdownText;


    // Todo the whole cutscene...
    // when the cutscene is finished, call OnCutsceneFinished()
    // and the bootstrapper will load the game normally.

    void Awake()
    {
        introCutscenePlayableDirector.stopped += CutsceneEnded;
    }

    private void OnDisable()
    {
        introCutscenePlayableDirector.stopped -= CutsceneEnded;
    }

    void CutsceneEnded(PlayableDirector director)
    {
        OnCutsceneFinished();
    }

    public void OnCutsceneFinished()
    {
        introCutsceneFinishedEvent.TriggerEvent();
    }
    


}
