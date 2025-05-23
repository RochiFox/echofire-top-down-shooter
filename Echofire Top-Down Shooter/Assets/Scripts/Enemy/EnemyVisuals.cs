using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyMeleeWeaponType
{
    OneHand,
    Throw,
    Unarmed
}

public enum EnemyRangeWeaponType
{
    Pistol,
    Revolver,
    Shotgun,
    AutoRifle,
    Rifle
}

public class EnemyVisuals : MonoBehaviour
{
    public GameObject CurrentWeaponModel { get; private set; }

    [Header("Corruption visuals")] [SerializeField]
    private GameObject[] corruptionCrystals;

    [SerializeField] private int corruptionAmount;

    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    public void EnableWeaponTrail(bool enable)
    {
        EnemyWeaponModel currentWeaponScript = CurrentWeaponModel.GetComponent<EnemyWeaponModel>();

        currentWeaponScript.EnableTrailEffect(enable);
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<EnemyMelee>();
        bool thisEnemyIsRange = GetComponent<EnemyRange>();

        if (thisEnemyIsRange)
            CurrentWeaponModel = FindRangeWeaponModel();

        if (thisEnemyIsMelee)
            CurrentWeaponModel = FindMeleeWeaponModel();

        CurrentWeaponModel.SetActive(true);

        OverrideAnimatorControllerIfCan();
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMaterial = new(skinnedMeshRenderer.material)
        {
            mainTexture = colorTextures[randomIndex]
        };

        skinnedMeshRenderer.material = newMaterial;
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndex = new List<int>();
        corruptionCrystals = CollectCorruptionCrystals();

        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndex.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (availableIndex.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableIndex.Count);
            int objectIndex = availableIndex[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndex.RemoveAt(randomIndex);
        }
    }

    private GameObject FindRangeWeaponModel()
    {
        EnemyRangeWeaponModel[] weaponModels = GetComponentsInChildren<EnemyRangeWeaponModel>(true);
        EnemyRangeWeaponType weaponType = GetComponent<EnemyRange>().weaponType;

        foreach (EnemyRangeWeaponModel weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                return weaponModel.gameObject;
        }

        Debug.LogWarning($"No range weapon model found for type: {weaponType}");
        return null;
    }

    private GameObject FindMeleeWeaponModel()
    {
        EnemyWeaponModel[] weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);

        EnemyMeleeWeaponType weaponType = GetComponent<EnemyMelee>().weaponType;

        List<EnemyWeaponModel> filteredWeaponModels =
            weaponModels.Where(weaponModel => weaponModel.weaponType == weaponType).ToList();

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        return filteredWeaponModels[randomIndex].gameObject;
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        EnemyCorruptionCrystal[] crystalComponents = GetComponentsInChildren<EnemyCorruptionCrystal>();
        GameObject[] corruptionCrystal = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystal[i] = crystalComponents[i].gameObject;
        }

        return corruptionCrystal;
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController overrideController =
            CurrentWeaponModel.GetComponent<EnemyWeaponModel>()?.overrideController;

        if (overrideController)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }
}