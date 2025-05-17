using UnityEngine;

public class RecoveryStateMelee : EnemyState
{
    private EnemyMelee enemy;

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

        enemy.transform.rotation = enemy.FaceTarget(enemy.Player.position);

        if (TriggerCalled)
            StateMachine.ChangeState(enemy.ChaseState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}