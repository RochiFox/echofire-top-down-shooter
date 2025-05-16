using UnityEngine;

public class PickupWeapon : Interactable
{
    private PlayerWeaponController weaponController;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Weapon weapon;

    [SerializeField] private BackupWeaponModel[] models;

    private bool oldWeapon;

    private void Start()
    {
        if (!oldWeapon)
            weapon = new Weapon(weaponData);

        UpdateGameObject();
    }

    public void SetupPickupWeapon(Weapon newWeapon, Transform newTransform)
    {
        oldWeapon = true;

        this.weapon = newWeapon;
        weaponData = newWeapon.WeaponData;

        this.transform.position = newTransform.position + new Vector3(0, 0.75f, 0);
    }

    [ContextMenu("Update Item Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "Pickup Weapon - " + weaponData.weaponType.ToString();
        UpdateItemModel();
    }

    private void UpdateItemModel()
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
        weaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnObject(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!weaponController)
            weaponController = other.GetComponent<PlayerWeaponController>();
    }
}