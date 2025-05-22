public class EnemyRange : Enemy
{
    public IdleStateRange IdleState { get; private set; }
    public MoveStateRange MoveState { get; private set; }
    public BattleStateRange BattleState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateRange(this, StateMachine, "Idle");
        MoveState = new MoveStateRange(this, StateMachine, "Move");
        BattleState = new BattleStateRange(this, StateMachine, "Battle");
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

    protected override void EnterBattleMode()
    {
        if (InBattleMode)
            return;

        base.EnterBattleMode();

        StateMachine.ChangeState(BattleState);
    }
}