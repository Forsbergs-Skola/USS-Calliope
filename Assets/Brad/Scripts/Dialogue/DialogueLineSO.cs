using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLineSO", menuName = "Dialogue/DialogueLineSO")]
public class DialogueLineSO : ScriptableObject
{
    
    [SerializeField] private string lineID = string.Empty;
    [SerializeField] private string speakerName = string.Empty;
    //[SerializeField] private bool isQuit;
    //[SerializeField] private string onLineStartedString = string.Empty;
    //[SerializeField] private string onLineEndedString = string.Empty;
    [Multiline][SerializeField] private string lineText = string.Empty;
    [Range(0.001f, 1.0f)] [SerializeField] private float characterRevealInterval = 0.002f;
    [SerializeField] private string portraitTexturePath = string.Empty;


    public string LineID { get => lineID; }
    public string SpeakerName { get => speakerName; }
    public string LineText { get => lineText; }
    public float CharacterRevealInterval { get => characterRevealInterval; }
    public string PortraitTexturePath { get => portraitTexturePath; }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(lineID))
        {
            Debug.Log("FOO");
            lineID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

    }
}
