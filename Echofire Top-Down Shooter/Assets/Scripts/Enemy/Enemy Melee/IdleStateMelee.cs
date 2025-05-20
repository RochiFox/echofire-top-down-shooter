public class IdleStateMelee : EnemyState
{
    private readonly EnemyMelee enemy;

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

        if (StateTimer < 0)
            StateMachine.ChangeState(enemy.MoveState);
    }
}