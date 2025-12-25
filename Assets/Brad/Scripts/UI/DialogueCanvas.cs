using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueCanvas : MonoBehaviour, ICanvasUI
{

    //[SerializeField] TMP_Text testText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private Button continueButton;
    [SerializeField] private RawImage portraitImage;

    private DialogueConversationSO currentConvo = null;
    private int currentLineIdx = -1;

    private void OnEnable()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(true);
        }
        continueButton.onClick.AddListener(AdvanceDialogue);
        continueButton.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Resources.UnloadUnusedAssets();
        continueButton.onClick.RemoveAllListeners();
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.PauseGame(false);
        }
    }

    private void AdvanceDialogue()
    {
        if (currentConvo == null) return;
        if (currentLineIdx >= currentConvo.Lines.Count - 1)
        {
            EventRelay.Instance.UIEvents.DialogueConvoFinishedEvent.TriggerEvent(currentConvo.ConvoID);
        }
        else
        {
            currentLineIdx++;
            DialogueLineSO thisLineData = currentConvo.Lines[currentLineIdx];

            // get all the required data about the current line
            string speakerName = thisLineData.SpeakerName;
            string thisLine = thisLineData.LineText;
            string portraitResourcePath = thisLineData.PortraitTexturePath;
            float revealInterval = thisLineData.CharacterRevealInterval;
            string lineID = thisLineData.LineID;

            // put it on-screen
            if (!string.IsNullOrEmpty(portraitResourcePath)) HandlePortrait(portraitResourcePath);
            speakerNameText.text = speakerName;
            StartCoroutine(LineRevealer(thisLine, revealInterval, lineID));

        }
    }

    private void HandlePortrait(string resourcePath)
    {
        portraitImage.texture = Resources.Load<Texture>(resourcePath);
    }
    
    private System.Collections.IEnumerator LineRevealer(string lineText, float revealInterval, string convoID)
    {
        EventRelay.Instance.UIEvents.DialogueLineStartedEVent.TriggerEvent(convoID);
        continueButton.gameObject.SetActive(false);
        dialogueText.text = "";
        foreach(char character in lineText)
        {
            dialogueText.text += character;
            yield return new WaitForSecondsRealtime(revealInterval);
        }
        continueButton.gameObject.SetActive(true);
        EventRelay.Instance.UIEvents.DialogueLineFinishedEvent.TriggerEvent(convoID);
    }
    

    /////////
    // API //
    /////////
    
    public void LaunchConversation(DialogueConversationSO convo)
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.UIEvents.DialogueConvoStartedEVent.TriggerEvent(convo.ConvoID);

        currentConvo = convo;
        AdvanceDialogue();
        


        /*
        string testStr = "";

        // TODO: replace with actual UI presentation logic...
        foreach (DialogueLineSO line in convo.Lines)
        {
            string thisLine = $"{line.SpeakerName} says: {line.LineText}";
            testStr += $"- {thisLine}\n\n";
            
            //Debug.Log($"{line.SpeakerName} says: {line.LineText}");
        }
        testText.text = testStr;
        */
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
