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

    public virtual void GetHit()
    {
        healthPoints--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void ActivateManualMovement(bool newManualMovement) => manualMovement = newManualMovement;
    public void ActivateManualRotation(bool newManualRotation) => manualRotation = newManualRotation;

    public bool ManualMovementActive() => manualMovement;
    public bool ManualRotationActive() => manualRotation;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressionRange);
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    public bool PlayerInAggressionRange() => Vector3.Distance(transform.position, Player.position) < aggressionRange;

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