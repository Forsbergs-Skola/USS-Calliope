using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private string playerTag = "Player";
    
    //TODO: Delete this field and use the method SetAttacks!!
    [Header("Attacks")]
    [SerializeField] private List<EnemyAttackSOClass> availableAttacks;
    
    private List<EnemyAttackSOClass> currentAttacks = new();
    private EnemyAttackContext context;

    private void Awake()
    {
        context = new EnemyAttackContext
        {
            enemy = transform,
            movement = GetComponent<EnemyFollowPlayer>()
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
        currentAttacks = attacks;
    }

    private void Update()
    {
        if (context.player == null) return;
        Debug.Log($"[EnemyAttackController] Executing Update");
        //foreach (var attack in currentAttacks)
        foreach (var attack in availableAttacks)
        {
            Debug.Log($"[EnemyAttackController] foreach");
            if (attack.IsOnCooldown())
                continue;
            
            Debug.Log($"[EnemyAttackController] before canExecute");
            if (attack.CanExecute(context))
            {
                Debug.Log($"Executing attack: {attack.name}");
                attack.Execute(context);
                attack.MarkUsed();
                break; // One attack per frame
            }
        }
    }
}