using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 20;

    [Header("Idle data")] public float idleTime;
    public float aggressionRange;

    [Header("Move data")] public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    protected bool InBattleMode { get; private set; }

    [SerializeField] private Transform playerTransform;
    public Transform Player => playerTransform;

    public Animator Anim { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    protected EnemyStateMachine StateMachine { get; private set; }

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

    protected bool ShouldEnterBattleMode()
    {
        bool inAggressionRange = Vector3.Distance(transform.position, Player.position) < aggressionRange;

        if (inAggressionRange && !InBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    protected virtual void EnterBattleMode()
    {
        InBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation =
            Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    #region Animation events

    public void ActivateManualMovement(bool newManualMovement) => manualMovement = newManualMovement;
    public void ActivateManualRotation(bool newManualRotation) => manualRotation = newManualRotation;

    public bool ManualMovementActive() => manualMovement;
    public bool ManualRotationActive() => manualRotation;

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        StateMachine.CurrentState.AbilityTrigger();
    }

    #endregion

    #region Patrol logic

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }
}