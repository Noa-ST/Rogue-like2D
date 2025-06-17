using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Lớp trừu tượng quản lý chỉ số và buff của thực thể trong game.
/// Cung cấp cơ chế áp dụng, loại bỏ buff và cập nhật các chỉ số.
/// </summary>
public abstract class EntityStats : MonoBehaviour
{
    protected float health; // Chỉ số máu 
    protected SpriteRenderer sprite; 
    protected Animator aminator; 
    protected Color originalColor; // Lưu trữ màu gốc của thực thể
    protected List<Color> appliedTints = new List<Color>(); // Danh sách màu sắc được áp dụng do buff
    protected const float TINT_FACTOR = 4f; // Hệ số điều chỉnh màu khi bị ảnh hưởng bởi buff

    /// <summary>
    /// Lớp Buff đại diện cho các hiệu ứng tạm thời tác động lên thực thể.
    /// </summary>
    [System.Serializable]
    public class Buff
    {
        public BuffData data;
        public float remainingDuration, nextTick; // Thời gian còn lại của buff và thời gian tick tiếp theo
        public int variant; // Phiên bản của buff (có thể có nhiều mức độ khác nhau)

        public ParticleSystem effect; // Hiệu ứng hình ảnh của buff
        public Color tint; // Màu áp dụng khi buff có hiệu ứng màu
        public float animationSpeed = 1f; // Tốc độ hoạt ảnh khi có buff

        /// <summary>
        /// Khởi tạo buff với dữ liệu, chủ sở hữu, phiên bản và hệ số thời gian.
        /// </summary>
        public Buff(BuffData d, EntityStats owner, int variant = 0, float durationMultiplier = 1f)
        {
            data = d; // Lưu trữ dữ liệu buff
            BuffData.Stats buffStats = d.Get(variant); // Lấy thông tin buff tương ứng
            remainingDuration = buffStats.duration * durationMultiplier; // Cập nhật thời gian tồn tại
            nextTick = buffStats.tickInterval; // Thiết lập thời gian tick tiếp theo
            this.variant = variant; // Gán phiên bản buff

            if (buffStats.effect) effect = Instantiate(buffStats.effect, owner.transform); // Tạo hiệu ứng nếu có
            if (buffStats.tint.a > 0) // Nếu buff có màu tint
            {
                tint = buffStats.tint;
                owner.ApplyTint(buffStats.tint); // Áp dụng màu lên thực thể
            }

            animationSpeed = buffStats.animationSpeed; // Gán tốc độ hoạt ảnh
            owner.ApplyAnimationMutiplier(animationSpeed); // Áp dụng hiệu ứng lên hoạt ảnh
        }

        // Trả về dữ liệu buff tương ứng với phiên bản hiện tại
        public BuffData.Stats GetData()
        {
            return data.Get(variant);
        }

        public void Dispose(EntityStats owner)
        {
            if (effect != null)
            {
                effect.Stop(); // Dừng ParticleSystem trước khi hủy
                GameObject.Destroy(effect.gameObject);
                Debug.Log($"Hủy ParticleSystem của buff {data.name}");
            }
            if (tint.a > 0)
            {
                owner.RemoveTint(tint);
                Debug.Log($"Xóa tint {tint} của buff {data.name}");
            }
            owner.RemoveAnimationMutiplier(animationSpeed);
        }
    }

    // Danh sách các buff hiện tại trên thực thể
    protected List<Buff> activeBuffs = new List<Buff>();

    protected virtual void Start()
    {
        sprite = GetComponent<SpriteRenderer>(); 
        originalColor = sprite.color; // Lưu màu gốc
        aminator = GetComponent<Animator>();
    }

    [System.Serializable]
    public class BuffInfo
    {
        public BuffData data;
        public int variant;
        [Range(0f, 1f)] public float probability = 1f;
    }

    public virtual void ApplyAnimationMutiplier(float factor)
    {
        aminator.speed *= Mathf.Max(0.01f, factor);
    }

    public virtual void RemoveAnimationMutiplier(float factor)
    {
        aminator.speed /= Mathf.Max(0.01f, factor);
    }

    // Áp dụng màu sắc khi buff ảnh hưởng đến thực thể
    public virtual void ApplyTint(Color c)
    {
        appliedTints.Add(c);
        UpdateColor();
    }

    public virtual void RemoveTint(Color c)
    {
        if (appliedTints.Remove(c))
        {
            Debug.Log($"Đã xóa tint {c} khỏi appliedTints. Số tint còn lại: {appliedTints.Count}");
            UpdateColor();
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy tint {c} trong appliedTints");
        }
    }

