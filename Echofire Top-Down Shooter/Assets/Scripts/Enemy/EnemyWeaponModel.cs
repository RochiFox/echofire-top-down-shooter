using UnityEngine;

public class EnemyWeaponModel : MonoBehaviour
{
    public EnemyMeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController;

    [SerializeField] private GameObject[] trailEffects;

    public void EnableTrailEffect(bool enable)
    {
        foreach (GameObject effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }
}