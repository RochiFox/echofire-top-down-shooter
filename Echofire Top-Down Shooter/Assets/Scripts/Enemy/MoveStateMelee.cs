using UnityEngine;
using UnityEngine.AI;

public class MoveStateMelee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 destination;

    public MoveStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = EnemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        destination = enemy.GetPatrolDestination();
        enemy.Agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAggressionRange())
        {
            StateMachine.ChangeState(enemy.RecoveryState);
            return;
        }

        enemy.transform.rotation = enemy.FaceTarget(enemy.Agent.steeringTarget);

        if (enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance + 0.05f)
            StateMachine.ChangeState(enemy.IdleState);
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("Exit move state");
    }
}