    private void UpdateColor()
    {
        if (sprite == null) return;

        Color targetColor = originalColor;
        float totalWeight = 1f;
        foreach (Color c in appliedTints)
        {
            targetColor += c * c.a * TINT_FACTOR;
            totalWeight += c.a * TINT_FACTOR;
        }
        sprite.color = targetColor / totalWeight;
        Debug.Log($"Cập nhật màu sprite: {sprite.color}, appliedTints: {appliedTints.Count}");
    }


    // <summary>
    /// Kiểm tra xem thực thể có buff nhất định không.
    /// </summary>
    public virtual Buff GetBuff(BuffData data, int variant = -1)
    {
        foreach (Buff b in activeBuffs)
        {
            if (b.data == data)
            {
                if (variant >= 0)
                {
                    if (b.variant == variant) return b;

                }
                else
                {
                    return b;
                }
            }
        }
        return null;
    }

    public virtual bool ApplyBuff(BuffInfo info, float durationMultiplier = 1f)
    {
        if (Random.value <= info.probability)
            return ApplyBuff(info.data, info.variant, durationMultiplier);
        return false;
    }

    /// <summary>
    /// Áp dụng buff lên thực thể theo cơ chế stack khác nhau.
    /// </summary>
    public virtual bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1f)
    {
        Buff b;
        // Lấy thông tin buff (s) từ BuffData theo phiên bản (variant).
        BuffData.Stats s = data.Get(variant);

        switch (s.stackType)
        {
            case BuffData.StackType.stacksFully: // Cho phép stack nhiều buff giống nhau
                activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                RecalculateStats();
                break;

            case BuffData.StackType.refreshDurationOnly: // Chỉ làm mới thời gian của buff hiện có
                b = GetBuff(data, variant);
                if (b != null)

                    b.remainingDuration = s.duration * durationMultiplier;
                else
                {
                    activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                    RecalculateStats();
                }
                break;

            case BuffData.StackType.doesNotStack: // Không cho phép stack buff
                if (GetBuff(data, variant) != null)
                    return false; // Nếu buff đã tồn tại, không làm gì cả
                activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                RecalculateStats();
                return true;
        }
        return false;
    }

    /// <summary>
    /// Loại bỏ buff khỏi thực thể.
    /// </summary>
    public virtual bool RemoveBuff(BuffData data, int variant = -1)
    {
        List<Buff> toRemove = new List<Buff>();
        foreach (Buff b in activeBuffs)
        {
            if (b.data == data)
            {
                if (variant >= 0)
                {
                    if (b.variant == variant) toRemove.Add(b);
                }
                else
                {
                    toRemove.Add(b);
                }
            }
        }

        if (toRemove.Count > 0)
        {
            activeBuffs.RemoveAll(item => toRemove.Contains(item));
            RecalculateStats();
            return true;
        }
        return false;

    }




    // Các phương thức trừu tượng cần được lớp con hiện thực hóa
    public abstract void TakeDamage(float dmg); // Gây sát thương lên thực thể
    public abstract void RestoreHealth(float amount); // Hồi máu
    public abstract void Kill(); // Hủy thực thể
    public abstract void RecalculateStats(); // Cập nhật lại chỉ số

    // Hàm Update gọi mỗi frame để xử lý buff
    protected virtual void Update()
    {
        List<Buff> expired = new List<Buff>();
        foreach (Buff b in activeBuffs.ToList()) // Sao chép danh sách để tránh lỗi sửa đổi khi lặp
        {
            BuffData.Stats s = b.data.Get(b.variant);
            b.nextTick -= Time.deltaTime;
            if (b.nextTick < 0)
            {
                float tickDmg = b.data.GetTickDamage(b.variant);
                if (tickDmg > 0) TakeDamage(tickDmg);
                float tickHeal = b.data.GetTickHeal(b.variant);
                if (tickHeal > 0) RestoreHealth(tickHeal);
                b.nextTick = s.tickInterval;
                var effect = s.attackEffect as IAttackEffect;
                effect?.Apply(this, this);
            }
            if (s.duration <= 0) continue; // Bỏ qua buff không có thời gian (vĩnh viễn)
            b.remainingDuration -= Time.deltaTime;
            if (b.remainingDuration <= 0)
            {
                expired.Add(b);
            }
        }

        foreach (Buff b in expired)
        {
            Debug.Log($"Buff {b.data.name} hết hạn, gọi Dispose");
            b.Dispose(this); // Dọn dẹp hiệu ứng
            activeBuffs.Remove(b); // Xóa buff khỏi danh sách
        }

        if (expired.Count > 0)
        {
            UpdateColor(); // Cập nhật màu sắc sau khi xóa buff
            RecalculateStats(); // Cập nhật chỉ số
        }
    }
}

