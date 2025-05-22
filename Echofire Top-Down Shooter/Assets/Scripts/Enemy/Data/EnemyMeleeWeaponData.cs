using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Melee Weapon Data")]
public class EnemyMeleeWeaponData : ScriptableObject
{
    public List<EnemyMeleeAttackData> attackData;
    public float turnSpeed = 10;
}