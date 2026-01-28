using System.Collections;
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
    
    private bool isExecutingExclusiveAttack;
    private bool shouldChaseTarget;

    private void Awake()
    {
        /*var firePoint = transform.root.Find("FirePoint");
        context = new EnemyAttackContext
        {
            enemy = transform.root,
            movement = GetComponent<EnemyFollowPlayer>(),
            coroutineRunner = this,
            firePoint = firePoint,
        };
        
        if (context.firePoint == null)
        {
            Debug.Log($"[{name}] FirePoint not found!");
        }*/
        
        ai = GetComponent<EnemyAIStateController>();
    }
    
    private void Start()
    {
        var firePoint = transform.root.Find("FirePoint");
        context = new EnemyAttackContext
        {
            enemy = transform.root,
            movement = GetComponent<EnemyFollowPlayer>(),
            coroutineRunner = this,
            firePoint = firePoint/*transform.root.Find("FirePoint")*/,
        };
        
        if (context.firePoint == null)
        {
            Debug.Log($"[{name}] FirePoint not found!");
        }
        
        FindAndSetPlayer();
    }
    
    private void Update()
    {
        if (recheckTimer > 0f)
        {
            recheckTimer -= Time.deltaTime;
            return;
        }
        
        if (context.player == null)
            return;

        if (currentAttacks == null || currentAttacks.Count == 0)
            return;
        
        if (ai.CurrentState != EnemyAIStateController.State.Attacking)
        {
            activeAttack = null;
            return;
        }
        
        if(ai.CurrentState == EnemyAIStateController.State.Attacking)
        {
            var perception = context.enemy.GetComponent<IPerceptionSystem>();
            if (perception != null && !perception.CanSeeTarget(context.player))
            {
                activeAttack = null;
                ai.OnLostPlayer();
                return;
            }
        }
        
        var leapRuntime = context.enemy.GetComponent<EnemyLeapRuntime>();
        if (leapRuntime != null && leapRuntime.IsLeapActive)
            return;

        if (activeAttack == null)
        {
            //Debug.Log($"{name} ATTACKING but no activeAttack");
            TrySelectAttack();
            
            if (activeAttack == null &&
                context.enemy.GetComponent<FinalBossBrain>() != null)
            {
                RotateTowardsPlayer();
            }

            return;
        }

        //
        if (!activeAttack.attack.CanExecute(context))
        {
            activeAttack = null;
            //ai.OnLostPlayer(); // go to watchful
            recheckTimer = attackRecheckInterval;
            //HandleMovementForAttack(activeAttack);
            return;
        }
        
        context.movement.SetTarget(context.player);
        context.movement.SetFollow(true);

        if (activeAttack.IsOnCooldown()) return;

        HandleMovementForAttack(activeAttack);
        
        if (activeAttack.attack is SO_RangedEnemyAttack ||
           activeAttack.attack is SO_BossFastRangedAttack)
        {
            context.movement.SetFollow(false);
            context.movement.SetTarget(null);
            RotateTowardsPlayer();
        }
        
        if (activeAttack.attack is SO_BossFuryLeapAttack)
        {
            var fury = context.enemy.GetComponent<BossFuryCounter>();
            if (!fury.ConsumeLeap())
            {
                activeAttack = null;
                return;
            }
        }
        
        activeAttack.attack.Execute(context);
        activeAttack.MarkUsed();
        
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
        
        /*// If active attack has not yet been chosen
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
        activeAttack.MarkUsed();*/
        
        /*if (isExecutingExclusiveAttack) return;
        
        bool hasRanged = HasAttack<SO_RangedEnemyAttack>() || HasAttack<SO_BossFastRangedAttack>();
        bool hasMelee = HasAttack<SO_MeleeAttack>() || HasAttack<SO_BossHeavyMeleeAttack>();
        bool hasLeap = HasAttack<SO_LeapAttack>() || HasAttack<SO_BossFuryLeapAttack>();
        
        // Enemy only has  Ranged
        if (hasRanged && !hasMelee && !hasLeap)
        {
            ExecuteRanged();
            return;
        }
        
        // Enemy has Ranged and Melee
        if (hasRanged && hasMelee && !hasLeap)
        {
            if (IsTooCloseForRanged())
                ExecuteMelee();
            else
                ExecuteRanged();
            return;
        }
        
        // Enemy has Melee and Leap
        if (!hasRanged && hasMelee && hasLeap)
        {
            if (CanExecuteLeap())
                ExecuteLeapExclusive();
            else
                ExecuteMelee();
            return;
        }
        
        
        if (hasRanged && hasMelee && hasLeap)
        {
            float roll = Random.value;

            if (roll < 0.2f && CanExecuteLeap())
            {
                ExecuteLeapExclusive();
                return;
            }

            if (IsTooCloseForRanged())
                ExecuteMelee();
            else
                ExecuteRanged();
        }
        
        // enemy has Ranged, Melee and Leap
        if (hasRanged && hasMelee && hasLeap)
        {
            float roll = Random.value;

            if (roll < 0.2f && CanExecuteLeap())
            {
                ExecuteLeapExclusive();
                return;
            }

            if (IsTooCloseForRanged())
                ExecuteMelee();
            else
                ExecuteRanged();
        }*/
    }
    
    private float GetRangedMax(EnemyAttackInstance attack)
    {
        if (attack.attack is SO_RangedEnemyAttack r)
            return r.maxRange;

        if (attack.attack is SO_BossFastRangedAttack b && b.baseRanged != null)
            return b.baseRanged.maxRange;

        return 0f;
    }
    
    private void HandleMovementForAttack(EnemyAttackInstance attack)
    {
        var follow = context.movement;

        if (attack.attack is SO_RangedEnemyAttack ||
            attack.attack is SO_BossFastRangedAttack)
        {
            // stay in place and rotate
            /*follow.SetFollow(false);
            RotateTowardsPlayer();
            return;*/
            float dist = Vector3.Distance(
                context.enemy.position,
                context.player.position
            );

            float maxRange = GetRangedMax(attack);
            //Debug.Log($"[HnadleMovement] dist: {dist}, maxrange: {maxRange}");
            if (dist > maxRange)
            {
                // go to the player
                follow.SetTarget(context.player);
                follow.SetFollow(true);
            }
            else
            {
                // already close 
                follow.SetFollow(false);
                RotateTowardsPlayer();
            }

            return;
        }

        if (attack.attack is SO_MeleeAttack ||
            attack.attack is SO_BossHeavyMeleeAttack)
        {
            follow.SetTarget(context.player);
            follow.SetFollow(true);
            return;
        }

        if (attack.attack is SO_LeapAttack ||
            attack.attack is SO_BossFuryLeapAttack)
        {
            follow.SetFollow(false);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 dir = context.player.position - context.enemy.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        context.enemy.rotation = Quaternion.RotateTowards(
            context.enemy.rotation,
            rot,
            360f * Time.deltaTime
        );
    }

    private void TrySelectAttack()
    {
        float dist = Vector3.Distance(context.enemy.position, context.player.position);

        var ranged = GetAttack<SO_RangedEnemyAttack>() ?? GetAttack<SO_BossFastRangedAttack>();
        var melee = GetAttack<SO_MeleeAttack>() ?? GetAttack<SO_BossHeavyMeleeAttack>();
        var leap  = GetAttack<SO_LeapAttack>() ?? GetAttack<SO_BossFuryLeapAttack>();
        
        var decisionSystem = context.enemy.GetComponent<IAttackDecisionSystem>();
        if (decisionSystem != null)
        {
            activeAttack = decisionSystem.ChooseAttack(ranged, melee, leap, context);
            
            return;
        }

        // security
        if (ranged != null && !ranged.attack.CanExecute(context))
            ranged = null;

        if (leap != null && !leap.attack.CanExecute(context))
            leap = null;
        
        // Leap + Melee
        if (leap != null && melee != null)
        {
            float leapMin = GetLeapMinDistance(leap);
            activeAttack = dist >= leapMin ? leap : melee;
            return;
        }

        // Ranged + Melee
        if (ranged != null && melee != null)
        {
            activeAttack = IsTooCloseForRanged() ? melee : ranged;
            return;
        }

        // Ranged + Leap
        if (ranged != null && leap != null)
        {
            activeAttack = Random.value < 0.8f ? ranged : leap;
            return;
        }

        // only one
        activeAttack = ranged ?? melee ?? leap;
        //Debug.Log($"jrv activeAttack {activeAttack}");
    }

    private float GetLeapMinDistance(EnemyAttackInstance leap)
    {
        if (leap.attack is SO_LeapAttack l)
            return l.minExecuteDistance;

        if (leap.attack is SO_BossFuryLeapAttack b)
            return b.minDistance;

        return 0f;
    }

    
    private void FindAndSetPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            SetPlayer(playerObject.transform);
            //Debug.Log($"[EnemyAttackController] Player found: {context.player != null}");
        }
        else
        {
            //Debug.LogError($"[EnemyAttackController] No GameObject with tag '{playerTag}' found!");
        }
    }

    public void SetPlayer(Transform player)
    {
        context.player = player;
        //Debug.Log($"[EnemyAttackController] Player set: {player.name}");
    }

    public void SetInfection(float infection)
    {
        context.infectionPercentage = infection;
    }
    
    /*private void TrySelectAttack()
    {
        var validAttacks = new List<EnemyAttackInstance>();

        foreach (var atk in currentAttacks)
        {
            if (atk.attack != null && atk.attack.CanExecute(context))
                validAttacks.Add(atk);
        }

        if (validAttacks.Count == 0) return;

        activeAttack = ChooseByProbability(validAttacks);
    }*/
    
    public void SetAttacks(List<EnemyAttackSOClass> attacks)
    {
        currentAttacks.Clear();
        foreach (var attack in attacks)
        {
            currentAttacks.Add(new EnemyAttackInstance { attack = attack });
        }
        //Debug.Log($"[EnemyAttackController] Attack instances created: {currentAttacks.Count}");
    }
    
    /*private EnemyAttackInstance ChooseByProbability(List<EnemyAttackInstance> attacks)
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
    }*/
    
    private bool HasAttack<T>() where T : EnemyAttackSOClass
    {
        return currentAttacks.Exists(a => a.attack is T);
    }

    private EnemyAttackInstance GetAttack<T>() where T : EnemyAttackSOClass
    {
        return currentAttacks.Find(a => a.attack is T);
    }
    private void ExecuteRanged()
    {
        EnemyAttackInstance ranged =
            GetAttack<SO_RangedEnemyAttack>() ??
            GetAttack<SO_BossFastRangedAttack>();

        if (ranged == null || ranged.IsOnCooldown()) return;

        context.movement?.SetFollow(false);

        Vector3 lookDir = context.player.position - context.enemy.position;
        lookDir.y = 0;
        context.enemy.rotation = Quaternion.LookRotation(lookDir);

        ranged.attack.Execute(context);
        ranged.MarkUsed();
    }

    private bool IsTooCloseForRanged()
    {
        float dist = Vector3.Distance(
            context.enemy.position,
            context.player.position
        );

        // Normal ranged
        var rangedInstance = GetAttack<SO_RangedEnemyAttack>();
        if (rangedInstance != null)
        {
            var rangedSO = rangedInstance.attack as SO_RangedEnemyAttack;
            if (rangedSO != null)
                return dist < rangedSO.minRange;
        }

        // Boss fast ranged
        var bossRangedInstance = GetAttack<SO_BossFastRangedAttack>();
        if (bossRangedInstance != null)
        {
            var bossSO = bossRangedInstance.attack as SO_BossFastRangedAttack;
            if (bossSO != null && bossSO.baseRanged != null)
                return dist < bossSO.baseRanged.minRange;
        }

        return false;
    }
    
    
    private void ExecuteMelee()
    {
        EnemyAttackInstance melee =
            GetAttack<SO_MeleeAttack>() ??
            GetAttack<SO_BossHeavyMeleeAttack>();

        if (melee == null || melee.IsOnCooldown()) return;

        context.movement?.SetTarget(context.player);
        context.movement?.SetFollow(true);

        melee.attack.Execute(context);
        melee.MarkUsed();
    }
    
    private bool CanExecuteLeap()
    {
        EnemyAttackInstance leap =
            GetAttack<SO_LeapAttack>() ??
            GetAttack<SO_BossFuryLeapAttack>();

        if (leap == null || leap.IsOnCooldown()) return false;
        return leap.attack.CanExecute(context);
    }

    private void ExecuteLeapExclusive()
    {
        EnemyAttackInstance leap =
            GetAttack<SO_LeapAttack>() ??
            GetAttack<SO_BossFuryLeapAttack>();

        if (leap == null) return;

        isExecutingExclusiveAttack = true;

        leap.attack.Execute(context);
        leap.MarkUsed();

        StartCoroutine(WaitForLeapEnd());
    }
    
    private IEnumerator WaitForLeapEnd()
    {
        var runtime = context.enemy.GetComponent<EnemyLeapRuntime>();

        while (runtime != null && runtime.IsLeapActive)
            yield return null;

        isExecutingExclusiveAttack = false;

        ai.OnStunnedEndAfterLeapAttack();

        /*var patrol = context.enemy.GetComponent<EnemyPatrolController>();
        if (patrol != null)
            patrol.WatchInPlace(4f);*/
    }
    
    public bool HasActiveAttack(out int attackType)
    {
        attackType = 0;

        if (activeAttack == null || activeAttack.attack == null)
            return false;

        if (activeAttack.attack is SO_MeleeAttack ||
            activeAttack.attack is SO_BossHeavyMeleeAttack)
            attackType = 1;
        else if (activeAttack.attack is SO_RangedEnemyAttack ||
                 activeAttack.attack is SO_BossFastRangedAttack)
            attackType = 2;
        else if (activeAttack.attack is SO_LeapAttack ||
                 activeAttack.attack is SO_BossFuryLeapAttack)
            attackType = 3;
        
        //Debug.Log($"jrv {name} attackType {attackType}");
        return attackType != 0;
    }

    public void deactivateAttacks()
    {
        activeAttack = null;
    }
}