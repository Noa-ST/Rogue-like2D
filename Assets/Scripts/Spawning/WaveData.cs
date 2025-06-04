using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Đại diện một đợt (wave) spawn gồm nhiều enemy hoặc object.
/// Kế thừa từ SpawnData để kế thừa logic spawn chung.
/// </summary>
[CreateAssetMenu(fileName = "Wave Data", menuName = "2D Top-down Rogue-like/Wave Data")]
public class WaveData : SpawnData
{
    [Header("Wave Data")]

    [Tooltip("Nếu số lượng kẻ thù ít hơn con số này, chúng ta sẽ tiếp tục xuất hiện cho đến khi đến nơi.")]
    [Min(0)] public int startingCount = 0;

    [Tooltip("Đợt sóng này có thể tạo ra tối đa bao nhiêu kẻ địch?")]
    [Min(1)] public uint totalSpawns = uint.MaxValue;

    [System.Flags]
    public enum ExitCondition { waveDuration = 1, reachedTotalSpawns = 2 }

    [Tooltip("Thiết lập những điều kiện có thể kích hoạt sự kết thúc của đợt sóng này.")]
    public ExitCondition exitConditions = (ExitCondition)1;

    [Tooltip("Tất cả kẻ thù phải chết thì đợt tấn công mới có thể tiến tới.")]
    public bool mustKillAll = false;

    [HideInInspector] public uint spawnCount;
    public override GameObject[] GetSpawns(int totalEnemies = 0)
    {
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);

        if (totalEnemies + count < startingCount)
            count = startingCount - totalEnemies;

        GameObject[] result = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = possibleSpawnPrefabs[Random.Range(0, possibleSpawnPrefabs.Length)];
        }

        return result;
    }
}



