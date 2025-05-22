using UnityEngine;

public class BattleStateRange : EnemyState
{
    private readonly EnemyRange enemy;
    private float lastTimeShoot = -10;

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

        if (Time.time > lastTimeShoot + 1 / enemy.fireRate)
        {
            enemy.FireSingleBullet();
            lastTimeShoot = Time.time;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}