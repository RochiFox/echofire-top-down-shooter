public class BattleStateRange : EnemyState
{
    private readonly EnemyRange enemy;

    public BattleStateRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.Player.position);
    }

    public override void Exit()
    {
        base.Exit();
    }
}