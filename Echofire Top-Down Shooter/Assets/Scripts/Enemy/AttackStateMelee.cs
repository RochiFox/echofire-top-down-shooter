using UnityEngine;

public class AttackStateMelee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 attackDirection;

    private const float MAX_ATTACK_DISTANCE = 50f;

    public AttackStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Agent.isStopped = true;
        enemy.Agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection,
                enemy.attackMoveSpeed * Time.deltaTime);
        }

        if (TriggerCalled)
            StateMachine.ChangeState(enemy.RecoveryState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}