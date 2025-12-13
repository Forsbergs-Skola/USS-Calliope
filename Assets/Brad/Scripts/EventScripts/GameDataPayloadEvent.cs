using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "GameDataPayloadEvent", menuName = "Event Channels/GameDataPayloadEvent")]
    public class GameDataPayloadEvent : ScriptableObject
    {
        public event System.Action<GameData> OnEventTriggered;
        public void TriggerEvent(GameData payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}


