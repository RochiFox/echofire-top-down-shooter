using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyState IdleState { get; private set; }
    public EnemyState MoveState { get; private set; }

    private void Start()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyState(this, StateMachine, "Idle");
        MoveState = new EnemyState(this, StateMachine, "Move");

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();

        if (Input.GetKeyDown(KeyCode.V))
            StateMachine.ChangeState(IdleState);

        if (Input.GetKeyDown(KeyCode.C))
            StateMachine.ChangeState(MoveState);
    }
}