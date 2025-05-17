using UnityEngine;

public class MoveStateMelee : EnemyState
{
    public MoveStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        Debug.Log("I'm moving around");
    }

    public override void Exit()
    {
        base.Exit();
    }
}