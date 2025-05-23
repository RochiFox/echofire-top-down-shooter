public class RecoveryStateMelee : EnemyState
{
    private readonly EnemyMelee enemy;

    public RecoveryStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Agent.isStopped = true;
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Enemy.PlayerTransform.position);

        if (TriggerCalled)
        {
            if (enemy.CanThrowAxe())
                StateMachine.ChangeState(enemy.AbilityState);
            else if (enemy.PlayerInAttackRange())
                StateMachine.ChangeState(enemy.AttackState);
            else
                StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}