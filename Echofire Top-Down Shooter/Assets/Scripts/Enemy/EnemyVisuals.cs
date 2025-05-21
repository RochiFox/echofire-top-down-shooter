using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyMeleeWeaponType
{
    OneHand,
    Throw
}

public class EnemyVisuals : MonoBehaviour
{
    [Header("Weapon visuals")] [SerializeField]
    private EnemyWeaponModel[] weaponModels;

    public GameObject CurrentWeaponModel { get; private set; }

    [Header("Corruption visuals")] [SerializeField]
    private GameObject[] corruptionCrystals;

    [SerializeField] private int corruptionAmount;

    [SerializeField] private EnemyMeleeWeaponType weaponType;

    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
        CollectCorruptionCrystals();
    }

    public void SetupWeaponType(EnemyMeleeWeaponType type) => weaponType = type;

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomWeapon()
    {
        foreach (EnemyWeaponModel weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<EnemyWeaponModel> filteredWeaponModels =
            weaponModels.Where(weaponModel => weaponModel.weaponType == weaponType).ToList();

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        CurrentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
        CurrentWeaponModel.SetActive(true);
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMaterial = new Material(skinnedMeshRenderer.material)
        {
            mainTexture = colorTextures[randomIndex]
        };

        skinnedMeshRenderer.material = newMaterial;
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndex = new List<int>();

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

    private void CollectCorruptionCrystals()
    {
        EnemyCorruptionCrystal[] crystalComponents = GetComponentsInChildren<EnemyCorruptionCrystal>();
        corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }
    }
}