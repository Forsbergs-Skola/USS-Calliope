using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/Infection Phase Data")]
public class SO_InfectionPhaseData : ScriptableObject
{
    [Header("Phase Info")]
    public string phaseName;
    [Range(0,100)] public float minInfection;
    [Range(0,100)] public float maxInfection;

    [Header("Stats")]
    public float moveSpeedMultiplier = 1f;
    public float perceptionMultiplier = 1f;
    public float attackCooldown = 1.5f;

    [Header("Attacks Available In This Phase")]
    public List<EnemyAttackSOClass> availableAttacks;
    
    [Header("Audio")]
    public AudioClip idleClip;
    public AudioClip alertClip;
}