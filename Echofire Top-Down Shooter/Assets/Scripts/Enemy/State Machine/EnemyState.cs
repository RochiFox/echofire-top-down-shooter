using UnityEngine;

public class EnemyState
{
    protected readonly Enemy EnemyBase;
    protected readonly EnemyStateMachine StateMachine;

    private readonly string animBoolName;
    protected float StateTimer;

    protected bool TriggerCalled;

    protected EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.EnemyBase = enemyBase;
        this.StateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        EnemyBase.Anim.SetBool(animBoolName, true);

        TriggerCalled = false;
    }

    public virtual void Update()
    {
        StateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        EnemyBase.Anim.SetBool(animBoolName, false);
    }

    public virtual void AbilityTrigger()
    {
    }

    public void AnimationTrigger() => TriggerCalled = true;
}