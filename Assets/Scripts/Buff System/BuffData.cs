using UnityEngine;

//+ Tạo một menu trong Unity để tạo đối tượng BuffData từ Editor
[CreateAssetMenu(fileName = "Buff Data", menuName = "2D Top-down Rogue-like/Buff Data")]
public class BuffData : ScriptableObject
{
    // tên của Buff
    public new string name = "New Buff";

    // Biểu tượng của buff (hiển thị trên UI hoặc HUD)
    public Sprite icon;

    // Cờ đánh dấu loại buff bằng cách sử dụng `Flags`, cho phép kết hợp nhiều loại buff cùng lúc
    [System.Flags]
    public enum Type : byte
    {
        buff = 1,        // Buff có lợi cho nhân vật
        debuff = 2,      // Debuff gây bất lợi cho nhân vật
        freeze = 4,      // Buff làm đóng băng mục tiêu
        strong = 8       // Buff giúp nhân vật mạnh hơn
    }

    // Biến lưu trữ loại buff, có thể là một hoặc nhiều loại cùng lúc
    public Type type;

    // Kiểu stack của buff khi được áp dụng nhiều lần
    public enum StackType : byte
    {
        refreshDurationOnly,  // Làm mới thời gian của buff mà không tăng thêm hiệu ứng
        stacksFully,          // Buff có thể cộng dồn đầy đủ
        doesNotStack          // Buff không thể cộng dồn
    }

    // Cách thức buff ảnh hưởng đến chỉ số nhân vật
    public enum ModifierType : byte
    {
        additive,       // Cộng trực tiếp vào giá trị gốc
        multiplicative  // Nhân lên dựa trên giá trị gốc
    }

    // Lớp chứa thông tin chi tiết về buff
    [System.Serializable]
    public class Stats
    {
        // Tên của buff ở từng cấp độ
        public string name;

        [Header("Visuals")]
        [Tooltip("Hiệu ứng được gắn vào GameObject có buff.")]
        // Hiệu ứng hình ảnh gắn vào đối tượng bị ảnh hưởng bởi buff
        public ParticleSystem effect;

        [Tooltip("Màu sắc tô điểm của các đơn vị bị ảnh hưởng bởi buff này.")]
        // Màu sắc hiển thị trên nhân vật khi bị buff hoặc debuff
        public Color tint = new Color(0, 0, 0, 0);

        [Tooltip("Buff này có làm chậm hoặc tăng tốc hoạt ảnh của GameObject bị ảnh hưởng hay không.")]
        // Tốc độ hoạt ảnh của đối tượng bị buff (ví dụ: làm chậm hoặc tăng tốc)
        public float animationSpeed = 1f;

        [Header("Stats")]
        // Thời gian tồn tại của buff (s)
        public float duration;

        // Lượng sát thương gây ra mỗi giây
        public float damagePerSecond, healPerSecond;

        [Tooltip("Kiểm soát tần suất áp dụng sát thương / hồi máu mỗi giây.")]
        // Khoảng thời gian giữa các lần tính sát thương hoặc hồi máu
        public float tickInterval = 0.25f;

        // Kiểu cộng dồn của buff
        public StackType stackType;

        // Kiểu tính toán chỉ số của buff
        public ModifierType modifierType;

        // Constructor mặc định khởi tạo giá trị ban đầu
        public Stats()
        {
            duration = 10f;        // Buff tồn tại trong 10 giây
            damagePerSecond = 1f;  // Gây 1 sát thương mỗi giây
            healPerSecond = 1f;    // Hồi 1 máu mỗi giây
            tickInterval = 0.25f;  // Cập nhật hiệu ứng mỗi 0.25 giây
        }

        // Chỉ số buff ảnh hưởng đến nhân vật (người chơi)
        public CharacterData.Stats playerModifier;

        // Chỉ số buff ảnh hưởng đến kẻ địch
        public EnemyStat.Stats enemyModifier;
    }

    // Mảng chứa các phiên bản (cấp độ) khác nhau của buff
    public Stats[] variations = new Stats[1]
    {
         new Stats { name = "Level 1" } // Mặc định có một phiên bản cấp 1
    };

    // Tính toán sát thương gây ra mỗi lần tick
    public float GetTickDamage(int variant = 0)
    {
        Stats s = Get(variant);
        return s.damagePerSecond * s.tickInterval;
    }

    // Tính toán lượng hồi máu mỗi lần tick
    public float GetTickHeal(int variant = 0)
    {
        Stats s = Get(variant);
        return s.healPerSecond * s.tickInterval;
    }

    // Lấy dữ liệu buff theo cấp độ (nếu không có thì lấy cấp thấp nhất)
    public Stats Get(int variant = -1)
    {
        return variations[Mathf.Max(0, variant)];
    }
}
