using System;

public static class AlarmEvents
{
    public static event Action<PatrolZone> OnAlarmTriggered;
    public static event Action<PatrolZone> OnAlarmCleared;

    public static void TriggerAlarm(PatrolZone zone) => OnAlarmTriggered?.Invoke(zone);
    public static void ClearAlarm(PatrolZone zone) => OnAlarmCleared?.Invoke(zone);
}