using UnityEngine;

public class PickupWeapon : Interactable
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Weapon weapon;

    [SerializeField] private BackupWeaponModel[] models;

    private bool oldWeapon;

    private void Start()
    {
        if (!oldWeapon)
            weapon = new Weapon(weaponData);

        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon newWeapon, Transform newTransform)
    {
        oldWeapon = true;

        this.weapon = newWeapon;
        weaponData = newWeapon.WeaponData;

        this.transform.position = newTransform.position + new Vector3(0, 0.75f, 0);
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup Weapon - " + weaponData.weaponType.ToString();
        SetupWeaponModel();
    }

    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);

            if (model.weaponType != weaponData.weaponType) continue;

            model.gameObject.SetActive(true);
            UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
        }
    }

    public override void Interaction()
    {
        WeaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnObject(gameObject);
    }
}