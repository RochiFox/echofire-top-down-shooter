public class IdleStateMelee : EnemyState
{
    private EnemyMelee enemy;

    public IdleStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        StateTimer = EnemyBase.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAggressionRange())
        {
            StateMachine.ChangeState(enemy.RecoveryState);
            return;
        }

        if (StateTimer < 0)
            StateMachine.ChangeState(enemy.MoveState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}