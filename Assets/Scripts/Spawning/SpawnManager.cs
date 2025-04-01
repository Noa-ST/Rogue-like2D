using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex;
    int currentWaveSpawnCount = 0;

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, stop spawning any more. For performance.")]
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
            ActiveCooldown();
            return;
        }

        GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStat.count);

        foreach (GameObject prefab in spawns)
        {
            if (!CanSpawn()) continue;

            Instantiate(prefab, GeneratePosition(), Quaternion.identity);
            currentWaveSpawnCount++;
        }

        ActiveCooldown();
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

        switch(Random.Range(0, 2))
        {
            case 0: default:
                return Ins.referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y) );
            case 1:
                return Ins.referenceCamera.ViewportToWorldPoint(new Vector3(x, Mathf.Round(y)) );
        }
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
