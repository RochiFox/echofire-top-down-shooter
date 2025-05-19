public class DeadStateMelee : EnemyState
{
    private EnemyMelee enemy;
    private EnemyRagdoll ragdoll;

    private bool interactionDisabled;

    public DeadStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = EnemyBase as EnemyMelee;
        ragdoll = enemy?.GetComponent<EnemyRagdoll>();
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisabled = false;

        enemy.Anim.enabled = false;
        enemy.Agent.isStopped = true;

        ragdoll.RagdollActive(true);

        StateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        // TODO: check later if interaction will degrade performance of the game
        if (!(StateTimer < 0) || interactionDisabled) return;
        interactionDisabled = true;
        ragdoll.RagdollActive(false);
        ragdoll.CollidersActive(false);
    }

    public override void Exit()
    {
        base.Exit();
    }
}