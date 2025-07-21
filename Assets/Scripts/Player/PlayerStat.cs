using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerStat : EntityStats
{
    // Dữ liệu của nhân vật, được truyền từ UI Character Selector
    private CharacterData _characterData;

    // Chỉ số cơ bản và chỉ số thực tế của nhân vật
    public CharacterData.Stats baseStats;
    [SerializeField] private CharacterData.Stats actualStats;

    // Thuộc tính để lấy và thiết lập chỉ số của nhân vật
    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    public CharacterData.Stats Actual
    {
        get { return actualStats; }
    }

    #region Current Stats Properties
    // Thuộc tính cho máu hiện tại của người chơi
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, actualStats.maxHealth); // Giới hạn trong maxHealth
                UpdateHealthBar();
                OnHealthChanged?.Invoke(); // Thông báo khi máu thay đổi
            }
        }
    }

    public event System.Action OnHealthChanged; // Sự kiện khi máu thay đổi
    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect;
    public ParticleSystem blockedEffect;

    // Kinh nghiệm và cấp độ của người chơi
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // Cấu trúc để xác định phạm vi cấp độ và mức tăng giới hạn kinh nghiệm
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    private float _invincibilityTimer;
    private bool _isInvincible;

    public List<LevelRange> levelRanges;

    private PlayerCollector _collector;
    private PlayerInventory _inventory;

    [Header("UI")]
    public Image healthBar;
    private Image experienceBar;
    private TMP_Text levelTxt;

    public event System.Action OnStatsChanged; // Sự kiện khi stats thay đổi

    private void Awake()
    {
        _characterData = UICharacterSelector.GetData();
        _inventory = GetComponent<PlayerInventory>();
        _collector = GetComponentInChildren<PlayerCollector>();
        if (_characterData != null)
        {
            baseStats = actualStats = _characterData.stats;
            CurrentHealth = actualStats.maxHealth; // Sử dụng thuộc tính để khởi tạo
            if (_collector != null) _collector.SetRadius(actualStats.magnet);
        }
    }

    protected override void Start()
    {
        base.Start();
        if (_characterData == null) return;
        if (_characterData.StartingWeapon == null) return;

        StartCoroutine(DelayedAddWeapon());
        experienceCap = levelRanges.Count > 0 ? levelRanges[0].experienceCapIncrease : 0;
        GameManager.Ins.AssignChosenCharacterUI(_characterData);
        SetUIReferences();
        UpdateHealthBar();
        UpdateExperienceBar();
        UpdateLevelText();
        StartCoroutine(InvincibilityCoroutine()); // Bắt đầu coroutine cho invincibility
    }

    protected override void Update()
    {
        base.Update();
        Recover();
    }

    private IEnumerator DelayedAddWeapon()
    {
        yield return null;
        if (_inventory != null && _characterData != null && _characterData.StartingWeapon != null)
        {
            _inventory.Add(_characterData.StartingWeapon);
        }
    }

    public void SetUIReferences()
    {
        if (GameManager.Ins != null)
        {
            experienceBar = GameManager.Ins.experienceBar;
            levelTxt = GameManager.Ins.levelTxt;
            if (experienceBar == null) Debug.LogWarning("experienceBar is null in PlayerStat!");
            if (levelTxt == null) Debug.LogWarning("levelTxt is null in PlayerStat!");
        }
        else
        {
            Debug.LogWarning("GameManager.Ins is null!");
        }
    }

    public override void RecalculateStats()
    {
        actualStats = baseStats;

        if (_inventory != null)
        {
            foreach (PlayerInventory.Slot s in _inventory.passiveSlots)
            {
                if (s.item is Passive p)
                {
                    actualStats += p.GetBoosts();
                }
            }
        }

        CharacterData.Stats multiplier = new CharacterData.Stats
        {
            maxHealth = 1f,
            recovery = 1f,
            armor = 1f,
            moveSpeed = 1f,
            might = 1f,
            area = 1f,
            speed = 1f,
            duration = 1f,
            amount = 1,
            cooldown = 1f,
            luck = 1f,
            growth = 1f,
            greed = 1f,
            curse = 1f,
            magnet = 1f,
            revival = 1
        };

        if (activeBuffs != null) // Kiểm tra null
        {
            foreach (Buff b in activeBuffs)
            {
                BuffData.Stats bd = b.GetData();
                switch (bd.modifierType)
                {
                    case BuffData.ModifierType.additive:
                        actualStats += bd.playerModifier;
                        break;
                    case BuffData.ModifierType.multiplicative:
                        multiplier *= bd.playerModifier;
                        break;
                }
            }
        }
        actualStats *= multiplier;

        if (_collector != null) _collector.SetRadius(actualStats.magnet);
        // Điều chỉnh CurrentHealth nếu vượt quá maxHealth mới
        if (CurrentHealth > actualStats.maxHealth)
        {
            CurrentHealth = actualStats.maxHealth;
        }
        OnStatsChanged?.Invoke(); // Thông báo thay đổi
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExperienceBar();
    }

    private void LevelUpChecker()
    {
        while (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (var range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            GameManager.Ins.StartLevelUp();
            if (AudioController.Ins != null)
            {
                AudioController.Ins.PlayLevelUpSound(); 
            }
        }
    }

    private void UpdateExperienceBar()
    {
        if (experienceBar != null)
        {
            experienceBar.fillAmount = (float)experience / experienceCap;
        }
    }

    private void UpdateLevelText()
    {
        if (levelTxt != null)
        {
            levelTxt.text = "LEVEL " + level.ToString();
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (!_isInvincible)
        {
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                CurrentHealth -= dmg;
                if (damageEffect != null)
                    Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
                if (AudioController.Ins != null)
                {
                    AudioController.Ins.PlayDamageTakenSound(); 
                }
                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else if (blockedEffect != null)
            {
                Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            _invincibilityTimer = invincibilityDuration;
            _isInvincible = true;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
        }
    }

    public override void Kill()
    {
        if (!GameManager.Ins.isGameOver)
        {
            GameManager.Ins.AssignLevelReachedUI(level);
            GameManager.Ins.GameOver();
        }
    }

    public override void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    private void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        while (true)
        {
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    _isInvincible = false;
                }
            }
            yield return null;
        }
    }
}