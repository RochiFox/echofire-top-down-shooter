using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyRange : Enemy
{
    private static readonly int Shoot = Animator.StringToHash("Shoot");

    [Header("Weapon details")] public EnemyRangeWeaponType weaponType;
    public EnemyRangeWeaponData weaponData;
    [Space] public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [SerializeField] public List<EnemyRangeWeaponData> availableWeaponData;

    public IdleStateRange IdleState { get; private set; }
    public MoveStateRange MoveState { get; private set; }
    private BattleStateRange BattleState { get; set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleStateRange(this, StateMachine, "Idle");
        MoveState = new MoveStateRange(this, StateMachine, "Move");
        BattleState = new BattleStateRange(this, StateMachine, "Battle");
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.Initialize(IdleState);
        Visuals.SetupLook();
        SetupWeapon();
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.Update();
    }

    public void FireSingleBullet()
    {
        Anim.SetTrigger(Shoot);

        Vector3 bulletsDirection = ((PlayerTransform.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<EnemyBullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }

    protected override void EnterBattleMode()
    {
        if (InBattleMode)
            return;

        base.EnterBattleMode();

        StateMachine.ChangeState(BattleState);
    }

    private void SetupWeapon()
    {
        List<EnemyRangeWeaponData> filteredData =
            availableWeaponData.Where(weaponData => weaponData.weaponType == weaponType).ToList();

        if (filteredData.Count > 0)
        {
            int random = Random.Range(0, filteredData.Count);
            weaponData = filteredData[random];
        }
        else
        {
            Debug.Log("No available weapon data was found");
        }

        gunPoint = Visuals.CurrentWeaponModel.GetComponent<EnemyRangeWeaponModel>().gunPoint;
    }
}