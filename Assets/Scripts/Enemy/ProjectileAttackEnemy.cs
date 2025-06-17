using UnityEngine;

public class ProjectileAttackEnemy : MonoBehaviour
{
    public float damage = 5f;
    public float lifeTime = 5f;
    public LayerMask targetLayer; // Lớp đối tượng có thể bị trúng đạn

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy sau vài giây
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm đúng lớp đối tượng mục tiêu
        if ((targetLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            EntityStats target = collision.GetComponent<EntityStats>();
            if (target != null)
            {
                target.TakeDamage(damage); // Gây sát thương
            }

            Destroy(gameObject); // Hủy đạn sau khi trúng mục tiêu
        }
    }
}
