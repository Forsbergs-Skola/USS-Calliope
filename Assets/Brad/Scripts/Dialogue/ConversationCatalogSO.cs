using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConversationCatalogSO", menuName = "Dialogue/ConversationCatalogSO")]
public class ConversationCatalogSO : ScriptableObject
{
    [SerializeField] private List<DialogueConversationSO> conversations;

    public List<DialogueConversationSO> Conversations { get => conversations; }

}
