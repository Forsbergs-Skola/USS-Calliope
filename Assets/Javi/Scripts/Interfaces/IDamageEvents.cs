using UnityEngine.Events;
using System;

public interface IDamageEvents
{
    event Action<float> OnDamaged;
}