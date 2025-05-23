using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyMeleeAttackData
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
    private DeadStateMelee DeadState { get; set; }
    public AbilityStateMelee AbilityState { get; private set; }

    #endregion

    [Header("Enemy Settings")] public EnemyMeleeType meleeType;
    public EnemyMeleeWeaponType weaponType;

    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe throw ability")] public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeAxeThrow;
    public Transform axeStartPoint;

    [Header("Attack Data")] public EnemyMeleeAttackData attackData;
    public List<EnemyMeleeAttackData> attackList;

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

        InitializePerk();
        Visuals.SetupLook();
        UpdateAttackData();
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
        StateMachine.ChangeState(RecoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed *= 0.6f;
        EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        EnemyWeaponModel currentWeapon = Visuals.CurrentWeaponModel.GetComponent<EnemyWeaponModel>();

        if (currentWeapon.weaponData)
        {
            attackList = new List<EnemyMeleeAttackData>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    private void InitializePerk()
    {
        switch (meleeType)
        {
            case EnemyMeleeType.AxeThrow:
                weaponType = EnemyMeleeWeaponType.Throw;
                break;
            case EnemyMeleeType.Shield:
                Anim.SetFloat(ChaseIndex, 1);
                shieldTransform.gameObject.SetActive(true);
                weaponType = EnemyMeleeWeaponType.OneHand;
                break;
            case EnemyMeleeType.Dodge:
                weaponType = EnemyMeleeWeaponType.Unarmed;
                break;
            case EnemyMeleeType.Regular:
                weaponType = EnemyMeleeWeaponType.OneHand;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
            StateMachine.ChangeState(DeadState);
    }

    public void EnableWeaponModel(bool active)
    {
        Visuals.CurrentWeaponModel.gameObject.SetActive(active);
    }

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMeleeType.Dodge)
            return;

        if (StateMachine.CurrentState != ChaseState)
            return;

        if (Vector3.Distance(transform.position, PlayerTransform.position) < 1.8f)
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

    public bool PlayerInAttackRange() =>
        Vector3.Distance(transform.position, PlayerTransform.position) < attackData.attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}