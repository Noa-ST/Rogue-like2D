using UnityEngine;

public abstract class EventData : SpawnData
{
    [Header("Event Settings")]

    // Xác suất cơ bản để sự kiện xảy ra, giá trị trong khoảng [0, 1]
    [Range(0f, 1f)] public float probability = 1f;

    // Hệ số ảnh hưởng của chỉ số may mắn vào xác suất sự kiện xảy ra
    [Range(0f, 1f)] public float luckFator = 1f;

    // Khoảng thời gian (tính bằng giây) sau khi màn chơi bắt đầu thì sự kiện này mới có thể xảy ra
    public float activeAfter = 0;

    /// <summary>
    /// Phương thức trừu tượng, các lớp con kế thừa phải triển khai cách sự kiện được kích hoạt
    /// </summary>
    /// <param name="player"> Thông tin về người chơi, có thể là null </param>
    /// <returns> Trả về true nếu sự kiện kích hoạt thành công </returns>
    public abstract bool Activate(PlayerStat player = null);

    /// <summary>
    /// Kiểm tra xem sự kiện đã có thể xảy ra chưa (đủ thời gian yêu cầu chưa)
    /// </summary>
    /// <returns> Trả về true nếu sự kiện đủ điều kiện để xảy ra </returns>
    public bool IsActive()
    {
        if (!GameManager.Ins) return false;
        if (GameManager.Ins.GetElapsedTime() > activeAfter) return true;
        return false;
    }

    /// <summary>
    /// Kiểm tra xem sự kiện có xảy ra hay không dựa trên xác suất và chỉ số may mắn của người chơi.
    /// Luck càng cao thì xác suất xảy ra càng giảm (giảm theo hàm mũ nhẹ).
    /// </summary>
    /// <param name="s">Chỉ số người chơi.</param>
    /// <returns>True nếu sự kiện xảy ra, false nếu không.</returns>
    public bool CheckIfWillHappen(PlayerStat s)
    {
        if (probability >= 1f) return true;

        // Giảm xác suất theo luck (luck càng cao, xác suất càng giảm nhẹ)
        float luckFactor = Mathf.Pow(1f - luckFator, s.Stats.luck);
        float actualProbability = probability * luckFactor;

        return actualProbability >= Random.Range(0f, 1f);
    }
}

