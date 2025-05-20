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
    Shield,
    Dodge,
    AxeThrow
}

public class EnemyMelee : Enemy
{
    private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
    private static readonly int Dodge = Animator.StringToHash("Dodge");

    #region States

    public IdleStateMelee IdleState { get; private set; }
    public MoveStateMelee MoveState { get; private set; }
    public RecoveryStateMelee RecoveryState { get; private set; }
    public ChaseStateMelee ChaseState { get; private set; }
    public AttackStateMelee AttackState { get; private set; }
    public DeadStateMelee DeadState { get; private set; }
    public AbilityStateMelee AbilityState { get; private set; }

    #endregion

    [Header("Enemy Settings")] public EnemyMeleeType meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe throw ability")] public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrow;
    public Transform axeStartPoint;

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
        AbilityState = new AbilityStateMelee(this, StateMachine, "AxeThrow");
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

        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    protected override void EnterBattleMode()
    {
        if (InBattleMode)
            return;

        base.EnterBattleMode();
        StateMachine.ChangeState(RecoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed *= 0.6f;
        pulledWeapon.gameObject.SetActive(false);
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

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMeleeType.Dodge)
            return;

        if (StateMachine.CurrentState != ChaseState)
            return;

        if (Vector3.Distance(transform.position, Player.position) < 1.8f)
            return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll");

        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            Anim.SetTrigger(Dodge);
        }
    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMeleeType.AxeThrow)
            return false;

        if (Time.time > lastTimeAxeThrow + axeThrowCooldown)
        {
            lastTimeAxeThrow = Time.time;
            return true;
        }

        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.Log(clipName + " animation not found!");
        return 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.position) < attackData.attackRange;
}