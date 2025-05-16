using UnityEngine;

public class PickupWeapon : Interactable
{
    private PlayerWeaponController weaponController;
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private BackupWeaponModel[] models;

    private void Start()
    {
        UpdateGameObject();
    }

    [ContextMenu("Update Item Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "Pickup Weapon - " + weaponData.weaponType.ToString();
        UpdateItemModel();
    }

    public void UpdateItemModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);

            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weaponData);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!weaponController)
            weaponController = other.GetComponent<PlayerWeaponController>();
    }
}