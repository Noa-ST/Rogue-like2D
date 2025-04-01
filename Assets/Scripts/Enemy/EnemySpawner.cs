using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replace by the Spawn Manager")]
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;                   // Tên của đợt kẻ địch để phân biệt từng đợt
        public List<EnemyGroup> enemyGroups;      // Danh sách các nhóm kẻ địch có trong đợt này
        public int waveQuota;                     // Tổng số lượng kẻ địch cần spawn trong đợt
        public float spawnInterval;               // Thời gian giãn cách giữa mỗi lần spawn
        public int spawnCount;                    // Số lượng kẻ địch đã được spawn cho đợt hiện tại
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;                  // Tên nhóm enemy 
        public int enemyCount;                    // Số lượng kẻ địch cần spawn trong nhóm này
        public int spawnCount;                    // Số lượng kẻ địch đã spawn từ nhóm này
        public GameObject enemyPrefab;            // Prefab của kẻ địch để tạo đối tượng trong nhóm này
    }

    public List<Wave> waves;                      // Danh sách các đợt kẻ địch (mỗi đợt chứa nhiều nhóm kẻ địch)
    public int currentWaveCount;                  // Số thứ tự của đợt hiện tại (bắt đầu từ 0)

    [Header("Spawner Attributes")]
    float _spawnTimer;                            // Bộ đếm thời gian để kiểm tra khi nào cần spawn kẻ địch tiếp theo
    public int enemiesAlive;                      // Số lượng kẻ địch hiện tại đang có mặt trên màn hình
    public int maxEnemiesAllowed;                 // Giới hạn số lượng kẻ địch tối đa được spawn cùng lúc
    public bool maxEnemiesReached = false;        // Cờ để kiểm tra nếu số lượng kẻ địch đã đạt giới hạn
    public float waveInterval;                    // Thời gian chờ giữa các đợt kẻ địch
    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativesSpawnPoints;  // Các vị trí spawn kẻ địch tương đối so với vị trí của người chơi

    Transform _player;                            // Biến lưu vị trí của người chơi trong trò chơi

    private void Start()
    {
        _player = FindObjectOfType<PlayerStat>().transform; 
        CalculateWaveQuota();                                
    }

    private void Update()
    {
        // Kiểm tra điều kiện bắt đầu đợt mới nếu chưa có kẻ địch nào spawn cho đợt hiện tại
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());  // Bắt đầu đợt tiếp theo với một khoảng thời gian chờ
        }

        _spawnTimer += Time.deltaTime;        // Tăng giá trị bộ đếm thời gian mỗi khung hình

        // Khi bộ đếm thời gian đạt đến thời gian spawn được thiết lập
        if (_spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            _spawnTimer = 0f;                 // Reset bộ đếm
            SpawnEnemies();                   // Gọi hàm spawn kẻ địch
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        yield return new WaitForSeconds(waveInterval);  // Chờ một khoảng thời gian trước khi bắt đầu đợt mới

        // Kiểm tra nếu vẫn còn đợt chưa được spawn
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;               // Tăng đợt hiện tại lên 1
            CalculateWaveQuota();             // Tính lại số lượng kẻ địch cần spawn cho đợt mới
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;             // Biến để lưu tổng số lượng kẻ địch cần spawn trong đợt

        // Lặp qua từng nhóm kẻ địch trong đợt hiện tại để tính tổng số lượng kẻ địch cần spawn
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;  // Cộng số lượng kẻ địch của từng nhóm vào currentWaveQuota
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;  // Gán tổng số lượng kẻ địch cho waveQuota của đợt
        Debug.LogWarning(currentWaveQuota);           // In ra tổng số lượng kẻ địch của đợt để kiểm tra
    }

    void SpawnEnemies()
    {
        // Kiểm tra nếu tổng số lượng kẻ địch đã spawn trong đợt chưa đạt tới waveQuota và chưa đạt giới hạn kẻ địch
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Kiểm tra nếu nhóm kẻ địch này chưa spawn đủ số lượng cần thiết
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Nếu số lượng kẻ địch đang tồn tại đạt tới giới hạn tối đa thì ngừng spawn thêm
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;                 // Thoát ra khỏi vòng lặp và không spawn thêm
                    }

                    // Tạo ra một vị trí spawn ngẫu nhiên dựa trên vị trí của người chơi và một vị trí trong danh sách
                    Vector2 spawnPosition = _player.position + relativesSpawnPoints[Random.Range(0, relativesSpawnPoints.Count)].position;

                    // Instantiate kẻ địch tại vị trí spawn ngẫu nhiên với prefab được thiết lập
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    // Tăng biến đếm cho số lượng kẻ địch đã spawn trong nhóm và trong đợt
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;             // Tăng số lượng kẻ địch hiện tại trên màn hình
                }
            }
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        // Nếu số lượng kẻ địch hiện tại dưới giới hạn tối đa, reset maxEnemiesReached để tiếp tục spawn
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}

