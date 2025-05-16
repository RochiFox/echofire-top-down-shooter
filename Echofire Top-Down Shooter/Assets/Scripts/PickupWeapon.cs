using UnityEngine;

public class PickupWeapon : Interactable
{
    private PlayerWeaponController weaponController;
    [SerializeField] private WeaponData weaponData;

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