using UnityEngine;

public class AttackStateMelee : EnemyState
{
    private static readonly int AttackAnimationSpeed = Animator.StringToHash("AttackAnimationSpeed");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
    private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");

    private EnemyMelee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50f;

    public AttackStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.Anim.SetFloat(AttackAnimationSpeed, enemy.attackData.animationSpeed);
        enemy.Anim.SetFloat(AttackIndex, enemy.attackData.attackIndex);

        enemy.Agent.isStopped = true;
        enemy.Agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.transform.rotation = enemy.FaceTarget(enemy.Player.position);
        }


        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection,
                attackMoveSpeed * Time.deltaTime);

            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (!TriggerCalled) return;

        if (enemy.PlayerInAttackRange())
            StateMachine.ChangeState(enemy.RecoveryState);
        else
            StateMachine.ChangeState(enemy.RecoveryState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.Anim.SetFloat(RecoveryIndex, 0);

        if (enemy.PlayerInAttackRange())
            enemy.Anim.SetFloat(RecoveryIndex, 1);
    }
}