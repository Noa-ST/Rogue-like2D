using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Đánh dấu class này là một ScriptableObject có thể tạo trong Unity Editor
[CreateAssetMenu(fileName = "Mob Event Data", menuName = "2D Top-down Rogue-like/Event Data/Mob")]
public class MobEventData : EvenData
{
    [Header("Mob Data")]

    // Góc quay tối đa mà quái có thể xuất hiện xung quanh người chơi (từ 0 đến 360 độ)
    [Range(0f, 360f)] public float possibleAngles = 360f;

    // Bán kính dao động của vị trí spawn (quái có thể xuất hiện lệch đi trong khoảng này)
    [Min(0)] public float spawnRadius = 2f;

    // Khoảng cách tối thiểu từ người chơi đến vị trí spawn của quái
    [Min(0)] public float spawnDistance = 20f;

    /// <summary>
    /// Kích hoạt sự kiện spawn quái nếu người chơi tồn tại
    /// </summary>
    /// <param name="player"> Chỉ số người chơi, dùng để xác định vị trí spawn </param>
    /// <returns> Trả về false vì sự kiện này chỉ spawn quái chứ không có điều kiện thành công </returns>
    public override bool Activate(PlayerStat player = null)
    {
        // Chỉ kích hoạt nếu người chơi tồn tại
        if (player)
        {
            // Tạo một góc ngẫu nhiên trong phạm vi cho phép
            float randomAngle = Random.Range(0, possibleAngles) * Mathf.Deg2Rad;

            // Lặp qua tất cả các prefab quái có thể spawn
            foreach (GameObject o in GetSpawns())
            {
                // Tạo vị trí spawn bằng cách di chuyển ra xa người chơi theo một góc ngẫu nhiên
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Cos(randomAngle),
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Sin(randomAngle)
                );

                // Instantiate quái tại vị trí đã tính toán, với hướng quay mặc định
                Instantiate(o, spawnPosition, Quaternion.identity);
            }
        }

        // Luôn trả về false vì sự kiện này không có điều kiện thành công cụ thể
        return false;
    }
}

