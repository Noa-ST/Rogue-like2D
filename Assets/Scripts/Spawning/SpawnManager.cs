using UnityEngine;

/// <summary>
/// Điều phối toàn bộ quá trình spawn
/// </summary>
public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex;
    int currentWaveSpawnCount = 0;

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("Nếu có nhiều hơn số lượng kẻ thù này, hãy dừng việc sinh sản thêm nữa. Để có hiệu suất.")]
    public int maximumEnemyCount = 300;
    float spawnTimer;
    float currentWaveDuration = 0f;
    public bool boostedByCurse = true;

    public static SpawnManager Ins;

    private void Start()
    {
        if (Ins) Debug.LogWarning("There is more than 1 Spawn Manager in the Scene! Plese remove the extras.");
        Ins = this;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0)
        {
            if (HasWaveEnded())
            {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;

                if (currentWaveIndex >= data.Length)
                {
                    Debug.Log("All waves have been spawned! Shutting down.", this);
                    enabled = false;
                }
                return;
            }

            if (!CanSpawn())
            {
                spawnTimer += data[currentWaveIndex].GetSpawnInterval();
                return;
            }

            GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStat.count);

            foreach (GameObject prefab in spawns)
            {
                if (!CanSpawn()) continue;

                Instantiate(prefab, GeneratePosition(), Quaternion.identity);
                currentWaveSpawnCount++;
            }

            spawnTimer += data[currentWaveIndex].GetSpawnInterval();
        }
    }

    private void ActiveCooldown()
    {
        float curseBoost = boostedByCurse ? GameManager.GetCumulativeCurse() : 1;
        spawnTimer += data[currentWaveIndex].GetSpawnInterval() / curseBoost;

    }

    private bool CanSpawn()
    {
        if (hasExceededMaxEnemies()) return false;

        if (Ins.currentWaveSpawnCount > Ins.data[Ins.currentWaveIndex].totalSpawns) return false;

        if (Ins.currentWaveDuration > Ins.data[Ins.currentWaveIndex].duration) return false;
        return true;
    }

    private static bool hasExceededMaxEnemies()
    {
        if (!Ins) return false;
        if (EnemyStat.count > Ins.maximumEnemyCount) return true;
        return false;
    }

    private bool HasWaveEnded()
    {
        WaveData currentWave = data[currentWaveIndex];

        if ((currentWave.exitConditions & WaveData.ExitCondition.waveDuration) > 0)
            if (currentWaveDuration < currentWave.duration) return false;

        if ((currentWave.exitConditions & WaveData.ExitCondition.reachedTotalSpawns) > 0)
            if (currentWaveSpawnCount < currentWave.totalSpawns) return false;

        if (currentWave.mustKillAll && EnemyStat.count > 0)
            return false;

        return true;
    }

    private void Reset()
    {
        referenceCamera = Camera.main;
    }

    public static Vector3 GeneratePosition()
    {
        if (!Ins.referenceCamera) Ins.referenceCamera = Camera.main;

        if (!Ins.referenceCamera.orthographic)
            Debug.LogWarning("The reference camera is not orthograhic! This will cause enemy spawns to sometimes appear within camera boundaries!");

        float x = Random.Range(0f, 1f), y = Random.Range(0f, 1f);

        Vector3 viewportPoint = new Vector3(Mathf.Round(x), y, 0f); // Đặt Z = 0 trong viewport
        Vector3 worldPosition = Ins.referenceCamera.ViewportToWorldPoint(viewportPoint);
        worldPosition.z = 0f;
        Debug.Log("Generated spawn position: " + worldPosition); // Debug vị trí spawn
        return worldPosition;
    }

    public static bool IsWithinBoundaries(Transform checkedObject)
    {
        Camera c = Ins && Ins.referenceCamera ? Ins.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkedObject.position);
        if (viewport.x < 0f || viewport.x > 1f) return false;
        if (viewport.y < 0f || viewport.y > 1f) return false;
        return true;
    }
}