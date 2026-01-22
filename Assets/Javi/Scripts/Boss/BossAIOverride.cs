using UnityEngine;

public class BossAIOverride : MonoBehaviour
{
    private EnemyAIStateController ai;

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
    }

    private void Start()
    {
        // boss doesn't patrol like other enemies
        ai.ForceBossState();
    }
}