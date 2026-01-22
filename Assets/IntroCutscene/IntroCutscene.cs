using UnityEngine;
using Events;
using TMPro;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent introCutsceneFinishedEvent;
    [SerializeField] private TMP_Text countdownText;


    // Todo the whole cutscene...
    // when the cutscene is finished, call OnCutsceneFinished()
    // and the bootstrapper will load the game normally.


    private void Start()
    {
        StartCoroutine(FakeCutscene()); // placeholder
    }


    public void OnCutsceneFinished()
    {
        introCutsceneFinishedEvent.TriggerEvent();
    }


    private System.Collections.IEnumerator FakeCutscene()
    {
        // placeholder code   
        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        OnCutsceneFinished();
    }


}
