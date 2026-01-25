using System;
using System.Collections;
using UnityEngine;

public class DestroyEnemyOnAnimEnd : MonoBehaviour
{
    private Animator anim;
    private EnemyAIStateController ai;
    private EnemyAttackController enemyController;
    private SimpleMovementAgent movement;
    [SerializeField] private float targetY = 0f;
    //private bool alreadyHandled = false;
    
    void Awake()
    {
        //anim = GetComponent<Animator>();
        anim = GetComponentInChildren<Animator>();
        ai = GetComponent<EnemyAIStateController>();
        enemyController = GetComponent<EnemyAttackController>();
        movement =  GetComponent<SimpleMovementAgent>();
    }

    void Update()
    {
        if (ai == null || !ai.IsDead) return;
        /*
        // check animator
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"jrv {name} is dead, state {state}");
        // Death
        if (state.IsName("Death") && state.normalizedTime >= 1f)
        {
            Debug.Log($"jrv {name} is dead and destroyed");
            Destroy(gameObject);
        }
        */
        
        ai.enabled = false;
        enemyController.enabled = false;
        movement.enabled = false;
        anim.SetFloat("MoveSpeed", 0f);
        if (transform.position.y > targetY )
        {
            StartCoroutine(DeadlyRoutine());
        }
            
        
    }
    
    private IEnumerator DeadlyRoutine()
    {
        yield return new WaitForSeconds(3.3f);
        dying();
    }
    
    private void dying()
    {
        /*if (alreadyHandled)
            return;
        alreadyHandled = true;*/
        
        Vector3 pos = transform.position;
        pos.y = targetY;
        transform.position = pos;
        //Destroy(gameObject);
    }
}
