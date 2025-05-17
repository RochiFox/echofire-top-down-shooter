public class EnemyMelee : Enemy
{
    public IdleStateMelee IdleState { get; private set; }
    public MoveStateMelee MoveState { get; private set; }
    public RecoveryStateMelee RecoveryState { get; private set; }
    public ChaseStateMelee ChaseState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateMelee(this, StateMachine, "Idle");
        MoveState = new MoveStateMelee(this, StateMachine, "Move");
        RecoveryState = new RecoveryStateMelee(this, StateMachine, "Recovery");
        ChaseState = new ChaseStateMelee(this, StateMachine, "Chase");
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