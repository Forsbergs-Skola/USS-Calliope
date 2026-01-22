public interface IAttackDecisionSystem
{
    EnemyAttackInstance ChooseAttack(
        EnemyAttackInstance ranged,
        EnemyAttackInstance melee,
        EnemyAttackInstance leap,
        EnemyAttackContext context
    );
}