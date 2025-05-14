using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int Reload = Animator.StringToHash("Reload");
    private static readonly int ReloadSpeed = Animator.StringToHash("ReloadSpeed");
    private static readonly int EquipWeapon = Animator.StringToHash("EquipWeapon");
    private static readonly int Type = Animator.StringToHash("EquipType");
    private static readonly int EquipSpeed = Animator.StringToHash("EquipSpeed");

    private Player player;
    private Animator anim;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Rig ")] [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncreaseRigWeight;
    private Rig rig;

    [Header("Left hand IK")] [SerializeField]
    private float leftHandIkWeightIncreaseRate;

    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIKTarget;
    private bool shouldIncreaseLeftHandIKWeight;

    private void Awake()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public void PlayFireAnimation() => anim.SetTrigger(Fire);

    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.Weapon.CurrentWeapon().reloadSpeed;

        anim.SetTrigger(Reload);
        anim.SetFloat(ReloadSpeed, reloadSpeed);
        ReduceRigWeight();
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.Weapon.CurrentWeapon().weaponType;

        foreach (WeaponModel model in weaponModels)
        {
            if (model.weaponType == weaponType)
            {
                weaponModel = model;
            }
        }

        return weaponModel;
    }

    #region Animation Rigging Methods

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIKTarget.localPosition = targetTransform.localPosition;
        leftHandIKTarget.localRotation = targetTransform.localRotation;
    }

    private void UpdateLeftHandIKWeight()
    {
        if (!shouldIncreaseLeftHandIKWeight) return;

        leftHandIK.weight += leftHandIkWeightIncreaseRate * Time.deltaTime;

        if (leftHandIK.weight >= 1)
            shouldIncreaseLeftHandIKWeight = false;
    }

    private void UpdateRigWeight()
    {
        if (!shouldIncreaseRigWeight) return;

        rig.weight += rigWeightIncreaseRate * Time.deltaTime;

        if (rig.weight >= 1)
            shouldIncreaseRigWeight = false;
    }

    private void ReduceRigWeight()
    {
        rig.weight = .15f;
    }

    public void MaximizeRigWeight() => shouldIncreaseRigWeight = true;
    public void MaximizeLeftHandWeight() => shouldIncreaseLeftHandIKWeight = true;

    #endregion

    public void PlayWeaponEquipAnimation()
    {
        EquipType grabType = CurrentWeaponModel().equipAnimationType;
        float equipmentSpeed = player.Weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();

        anim.SetTrigger(EquipWeapon);
        anim.SetFloat(Type, ((float)grabType));
        anim.SetFloat(EquipSpeed, equipmentSpeed);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = (int)CurrentWeaponModel().holdType;

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (player.Weapon.HasOnlyOneWeapon() == false)
            SwitchOnBackupWeaponModel();

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    private void SwitchOffWeaponModels()
    {
        foreach (WeaponModel model in weaponModels)
        {
            model.gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in backupWeaponModels)
            backupModel.Activate(false);
    }

    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == player.Weapon.CurrentWeapon().weaponType)
                continue;

            if (player.Weapon.WeaponInSlots(backupModel.weaponType) == null) continue;

            if (backupModel.HangTypeIs(HangType.LowBackHang))
                lowHangWeapon = backupModel;

            if (backupModel.HangTypeIs(HangType.BackHang))
                backHangWeapon = backupModel;

            if (backupModel.HangTypeIs(HangType.SideHang))
                sideHangWeapon = backupModel;
        }

        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }
}