using UnityEngine;

public enum EnemyRangeWeaponHoldType
{
    Common,
    LowHold,
    HighHold
}

public class EnemyRangeWeaponModel : MonoBehaviour
{
    public EnemyRangeWeaponType weaponType;
    public EnemyRangeWeaponHoldType weaponHoldType;
}