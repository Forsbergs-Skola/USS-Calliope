using UnityEngine;
using System.Collections.Generic;

public class PlayerKeycards : MonoBehaviour
{
    [System.Serializable]
    public class KeycardData
    {
        public int keyId;
        public bool hasKeycard = false;
    }

    public List<KeycardData> keycards = new List<KeycardData>();

    public void GiveKeycard(int keyId)
    {
        KeycardData data = keycards.Find(k => k.keyId == keyId);
        if (data == null)
        {
            data = new KeycardData { keyId = keyId };
            keycards.Add(data);
        }
        data.hasKeycard = true;
    }

    public bool HasKeycard(int keyId)
    {
        KeycardData data = keycards.Find(k => k.keyId == keyId);
        return data != null && data.hasKeycard;
    }

    public bool TryUseKeycardForDoor(SlidingDoor door)
    {
        if (door.lockType != DoorLockType.Keycard || !door.isLocked) return false;

        KeycardData data = keycards.Find(k => k.keyId == door.keyId);
        if (data != null && data.hasKeycard)
        {
            return door.TryUnlockWithKeycard(door.keyId);
        }
        return false;
    }
}