using UnityEngine;

public class EnemyState
{
    protected Enemy EnemyBase;
    protected EnemyStateMachine StateMachine;

    protected string AnimBoolName;
    protected float StateTimer;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.EnemyBase = enemyBase;
        this.StateMachine = stateMachine;
        this.AnimBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        EnemyBase.Anim.SetBool(AnimBoolName, true);
    }

    public virtual void Update()
    {
        StateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        EnemyBase.Anim.SetBool(AnimBoolName, false);
    }
}