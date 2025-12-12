using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "EmptyPayloadEvent", menuName = "Event Channels/EmptyPayloadEvent")]
    public class EmptyPayloadEvent : ScriptableObject
    {
        public event System.Action OnEventTriggered;
        public void TriggerEvent()
        {
            OnEventTriggered?.Invoke();
        }
    }
}


