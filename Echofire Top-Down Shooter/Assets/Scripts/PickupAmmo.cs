using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    SmallBox,
    BigBox
}

public class PickupAmmo : Interactable
{
    private PlayerWeaponController weaponController;

    [SerializeField] private AmmoBoxType boxType;

    [System.Serializable]
    public struct AmmoData
    {
        public WeaponType weaponType;
        [Range(10, 100)] public int minAmount;
        [Range(10, 100)] public int maxAmount;
    }

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.BigBox)
            currentAmmoList = bigBoxAmmo;

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);

            AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
        }

        ObjectPool.instance.ReturnObject(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!weaponController)
            weaponController = other.GetComponent<PlayerWeaponController>();
    }

    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max);

        return Mathf.RoundToInt(randomAmmoAmount);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) return;

        weapon.totalReserveAmmo += amount;
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i != ((int)boxType)) continue;

            boxModel[i].SetActive(true);
            UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
        }
    }
}