using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private string playerTag = "Player";
    
    [SerializeField]  private List<EnemyAttackInstance> currentAttacks = new();
    private EnemyAttackContext context;

    private void Awake()
    {
        context = new EnemyAttackContext
        {
            enemy = transform.root,
            movement = GetComponent<EnemyFollowPlayer>(),
            coroutineRunner = this
        };
    }
    
    private void Start()
    {
        FindAndSetPlayer();
    }
    
    private void FindAndSetPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
            Debug.Log($"[EnemyAttackController] Player found: {context.player != null}");
        }
        else
        {
            Debug.LogError($"[EnemyAttackController] No GameObject with tag '{playerTag}' found!");
        }
    }

    public void SetPlayer(Transform player)
    {
        context.player = player;
        Debug.Log($"[EnemyAttackController] Player set: {player.name}");
    }

    public void SetInfection(float infection)
    {
        context.infectionPercentage = infection;
    }

    public void SetAttacks(List<EnemyAttackSOClass> attacks)
    {
        currentAttacks.Clear();
        foreach (var attack in attacks)
        {
            currentAttacks.Add(new EnemyAttackInstance
            {
                attack = attack
            });
        }
        Debug.Log($"[EnemyAttackController] Attack instances created: {currentAttacks.Count}");
    }

    private void Update()
    {
        if (context.player == null)
            return;

        if (currentAttacks == null || currentAttacks.Count == 0)
            return;

        foreach (var attackInstance in currentAttacks)
        {
            if (attackInstance == null || attackInstance.attack == null)
                continue;

            if (attackInstance.IsOnCooldown())
                continue;

            if (attackInstance.attack.CanExecute(context))
            {
                Debug.Log($"Executing attack: {attackInstance.attack.name}");
                attackInstance.attack.Execute(context);
                attackInstance.MarkUsed();
                break; // Only one attack per frame
            }
        }
    }

}