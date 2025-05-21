using UnityEngine;

public class AbilityStateMelee : EnemyState
{
    private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");

    private readonly EnemyMelee enemy;
    private Vector3 movementDirection;

    private const float MAX_MOVEMENT_DISTANCE = 20;

    private float moveSpeed;

    public AbilityStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = EnemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.EnableWeaponModel(true);

        moveSpeed = enemy.moveSpeed;

        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.Player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection,
                enemy.moveSpeed * Time.deltaTime);
        }

        if (TriggerCalled)
            StateMachine.ChangeState(enemy.RecoveryState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = moveSpeed;
        enemy.Anim.SetFloat(RecoveryIndex, 0);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject newAxe = ObjectPool.instance.GetObject(enemy.axePrefab);

        newAxe.transform.position = enemy.axeStartPoint.position;
        newAxe.GetComponent<EnemyAxe>().AxeSetup(enemy.axeFlySpeed, enemy.Player, enemy.axeAimTimer);
    }
}