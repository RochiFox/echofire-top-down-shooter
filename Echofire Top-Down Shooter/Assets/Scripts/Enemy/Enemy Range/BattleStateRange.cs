using UnityEngine;

public class BattleStateRange : EnemyState
{
    private readonly EnemyRange enemy;

    private float lastTimeShoot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    public BattleStateRange(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase,
        stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        enemy.Visuals.EnableIK(true);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(Enemy.PlayerTransform.position);

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

    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }


    private bool WeaponOnCooldown() => Time.time > lastTimeShoot + weaponCooldown;

    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;

    private bool CanShoot() => Time.time > lastTimeShoot + 1 / enemy.weaponData.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShoot = Time.time;
        bulletsShot++;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.Visuals.EnableIK(false);
    }
}