using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AlarmSystem", menuName = "NPCs/AlarmSystem")]
public class AlarmSystem : ScriptableObject
{
    [System.Serializable]
    public class AlarmEvent : UnityEvent<PatrolZone> { }
    
    public AlarmEvent onAlarmTriggered = new AlarmEvent();
    public AlarmEvent onAlarmCleared = new AlarmEvent();
    
    public void TriggerAlarm(PatrolZone zone)
    {
        //zone.IsActive = true;
        onAlarmTriggered?.Invoke(zone);
        AlarmEvents.TriggerAlarm(zone);
    }
    
    public void ClearAlarm(PatrolZone zone)
    {
        //zone.IsActive = false;
        onAlarmCleared?.Invoke(zone);
        AlarmEvents.ClearAlarm(zone);
    }
}