using UnityEngine;

public class EnemyMovement : SortTable
{
    protected Transform player;
    protected EnemyStat enemy;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction { none, respawnEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnEdge;

    [System.Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2 }
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;

    protected bool spawnedOutOfFrame = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
        enemy = GetComponent<EnemyStat>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    protected virtual void Update()
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

        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(enemy.Actual.knockbackMultiplier, pow) : 1);

        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(enemy.Actual.knockbackMultiplier, pow) : 1);
    }

    public virtual void Move()
    {
        Vector2 target = player.transform.position;
        Vector2 current = transform.position;
        Vector2 direction = (target - current).normalized;

        FlipSprite(direction);
        if (rb)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position,
            player.transform.position,
            enemy.Actual.moveSpeed * Time.deltaTime));
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.Actual.moveSpeed * Time.deltaTime
            );
        }
    }

    protected void FlipSprite(Vector2 direction)
    {
        if (!spriteRenderer) return;

        if (Mathf.Abs(direction.x) < 0.01f)
        {
            spriteRenderer.flipX = direction.x > 0;
        }
    }
}
