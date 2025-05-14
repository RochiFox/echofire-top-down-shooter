using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int XVelocity = Animator.StringToHash("xVelocity");
    private static readonly int ZVelocity = Animator.StringToHash("zVelocity");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    private Player player;
    private CharacterController characterController;
    private PlayerControls controls;
    private Animator animator;

    [Header("Movement info")] [SerializeField]
    private float walkSpeed;

    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float speed;
    private float verticalVelocity;

    public Vector2 MoveInput { get; private set; }
    private Vector3 movementDirection;

    private bool isRunning;

    private void Awake()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        speed = walkSpeed;

        AssignInputEvents();
    }


    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat(XVelocity, xVelocity, .1f, Time.deltaTime);
        animator.SetFloat(ZVelocity, zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = isRunning & movementDirection.magnitude > 0;
        animator.SetBool(IsRunning, playRunAnimation);
    }

    private void ApplyRotation()
    {
        Vector3 lookingDirection = player.Aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(MoveInput.x, 0, MoveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * (Time.deltaTime * speed));
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
            verticalVelocity = -.5f;
    }

    private void AssignInputEvents()
    {
        controls = player.Controls;

        controls.Character.Movement.performed += context => MoveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += _ => MoveInput = Vector2.zero;

        controls.Character.Run.performed += _ =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        controls.Character.Run.canceled += _ =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}