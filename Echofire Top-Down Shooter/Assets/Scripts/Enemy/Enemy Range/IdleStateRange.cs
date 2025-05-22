public class IdleStateRange : EnemyState
{
    private readonly EnemyRange enemy;

    public IdleStateRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = EnemyBase as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();

        StateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (StateTimer < 0)
            StateMachine.ChangeState(enemy.MoveState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}