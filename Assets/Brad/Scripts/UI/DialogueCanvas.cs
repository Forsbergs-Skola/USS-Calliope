using UnityEngine;
using TMPro;

public class DialogueCanvas : MonoBehaviour, ICanvasUI
{

    [SerializeField] TMP_Text testText;

    private void OnEnable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(true);
        }
    }
    private void OnDisable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(false);
        }
    }

    /////////
    // API //
    /////////
    
    public void LaunchConversation(DialogueConversationSO convo)
    {

        string testStr = "";

        // TODO: replace with actual UI presentation logic...
        foreach (DialogueLineSO line in convo.Lines)
        {
            string thisLine = $"{line.SpeakerName} says: {line.LineText}";
            testStr += $"- {thisLine}\n\n";
            
            //Debug.Log($"{line.SpeakerName} says: {line.LineText}");
        }
        testText.text = testStr;
    }

    // Interface Methods //
    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.DIALOGUE;
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
