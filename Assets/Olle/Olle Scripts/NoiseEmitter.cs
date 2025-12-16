using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float walkRadius   = 5f;
    public float runRadius    = 9f;
    public float crouchRadius = 2f;

    public LayerMask enemyMask;
    public ParticleSystem ringParticles;

    public void EmitWalk()
    {
        EmitRing(walkRadius);
    }

    public void EmitRun()
    {
        EmitRing(runRadius);
    }

    public void EmitCrouch()
    {
        EmitRing(crouchRadius);
    }

    void EmitRing(float radius)
    {
        // visual ring
        if (ringParticles != null)
        {
            var main = ringParticles.main;
            main.startSize = radius * 2f; // diameter
            ringParticles.Emit(1);
        }

        // Alerts enemies inside radius
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyMask);
        foreach (var hit in hits)
        {
            EnemyChase enemy = hit.GetComponent<EnemyChase>();
            if (enemy != null)
            {
                enemy.HeardPlayer(transform.position);
            }
        }
    }
}