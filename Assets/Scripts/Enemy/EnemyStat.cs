using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStat : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    // Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;      // Khoảng cách tối đa từ người chơi mà kẻ địch sẽ bị di chuyển lại gần
    Transform _player;                       // Biến lưu vị trí của người chơi

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float damageFadeTime = 0.6f;
    Color _originalColor;
    SpriteRenderer _sr;
    EnemyMovement _em;
    public float deathFadeTime;

    void Awake()
    {
        // Khởi tạo giá trị cho các chỉ số hiện tại của kẻ địch từ dữ liệu cấu hình
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    private void Start()
    {
        _player = FindObjectOfType<PlayerStat>().transform;
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
        _em = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        // Kiểm tra khoảng cách giữa kẻ địch và người chơi
        if (Vector2.Distance(transform.position, _player.position) >= despawnDistance)
        {
            ReturnEnemy();  // Nếu quá xa, đưa kẻ địch về gần người chơi hơn
        }
    }

    private void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();

        // Đặt lại vị trí của kẻ địch gần người chơi tại một vị trí spawn ngẫu nhiên
        transform.position = _player.position + es.relativesSpawnPoints[Random.Range(0, es.relativesSpawnPoints.Count)].position;
    }

    public void TakeDamage(float dmg, Vector2 sourcePostion, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        // Giảm máu của kẻ địch dựa trên sát thương nhận được
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        if (dmg > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePostion;
            _em.KnockBack(dir.normalized * knockbackForce, knockbackDuration);
        }

        // Kiểm tra nếu máu <= 0, tiêu diệt kẻ địch
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    IEnumerator DamageFlash()
    {
        _sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        _sr.color = _originalColor;
    }

    public void Kill()
    {
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = _sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Kiểm tra nếu kẻ địch va chạm với đối tượng có tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStat player = collision.gameObject.GetComponent<PlayerStat>();

            // Gây sát thương lên người chơi khi va chạm
            player.TakeDamage(currentDamage);
        }
    }

    private void OnDestroy()
    {
        // Khi kẻ địch bị tiêu diệt, gọi hàm `OnEnemyKilled` từ `EnemySpawner` để cập nhật số lượng kẻ địch hiện tại
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        if (es != null)
        {
            es.OnEnemyKilled();
        }
    }
}
