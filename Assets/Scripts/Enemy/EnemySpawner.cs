using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;                   // Tên của sóng, để phân biệt từng đợt quái
        public List<EnemyGroup> enemyGroups;      // Danh sách các nhóm quái vật trong sóng
        public int waveQuota;                     // Số lượng quái vật cần spawn trong sóng
        public float spawnInterval;               // Thời gian giữa mỗi lần spawn
        public int spawnCount;                    // Số lượng quái vật đã spawn cho sóng này
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;                  // Tên của nhóm quái vật
        public int enemyCount;                    // Số lượng quái vật trong nhóm này cần spawn
        public int spawnCount;                    // Số lượng quái vật đã spawn từ nhóm này
        public GameObject enemyPrefab;            // Prefab của quái vật để tạo ra trong nhóm này
    }

    public List<Wave> waves;                      // Danh sách các sóng trong trò chơi
    public int currentWaveCount;                  // Chỉ số của sóng hiện tại đang spawn

    [Header("Spawner Atributes")]
    float _spawnTimer;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;

    [Header("Spawn Positions")]
    public List<Transform> relativesSpawnPoints;

    Transform _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerStat>().transform;
        CalculateWaveQuota();                     // Gọi hàm để tính toán số lượng quái vật cho sóng hiện tại
    }

    private void Update()
    {
        if ((currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0))
        {
            StartCoroutine(BeginNextWave());
        }
        {

        }
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if (currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;                 // Khởi tạo biến đếm số lượng quái vật cần spawn cho sóng hiện tại

        // Duyệt qua từng nhóm quái vật trong sóng hiện tại
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount; // Cộng số lượng quái của từng nhóm vào biến currentWaveQuota
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota; // Gán tổng số lượng quái cần spawn cho sóng
        Debug.LogWarning(currentWaveQuota);           // In ra số lượng quái cần spawn cho sóng hiện tại để debug
    }

    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    Instantiate(enemyGroup.enemyPrefab, _player.position + relativesSpawnPoints[Random.Range(0, relativesSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}

