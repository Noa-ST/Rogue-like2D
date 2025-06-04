using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SpawnData : ScriptableObject
{
    [Tooltip("Danh sách tất cả các GameObject có thể được tạo ra.")]
    public GameObject[] possibleSpawnPrefabs = new GameObject[1];

    [Tooltip("Thời gian giữa mỗi lần xuất hiện (tính bằng giây). Sẽ lấy một số ngẫu nhiên giữa X và Y.")]
    public Vector2 spawnInterval = new Vector2(2, 3);

    [Tooltip("Có bao nhiêu kẻ địch được sinh ra trong mỗi khoảng thời gian?")]
    public Vector2Int spawnsPerTick = new Vector2Int(1, 1);

    [Tooltip("Kẻ thù sẽ xuất hiện trong bao lâu (tính bằng giây)?")]
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
