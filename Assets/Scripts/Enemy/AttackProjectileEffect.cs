using UnityEngine;

[CreateAssetMenu(fileName = "Buff Data", menuName = "2D Top-down Rogue-like/AttackProjectileEffect")]

public class AttackProjectileEffect : ScriptableObject, IAttackEffect
{
    public GameObject projectilePrefab;
    public float speed = 10f;

    public void Apply(EntityStats target, EntityStats source)
    {
        if (!projectilePrefab) return;

        GameObject go = Instantiate(projectilePrefab, source.transform.position, Quaternion.identity);
        Vector2 direction = (target.transform.position - source.transform.position).normalized;

        if (go.TryGetComponent(out Rigidbody2D rb))
            rb.velocity = direction * speed;
    }
}
