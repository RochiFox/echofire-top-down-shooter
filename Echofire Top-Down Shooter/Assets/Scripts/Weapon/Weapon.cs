using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;

    #region Regular mode variables

    public ShootType shootType;
    public int BulletsPerShot { get; private set; }

    private float defaultFireRate;
    private float fireRate; // bullets per second
    private float lastShootTime;

    #endregion

    #region Burst mode variables

    private bool burstAvailable;
    public bool burstActive;

    private int burstBulletsPerShot;
    private float burstFireRate;
    public float BurstFireDelay { get; private set; }

    #endregion

    [Header("Ammo details")] public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Weapon generic info variables

    public float ReloadSpeed { get; private set; }
    public float EquipmentSpeed { get; private set; }
    public float GunDistance { get; private set; }
    public float CameraDistance { get; private set; }

    #endregion

    public WeaponData WeaponData { get; private set; }

    #region Weapon spread variables

    [Header("Spread")] private float baseSpread;
    private float currentSpread;
    private float maxSpread;

    private float spreadIncreaseRate;

    private float lastSpreadUpdateTime;
    private const float SPREAD_COOLDOWN = 1;

    #endregion

    public Weapon(WeaponData weaponData)
    {
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        BulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        BurstFireDelay = weaponData.burstFireDelay;

        baseSpread = weaponData.baseSpread;
        maxSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        ReloadSpeed = weaponData.reloadSpeed;
        EquipmentSpeed = weaponData.equipmentSpeed;
        GunDistance = weaponData.gunDistance;
        CameraDistance = weaponData.cameraDistance;

        defaultFireRate = fireRate;
        this.WeaponData = weaponData;
    }

    #region Burst methods

    public bool BurstActivated()
    {
        if (weaponType != WeaponType.Shotgun) return burstActive;

        BurstFireDelay = 0;
        return true;
    }

    public void ToggleBurst()
    {
        if (burstAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            BulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            BulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (!(Time.time > lastShootTime + 1 / fireRate)) return false;

        lastShootTime = Time.time;
        return true;
    }

    #region Spread methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizeValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizeValue, randomizeValue, randomizeValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + SPREAD_COOLDOWN)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maxSpread);
    }

    #endregion

    #region Reload methods

    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
            return false;

        return totalReserveAmmo > 0;
    }

    public void RefillBullets()
    {
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;
    }

    #endregion
}