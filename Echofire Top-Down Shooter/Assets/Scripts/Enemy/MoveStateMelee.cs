using UnityEngine;

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
    }

    public override void Update()
    {
        base.Update();

        enemy.Agent.SetDestination(destination);

        if (enemy.Agent.remainingDistance <= 1)
            StateMachine.ChangeState(enemy.IdleState);
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("Exit move state");
    }
}