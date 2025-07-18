using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))] // Thêm Rigidbody2D để di chuyển
public abstract class SortTable : MonoBehaviour
{
    protected SpriteRenderer sorted; // Đổi private thành protected để lớp con truy cập
    public bool sortingActive = true;
    public float minimumDistance = 0.2f;
    public Transform player; // Tham chiếu đến người chơi
    protected Rigidbody2D rb; // Rigidbody2D để di chuyển
    public float stopDistance = 5f; // Khoảng cách dừng
    public float safeDistance = 1f; // Khoảng cách an toàn để lùi lại

    protected virtual void Awake()
    {
        sorted = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (sorted == null || rb == null)
        {
            Debug.LogError($"Missing component on {gameObject.name}: SpriteRenderer={sorted != null}, Rigidbody2D={rb != null}");
        }
        if (player == null)
        {
            Debug.LogWarning($"Player not assigned for {gameObject.name}");
        }
    }

    protected virtual void Start()
    {
        UpdatePlayerReference();
    }

    protected virtual void LateUpdate()
    {
        if (sortingActive)
        {
            int newSortOrder = (int)(-transform.position.y / minimumDistance);
            if (lastSortOrder != newSortOrder) sorted.sortingOrder = newSortOrder;
            lastSortOrder = newSortOrder;
        }
    }

    // Phương thức di chuyển (cho phép ghi đè)
    public virtual void Move()
    {
        if (player == null || rb == null)
        {
            Debug.LogWarning($"Cannot move {gameObject.name}: player={player != null}, rb={rb != null}");
            return;
        }

        Vector2 target = player.position;
        Vector2 current = transform.position;
        Vector2 direction = (target - current).normalized;
        float distance = Vector2.Distance(current, target);

        // Nếu quá xa, không di chuyển
        if (distance > stopDistance) return;

        // Nếu quá gần, lùi lại
        if (distance < safeDistance)
        {
            direction = -direction;
        }

        FlipSprite(direction);
        float moveDistance = GetMoveSpeed() * Time.deltaTime;
        rb.MovePosition(rb.position + direction * moveDistance);
    }

    // Phương thức để lật sprite, giờ là virtual
    protected virtual void FlipSprite(Vector2 direction)
    {
        if (sorted != null)
        {
            sorted.flipX = direction.x < 0; // Lật ngang khi di chuyển sang trái
        }
    }

    // Phương thức ảo để lấy tốc độ di chuyển (cho phép ghi đè)
    protected virtual float GetMoveSpeed()
    {
        return 2f; // Tốc độ mặc định
    }

    private int lastSortOrder = 0; // Biến để theo dõi sort order trước đó

    public static void UpdatePlayerReference()
    {
        SortTable[] objects = FindObjectsOfType<SortTable>();
        foreach (var obj in objects)
        {
            obj.player = FindObjectOfType<PlayerMovement>()?.transform;
            if (obj.player == null)
            {
                Debug.LogWarning($"Object {obj.gameObject.name} could not find PlayerMovement!");
            }
        }
    }
}