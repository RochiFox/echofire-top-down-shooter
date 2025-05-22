using UnityEngine;

public class BattleStateRange : EnemyState
{
    private readonly EnemyRange enemy;

    private float lastTimeShoot = -10;
    private int bulletsShot = 0;

    public BattleStateRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.Player.position);

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
                AttemptToResetWeapon();

            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttemptToResetWeapon() => bulletsShot = 0;

    private bool WeaponOnCooldown() => Time.time > lastTimeShoot + enemy.weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShot >= enemy.bulletsToShot;

    private bool CanShoot() => Time.time > lastTimeShoot + 1 / enemy.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot = Time.time;
        bulletsShot++;
    }

    public override void Exit()
    {
        base.Exit();
    }
}