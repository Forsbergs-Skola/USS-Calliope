using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    public string playerTag = "Player";
    public int keyId = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var keycards = other.GetComponent<PlayerKeycards>();
        if (keycards == null) return;

        keycards.GiveKeycard(keyId);
        Destroy(gameObject);
    }
}