using UnityEngine;

public class EnemyMovement : SortTable
{
    protected EnemyStat enemy; // Chỉ cần EnemyStat
    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction { none, respawnEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnEdge;

    [System.Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2 }
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;

    protected bool spawnedOutOfFrame = false;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyStat>();
        if (enemy == null) Debug.LogWarning("EnemyStat not found on " + gameObject.name + "!");
    }

    protected override void Start()
    {
        base.Start();
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
    }

    protected void Update() // Loại bỏ virtual vì không cần ghi đè từ SortTable
    {
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    protected virtual void HandleOutOfFrameAction()
    {
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch (outOfFrameAction)
            {
                case OutOfFrameAction.none:
                default:
                    break;
                case OutOfFrameAction.respawnEdge:
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    if (!spawnedOutOfFrame)
                        Destroy(gameObject);
                    break;
            }
        }
    }

    public void KnockBack(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;

        if (knockbackVariance == 0) return;

        float pow = 1;

        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0;
        bool reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(enemy.Actual.knockbackMultiplier, pow) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(enemy.Actual.knockbackMultiplier, pow) : 1);
    }

    public override void Move()
    {
        if (player == null || rb == null)
        {
            Debug.LogWarning("Enemy " + gameObject.name + " cannot move: player=" + (player != null) + ", rb=" + (rb != null));
            return;
        }

        Vector2 target = player.position;
        Vector2 current = transform.position;
        Vector2 direction = (target - current).normalized;

        // Sử dụng moveSpeed từ EnemyStat
        float moveDistance = enemy.Actual.moveSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + direction * moveDistance);
    }

    protected override void FlipSprite(Vector2 direction)
    {
        if (sorted == null) return;

        // Không flip nếu enemy gần như đứng yên (tránh rung lắc)
        if (Mathf.Abs(direction.x) < 0.1f) return;

        // Flip sprite dựa trên hướng x
        sorted.flipX = direction.x < 0;
    }
}