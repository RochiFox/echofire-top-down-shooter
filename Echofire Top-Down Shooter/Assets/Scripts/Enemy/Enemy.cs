using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Idle data")] public float idleTime;

    [Header("Move data")] public float moveSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

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
}