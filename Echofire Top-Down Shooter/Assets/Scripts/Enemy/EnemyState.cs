using UnityEngine;

public class EnemyState
{
    protected Enemy EnemyBase;
    protected EnemyStateMachine StateMachine;

    protected string AnimBoolName;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.EnemyBase = enemyBase;
        this.StateMachine = stateMachine;
        this.AnimBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        Debug.Log("I enter " + AnimBoolName + " state");
    }

    public virtual void Update()
    {
        Debug.Log("I'm running " + AnimBoolName + " state");
    }

    public virtual void Exit()
    {
        Debug.Log("I'm exit " + AnimBoolName + " state");
    }
}