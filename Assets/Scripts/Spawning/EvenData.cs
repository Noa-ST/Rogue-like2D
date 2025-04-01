using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EvenData : SpawnData
{
    [Header("Event Data")]

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

        // Nếu thời gian đã trôi qua nhiều hơn `activeAfter`, sự kiện có thể xảy ra
        if (GameManager.Ins.GetElapsedTime() > activeAfter) return true;

        // Ngược lại, sự kiện chưa đủ điều kiện để xảy ra
        return false;
    }

    /// <summary>
    /// Kiểm tra xem sự kiện có xảy ra hay không dựa trên xác suất và chỉ số may mắn của người chơi
    /// </summary>
    /// <param name="s"> Thông tin chỉ số của người chơi </param>
    /// <returns> Trả về true nếu sự kiện sẽ xảy ra, false nếu không </returns>
    public bool CheckIfWillHappen(PlayerStat s)
    {
        // Nếu xác suất = 1, sự kiện luôn xảy ra
        if (probability >= 1) return true;

        // Tính toán xác suất thực tế dựa trên chỉ số may mắn của người chơi
        float luckImpact = Mathf.Max(1, (s.Stats.luck * luckFator)); // Đảm bảo không bao giờ chia cho 0
        float actualProbability = probability / luckImpact;

        // So sánh xác suất thực tế với một số ngẫu nhiên từ 0 đến 1
        if (actualProbability >= Random.Range(0f, 1f)) return true;

        // Nếu không đạt điều kiện, sự kiện không xảy ra
        return false;
    }
}

