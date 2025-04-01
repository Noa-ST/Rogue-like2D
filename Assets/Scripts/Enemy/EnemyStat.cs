using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStat : EntityStats
{
    [System.Serializable]
    public struct Resistances
    {
        [Range(-1f, 1f)] public float freeze, kill, debuff;

        public static Resistances operator *(Resistances r, float factor)
        {
            r.freeze = Mathf.Min(1, r.freeze * factor);
            r.kill = Mathf.Min(1, r.kill * factor);
            r.debuff = Mathf.Min(1, r.debuff * factor);
            return r;
        }

        public static Resistances operator +(Resistances r, Resistances r2)
        {
            r.freeze += r2.freeze;
            r.kill = r2.kill;
            r.debuff = r2.debuff;
            return r;
        }

        public static Resistances operator *(Resistances r1, Resistances r2)
        {
            r1.freeze = Mathf.Min(1, r1.freeze * r2.freeze);
            r1.kill = Mathf.Min(1, r1.kill * r2.kill);
            r1.debuff = Mathf.Min(1, r1.debuff * r2.debuff);
            return r1;
        }
    }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, moveSpeed, damage;
        public float knockbackMultiplier;
        public Resistances resistances;

        [System.Flags]
        public enum Boostable { health = 1, moveSpeed = 2, damage = 4, knockbackMultiplier = 8, resistances = 16 }
        public Boostable curseBoosts, levelBoosts;

        private static Stats Boost(Stats s1, float factor, Boostable boostable)
        {
            if ((boostable & Boostable.health) != 0) s1.maxHealth = factor;

            if ((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed = factor;

            if ((boostable & Boostable.damage) != 0) s1.damage *= factor;

            if ((boostable & Boostable.knockbackMultiplier) != 0) s1.knockbackMultiplier /= factor;

            if ((boostable & Boostable.resistances) != 0) s1.resistances *= factor;
            return s1;
        }

        public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }

        public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.moveSpeed += s2.moveSpeed;
            s1.damage += s2.maxHealth;
            s1.knockbackMultiplier += s2.knockbackMultiplier;
            s1.resistances += s2.resistances;
            return s1;
        }

        public static Stats operator *(Stats s1, Stats s2)
        {
            s1.maxHealth *= s2.maxHealth;
            s1.moveSpeed *= s2.moveSpeed;
            s1.damage *= s2.maxHealth;
            s1.knockbackMultiplier *= s2.knockbackMultiplier;
            s1.resistances *= s2.resistances;
            return s1;
        }
    }

    public Stats baseStats = new Stats { maxHealth = 10, moveSpeed = 1, damage = 3, knockbackMultiplier = 1 };
    Stats actualStats;
    public Stats Actual
    {
        get { return actualStats; }
    }

    public BuffInfo[] attackEffects;

    Transform _player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float damageFadeTime = 0.6f;
    EnemyMovement _em;

    public static int count;

    public float deathFadeTime;

    void Awake()
    {
        count++;
    }

    protected override void Start()
    {
        base.Start();
        RecalculateStats();
        health = actualStats.maxHealth;
        _em = GetComponent<EnemyMovement>();
    }

    public override bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1)
    {
        if ((data.type & BuffData.Type.freeze) > 0)
            if (Random.value <= Actual.resistances.freeze) return false;

        if ((data.type & BuffData.Type.debuff) > 0)
            if (Random.value <= Actual.resistances.debuff) return false;

        return base.ApplyBuff(data, variant, durationMultiplier);
    }

    public override void RecalculateStats()
    {
        foreach (Buff b in activeBuffs)
        {
            actualStats += b.GetData().enemyModifier;
        }

        float curse = GameManager.GetCumulativeCurse(),
              level = GameManager.GetCumulativeLevels();
        actualStats = (baseStats * curse) ^ level;

        Stats multiplier = new Stats
        {
            maxHealth = 1f,
            moveSpeed = 1f,
            damage = 1f,
            knockbackMultiplier = 1,
            resistances = new Resistances { freeze = 1f, debuff = 1f, kill = 1f }
        };

        foreach (Buff b in activeBuffs)
        {
            BuffData.Stats bd = b.GetData();
            switch (bd.modifierType)
            {
                case BuffData.ModifierType.additive:
                    actualStats += bd.enemyModifier;
                    break;
                case BuffData.ModifierType.multiplicative:
                    multiplier *= bd.enemyModifier;
                    break;
            }
        }

        actualStats *= multiplier;
    }
    
    public override void TakeDamage(float dmg)
    {
        health -= dmg;

        if (dmg == actualStats.maxHealth)
        {
            if (Random.value < actualStats.resistances.kill) return;
        }

        if (dmg > 0)
        {
            StartCoroutine(DamageFlash());
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }

        if (health <= 0)
        {
            Kill();
        }

    }

    public void TakeDamage(float dmg, Vector2 sourcePostion, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        TakeDamage(dmg);

        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePostion;
            _em.KnockBack(dir.normalized * knockbackForce, knockbackDuration);
        }
    }

    public override void RestoreHealth(float amount)
    {
        if (health < actualStats.maxHealth)
        {
            health += amount;
            if (health > actualStats.maxHealth)
            {
                health = actualStats.maxHealth;
            }
        }
    }

    IEnumerator DamageFlash()
    {
        ApplyTint(damageColor);
        yield return new WaitForSeconds(damageFlashDuration);
        RemoveTint(damageColor);
    }

    public override void Kill()
    {
        DropRateManager drops = GetComponent<DropRateManager>();
        if (drops) drops.active = true;
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sprite.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Mathf.Approximately(Actual.damage, 0)) return;
        if (collision.collider.TryGetComponent(out PlayerStat p))
        {
            p.TakeDamage(Actual.damage);
            foreach (BuffInfo b in attackEffects)
            {
                p.ApplyBuff(b);
            }
        }
    }

    private void OnDestroy()
    {
        count--;
    }
}
