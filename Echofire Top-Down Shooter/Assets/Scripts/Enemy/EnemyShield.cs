using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");

    private EnemyMelee enemy;
    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyMelee>();
    }

    public void ReduceDurability()
    {
        durability--;

        if (durability > 0) return;

        enemy.Anim.SetFloat(ChaseIndex, 0); // Enables default chase animation
        Destroy(gameObject);
    }
}