using UnityEngine;
using System.Collections.Generic;
public interface IPatrolZone
{
    string ZoneID { get; }
    int RoomID { get; }
    //List<Transform> PatrolPoints { get; }
    bool IsActive { get; set; }
}