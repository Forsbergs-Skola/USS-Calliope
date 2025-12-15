using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "FloatPayloadEvent", menuName = "Event Channels/FloatPayloadEvent")]
    public class FloatPayloadEvent : ScriptableObject
    {
        public event System.Action<float> OnEventTriggered;
        public void TriggerEvent(float payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}


