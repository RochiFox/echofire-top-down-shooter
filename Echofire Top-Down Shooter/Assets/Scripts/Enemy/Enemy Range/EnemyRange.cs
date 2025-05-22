using UnityEngine;

public class EnemyRange : Enemy
{
    private static readonly int Shoot = Animator.StringToHash("Shoot");

    public Transform weaponHolder;
    public float fireRate = 1; // Bullets per second
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public float bulletSpeed = 20;
    public int bulletsToShot = 5; // Bullets to shoot before weapon goes on cooldown
    public float weaponCooldown = 1.5f; // Weapon cooldown

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
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.Update();
    }

    public void FireSingleBullet()
    {
        Anim.SetTrigger(Shoot);

        Vector3 bulletsDirection = ((Player.position + Vector3.up) - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<EnemyBullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = 20 / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    protected override void EnterBattleMode()
    {
        if (InBattleMode)
            return;

        base.EnterBattleMode();

        StateMachine.ChangeState(BattleState);
    }
}