public class EnemyRange : Enemy
{
    public IdleStateRange IdleState { get; private set; }
    public MoveStateRange MoveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateRange(this, StateMachine, "Idle");
        MoveState = new MoveStateRange(this, StateMachine, "Move");
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.Update();
    }
}