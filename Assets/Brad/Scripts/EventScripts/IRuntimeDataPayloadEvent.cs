using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "IRuntimeDataPayloadEvent", menuName = "Event Channels/IRuntimeDataPayloadEvent")]
    public class IRuntimeDataPayloadEvent : ScriptableObject
    {
        public event System.Action<IRuntimeData> OnEventTriggered;
        public void TriggerEvent(IRuntimeData payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}


