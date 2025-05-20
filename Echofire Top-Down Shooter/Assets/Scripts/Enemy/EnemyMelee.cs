using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)] public float animationSpeed;
    public AttackTypeMelee attackType;
}

public enum AttackTypeMelee
{
    Close,
    Charge
}

public enum EnemyMeleeType
{
    Regular,
    Shield
}

public class EnemyMelee : Enemy
{
    private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");

    public IdleStateMelee IdleState { get; private set; }
    public MoveStateMelee MoveState { get; private set; }
    public RecoveryStateMelee RecoveryState { get; private set; }
    public ChaseStateMelee ChaseState { get; private set; }
    public AttackStateMelee AttackState { get; private set; }
    public DeadStateMelee DeadState { get; private set; }

    [Header("Enemy Settings")] public EnemyMeleeType meleeType;
    public Transform shieldTransform;

    [Header("Attack Data")] public AttackData attackData;
    public List<AttackData> attackList;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateMelee(this, StateMachine, "Idle");
        MoveState = new MoveStateMelee(this, StateMachine, "Move");
        RecoveryState = new RecoveryStateMelee(this, StateMachine, "Recovery");
        ChaseState = new ChaseStateMelee(this, StateMachine, "Chase");
        AttackState = new AttackStateMelee(this, StateMachine, "Attack");
        DeadState = new DeadStateMelee(this, StateMachine, "Idle"); // Idle anim is just a placeholder (we use ragdoll)
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.Initialize(IdleState);

        InitializeSpeciality();
    }

    protected override void Update()
    {
        base.Update();

        StateMachine.CurrentState.Update();
    }

    private void InitializeSpeciality()
    {
        if (meleeType != EnemyMeleeType.Shield) return;

        Anim.SetFloat(ChaseIndex, 1);
        shieldTransform.gameObject.SetActive(true);
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
            StateMachine.ChangeState(DeadState);
    }

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.position) < attackData.attackRange;
}