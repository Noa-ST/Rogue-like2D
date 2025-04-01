using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Thời gian chờ hiện tại trước khi sự kiện tiếp theo có thể xảy ra.
    float currentEventCooldown = 0;

    // Danh sách các sự kiện có thể xảy ra.
    public EvenData[] events;

    [Tooltip("Thời gian chờ trước khi sự kiện đầu tiên kích hoạt.")]
    public float firstTriggerDelay = 180f;

    [Tooltip("Thời gian chờ giữa các sự kiện.")]
    public float triggerInterval = 30f;

    // Singleton instance của EventManager
    public static EventManager instance;

    // Lớp Event chứa thông tin về một sự kiện cụ thể
    [System.Serializable]
    public class Event
    {
        public EvenData data; // Dữ liệu sự kiện

        public float duration; // Thời gian sự kiện diễn ra
        public float cooldown = 0; // Thời gian hồi trước khi sự kiện tiếp theo có thể xảy ra
    }

    // Danh sách các sự kiện đang chạy
    List<Event> runningEvents = new List<Event>();

    // Danh sách tất cả người chơi trong game
    PlayerStat[] allPlayers;

    void Start()
    {
        // Kiểm tra nếu có nhiều hơn một EventManager trong scene
        if (instance)
            Debug.LogWarning("Có nhiều hơn 1 EventManager trong Scene! Hãy xóa bớt.");

        // Gán instance hiện tại cho biến instance
        instance = this;

        // Thiết lập cooldown ban đầu
        currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;

        // Lấy danh sách tất cả người chơi trong scene
        allPlayers = FindObjectsOfType<PlayerStat>();
    }

    void Update()
    {
        // Giảm thời gian chờ trước khi sự kiện tiếp theo có thể xảy ra
        currentEventCooldown -= Time.deltaTime;

        if (currentEventCooldown <= 0)
        {
            // Lấy một sự kiện ngẫu nhiên và kiểm tra xem nó có xảy ra không
            EvenData e = GetRandomEvent();
            if (e && e.CheckIfWillHappen(allPlayers[Random.Range(0, allPlayers.Length)]))         
                // Thêm sự kiện vào danh sách đang chạy
                runningEvents.Add(new Event
                {
                    data = e,
                    duration = e.duration
                });
            

            // Đặt lại cooldown cho sự kiện tiếp theo
            currentEventCooldown = triggerInterval;
        }

        // Danh sách các sự kiện cần xóa
        List<Event> toRemove = new List<Event>();

        // Duyệt qua các sự kiện đang chạy
        foreach (Event e in runningEvents)
        {
            // Giảm thời gian còn lại của sự kiện
            e.duration -= Time.deltaTime;
            if (e.duration <= 0)
            {
                // Nếu sự kiện hết thời gian, đánh dấu để xóa
                toRemove.Add(e);
                continue;
            }

            // Giảm cooldown cho sự kiện hiện tại
            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                // Chọn một người chơi ngẫu nhiên và kích hoạt sự kiện
                e.data.Activate(allPlayers[Random.Range(0, allPlayers.Length)]);

                // Đặt lại cooldown dựa trên khoảng thời gian spawn của sự kiện
                e.cooldown = e.data.GetSpawnInterval();
            }
        }

        // Xóa tất cả sự kiện đã hết thời gian
        foreach (Event e in toRemove)
            runningEvents.Remove(e);
    }

    /// <summary>
    /// Lấy một sự kiện ngẫu nhiên từ danh sách
    /// </summary>
    /// <returns>Một sự kiện hợp lệ hoặc null nếu không có sự kiện nào có thể xảy ra</returns>
    public EvenData GetRandomEvent()
    {
        // Nếu không có sự kiện nào được thiết lập, trả về null
        if (events.Length <= 0) return null;

        List<EvenData> possibleEvents = new List<EvenData>();

        foreach (EvenData e in events)
        {
            if (e.IsActive())
            {
                possibleEvents.Add(e);
            }
        }

        if (possibleEvents.Count > 0)
        {
            EvenData result = possibleEvents[Random.Range(0, possibleEvents.Count)];
            return result;
        }

        return null;
    }
}


