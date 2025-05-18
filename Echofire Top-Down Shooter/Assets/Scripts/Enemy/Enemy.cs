using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;
    public float aggressionRange;

    [Header("Attack data")] public float attackRange;
    public float attackMoveSpeed;

    [Header("Idle data")] public float idleTime;

    [Header("Move data")] public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    [SerializeField] private Transform playerTransform;
    public Transform Player => playerTransform;

    public Animator Anim { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }

    protected virtual void Awake()
    {
        StateMachine = new EnemyStateMachine();

        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
    }

    public void ActivateManualMovement(bool newManualMovement) => manualMovement = newManualMovement;
    public bool ManualMovementActive() => manualMovement;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    public bool PlayerInAggressionRange() => Vector3.Distance(transform.position, Player.position) < aggressionRange;
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.position) < attackRange;

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation =
            Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }
}