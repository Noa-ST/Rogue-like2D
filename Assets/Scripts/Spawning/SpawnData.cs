using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnData : ScriptableObject
{
    [Tooltip("A list of all possible GameObjects that can be spawned.")]
    public GameObject[] possibleSpawnPrefabs = new GameObject[1];

    [Tooltip("Time between each spawn (in seconds). Will take a random number between X and Y.")]
    public Vector2 spawnInterval = new Vector2(2, 3);

    [Tooltip("How many enemies are spawned per interval?")]
    public Vector2Int spawnsPerTick = new Vector2Int(1, 1);

    [Tooltip("How long (in seconds) this will spawn enemies for.")]
    [Min(0.1f)] public float duration = 60;

    public virtual GameObject[] GetSpawns(int totalEnemies = 0)
    {
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);
        GameObject[] result = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            result[i] = possibleSpawnPrefabs[Random.Range(0, possibleSpawnPrefabs.Length)];
        }

        return result;
    }

    public virtual float GetSpawnInterval()
    {
        return Random.Range(spawnInterval.x, spawnInterval.y);
    }
}
