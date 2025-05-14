using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;
    private Camera mainCam;

    [Header("Aim Visual - Laser")] [SerializeField]
    private LineRenderer aimLaser;

    [Header("Aim control")] [SerializeField]
    private Transform aim;

    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private bool isLockingToTarget;

    [Header("Camera control")] [SerializeField]
    private Transform cameraTarget;

    [Range(.5f, 2f)] [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 4f)] [SerializeField] private float maxCameraDistance = 4;
    [Range(3f, 5f)] [SerializeField] private float cameraSensitivity = 5f;

    [Space] [SerializeField] private LayerMask aimLayerMask;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Awake()
    {
        player = GetComponent<Player>();
        mainCam = Camera.main;
    }

    private void Start()
    {
        AssignInputEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            isAimingPrecisely = !isAimingPrecisely;

        if (Input.GetKeyDown(KeyCode.L))
            isLockingToTarget = !isLockingToTarget;

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        aimLaser.enabled = player.Weapon.WeaponReady();

        if (aimLaser.enabled == false)
            return;

        WeaponModel weaponModel = player.WeaponVisuals.CurrentWeaponModel();

        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.Weapon.GunPoint();
        Vector3 laserDirection = player.Weapon.BulletDirection();

        float laserTipLenght = .5f;
        float gunDistance = player.Weapon.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLenght = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (target && isLockingToTarget)
        {
            aim.position = target.GetComponent<Renderer>()
                ? target.GetComponent<Renderer>().bounds.center
                : target.position;

            return;
        }

        aim.position = GetMouseHitInfo().point;

        if (!isAimingPrecisely)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfo().transform.GetComponent<Target>())
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aim;
    public bool CanAimPrecisely() => isAimingPrecisely;

    public RaycastHit GetMouseHitInfo()
    {
        if (!mainCam)
            return lastKnownMouseHit;

        Ray ray = mainCam.ScreenPointToRay(mouseInput);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask)) return lastKnownMouseHit;
        lastKnownMouseHit = hitInfo;
        return hitInfo;
    }

    #region Camera Region

    private void UpdateCameraPosition()
    {
        cameraTarget.position =
            Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.Movement.MoveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    #endregion

    private void AssignInputEvents()
    {
        controls = player.Controls;

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += _ => mouseInput = Vector2.zero;
    }
}