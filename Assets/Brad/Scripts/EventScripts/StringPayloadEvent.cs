using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "StringPayloadEvent", menuName = "Event Channels/StringPayloadEvent")]
    public class StringPayloadEvent : ScriptableObject
    {
        public event System.Action<string> OnEventTriggered;
        public void TriggerEvent(string payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}

