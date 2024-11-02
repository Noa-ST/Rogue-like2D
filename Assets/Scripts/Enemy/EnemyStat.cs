using UnityEngine;

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

    public void TakeDamage(float dmg)
    {
        // Giảm máu của kẻ địch dựa trên sát thương nhận được
        currentHealth -= dmg;

        // Kiểm tra nếu máu <= 0, tiêu diệt kẻ địch
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        // Phá hủy đối tượng kẻ địch khỏi trò chơi khi bị tiêu diệt
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
        es.OnEnemyKilled();
    }
}
