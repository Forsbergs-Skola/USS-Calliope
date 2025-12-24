using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueConversationSO", menuName = "Dialogue/DialogueConversationSO")]
public class DialogueConversationSO : ScriptableObject
{
    [SerializeField] private string convoID = string.Empty;
    [SerializeField] private List<DialogueLineSO> lines;

    public string ConvoID { get => convoID; }
    public List<DialogueLineSO> Lines { get => lines; }

    private void OnValidate()
    {
        if (lines.Count <= 0) { Debug.LogError("A conversation must have at least one line"); }
        if (string.IsNullOrEmpty(convoID))
        {
            convoID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
