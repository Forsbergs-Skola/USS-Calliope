using UnityEngine;
using Olle.Scripts;

public class BossPerception : MonoBehaviour
{
    private Transform player;
    private CrouchInvisibility invis;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        invis = player.GetComponent<CrouchInvisibility>();
    }

    public bool CanSeePlayer()
    {
        return invis == null || !invis.IsInvisible;
    }

    public Vector3 GetLastKnownZone()
    {
        return player.position;
    }
}