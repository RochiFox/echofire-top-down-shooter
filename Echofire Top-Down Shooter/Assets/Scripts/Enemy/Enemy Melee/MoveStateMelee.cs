using UnityEngine;

public class MoveStateMelee : EnemyState
{
    private readonly EnemyMelee enemy;
    private Vector3 destination;

    public MoveStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = EnemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.Agent.speed = enemy.moveSpeed;

        destination = enemy.GetPatrolDestination();
        enemy.Agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.Agent.steeringTarget);

        if (enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance + 0.05f)
            StateMachine.ChangeState(enemy.IdleState);
    }
}