using System;
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
    [Header("Weapon model")] [SerializeField]
    private EnemyWeaponModel[] weaponModels;

    public GameObject CurrentWeaponModel { get; private set; }

    [SerializeField] private EnemyMeleeWeaponType weaponType;

    [Header("Color")] [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
    }

    public void SetupWeaponType(EnemyMeleeWeaponType type) => weaponType = type;

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
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
}