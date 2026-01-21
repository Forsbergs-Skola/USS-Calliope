using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private string playerTag = "Player";
    
    [SerializeField]  private List<EnemyAttackInstance> currentAttacks = new();
    private EnemyAttackContext context;
    private EnemyAIStateController ai;
    
    private EnemyAttackInstance activeAttack;
    private float recheckTimer;
    [SerializeField] private float attackRecheckInterval = 0.5f;

    private void Awake()
    {
        context = new EnemyAttackContext
        {
            enemy = transform.root,
            movement = GetComponent<EnemyFollowPlayer>(),
            coroutineRunner = this,
            firePoint = transform.root.Find("FirePoint"),
        };
        
        if (context.firePoint == null)
        {
            Debug.LogError($"[{name}] FirePoint not found!");
        }
        
        ai = GetComponent<EnemyAIStateController>();
    }
    
    private void Start()
    {
        FindAndSetPlayer();
    }
    
    private void Update()
    {
        if (context.player == null)
            return;

        if (currentAttacks == null || currentAttacks.Count == 0)
            return;
        
        if(ai.CurrentState != EnemyAIStateController.State.Attacking)
            return;
        
        /*if (activeAttack != null &&
            activeAttack.attack is SO_LeapAttack &&
            !context.enemy.GetComponent<EnemyLeapRuntime>().IsLeapActive)
        {
            activeAttack = null;
        }*/

        /*foreach (var attackInstance in currentAttacks)
        {
            if (attackInstance == null || attackInstance.attack == null)
                continue;

            if (attackInstance.IsOnCooldown())
                continue;

            if (attackInstance.attack.CanExecute(context))
            {
                Debug.Log($"{name} Executing attack: {attackInstance.attack.name}");
                attackInstance.attack.Execute(context);
                attackInstance.MarkUsed();
                break; // Only one attack per frame
            }
        }*/
        
        // If active attack has not yet been chosen
        if (activeAttack == null)
        {
            TrySelectAttack();
            return;
        }
        
        // if the active attack is no longer valid
        if (!activeAttack.attack.CanExecute(context))
        {
            activeAttack = null;
            return;
        }
        
        // Cooldown
        if (activeAttack.IsOnCooldown()) return;
        
        activeAttack.attack.Execute(context);
        activeAttack.MarkUsed();
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
    
    private void TrySelectAttack()
    {
        var validAttacks = new List<EnemyAttackInstance>();

        foreach (var atk in currentAttacks)
        {
            if (atk.attack != null && atk.attack.CanExecute(context))
                validAttacks.Add(atk);
        }

        if (validAttacks.Count == 0) return;

        activeAttack = ChooseByProbability(validAttacks);
    }
    
    private EnemyAttackInstance ChooseByProbability(List<EnemyAttackInstance> attacks)
    {
        bool hasMelee = attacks.Exists(a => a.attack is SO_MeleeAttack);
        bool hasRanged = attacks.Exists(a => a.attack is SO_RangedEnemyAttack);
        bool hasLeap = attacks.Exists(a => a.attack is SO_LeapAttack);

        float roll = Random.value;

        if (attacks.Count == 1)
            return attacks[0];

        if (hasRanged && hasMelee && hasLeap)
        {
            if (roll < 0.75f) return attacks.Find(a => a.attack is SO_RangedEnemyAttack);
            if (roll < 0.875f) return attacks.Find(a => a.attack is SO_MeleeAttack);
            return attacks.Find(a => a.attack is SO_LeapAttack);
        }

        if (hasRanged && hasMelee)
            return roll < 0.8f
                ? attacks.Find(a => a.attack is SO_RangedEnemyAttack)
                : attacks.Find(a => a.attack is SO_MeleeAttack);

        if (hasRanged && hasLeap)
            return roll < 0.8f
                ? attacks.Find(a => a.attack is SO_RangedEnemyAttack)
                : attacks.Find(a => a.attack is SO_LeapAttack);

        // Melee + Leap
        return roll < 0.5f
            ? attacks.Find(a => a.attack is SO_MeleeAttack)
            : attacks.Find(a => a.attack is SO_LeapAttack);
    }
    
    
}