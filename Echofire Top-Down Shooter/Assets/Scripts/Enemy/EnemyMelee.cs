public class EnemyMelee : Enemy
{
    public IdleStateMelee IdleState { get; private set; }
    public MoveStateMelee MoveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateMelee(this, StateMachine, "Idle");
        MoveState = new MoveStateMelee(this, StateMachine, "Move");
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