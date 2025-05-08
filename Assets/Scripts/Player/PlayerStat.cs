using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerStat : EntityStats
{
    // Dữ liệu của nhân vật, được truyền từ UI Character Selector
    CharacterData _characterData;

    // Chỉ số cơ bản và chỉ số thực tế của nhân vật
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

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
        get
        {
            return health;
        }
        set
        {
            if (health != value)
            {
                health = value;
                // Cập nhật thanh máu khi máu thay đổi
                UpdateHealthBar();
            }
        }
    }
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
        // Mức tăng giới hạn kinh nghiệm cho phạm vi này
        public int experienceCapIncrease; 
    }

    [Header("I-Frames")]
    // Thời gian bất tử sau khi nhận sát thương
    public float invincibilityDuration;
    // đếm thời gian bất tử
    float _invincibilityTimer; 
    bool _isInvincible;

    public List<LevelRange> levelRanges;

    PlayerCollector _collector;
    PlayerInventory _inventory;

    [Header("UI")]
    public Image healthBar;
    public Image experienceBar;
    public TMP_Text levelTxt;

    private void Awake()
    {
        _characterData = UICharacterSelector.GetData();
        _inventory = GetComponent<PlayerInventory>();
        _collector = GetComponentInChildren<PlayerCollector>();
        baseStats = actualStats = _characterData.stats;
        _collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    protected override void Start()
    {
        base.Start();
        if (_characterData == null) return;      
        if (_characterData.StartingWeapon == null) return;

        // Thêm vũ khí khởi đầu vào inventory
        _inventory.Add(_characterData.StartingWeapon);
        // Thiết lập giới hạn kinh nghiệm ban đầu từ phạm vi cấp độ đầu tiên
        experienceCap = levelRanges[0].experienceCapIncrease;
        // Gán UI cho nhân vật đã chọn
        GameManager.Ins.AssignChosenCharacterUI(_characterData);

        UpdateHealthBar(); // Cập nhật thanh máu
        UpdateExperienceBar(); // Cập nhật thanh kinh nghiệm
        UpdateLevelText(); // Cập nhật thông tin cấp độ
    }

    protected override void Update()
    {
        base.Update();
        if (_invincibilityTimer > 0)
        {
            _invincibilityTimer -= Time.deltaTime;
        }
        else if (_isInvincible)
        {
            _isInvincible = false;
        }

        Recover(); // Phục hồi máu dần dần theo thời gian
    }

    // Cập nhật lại các chỉ số của nhân vật sau khi buff được áp dụng
    public override void RecalculateStats()
    {
        actualStats = baseStats;

        // Cộng dồn chỉ số từ các vật phẩm trong inventory
        foreach (PlayerInventory.Slot s in _inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                // Cộng dồn các hiệu ứng passive
                actualStats += p.GetBoosts();
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

        // Cập nhật các chỉ số từ buff
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
        actualStats *= multiplier;

        _collector.SetRadius(actualStats.magnet);
    }

    // Hàm tăng kinh nghiệm
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExperienceBar();
    }

    // Hàm kiểm tra nếu đủ kinh nghiệm để lên cấp
    private void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0; // Biến tạm để lưu mức tăng giới hạn kinh nghiệm

            // Tìm mức tăng giới hạn kinh nghiệm phù hợp với cấp độ mới
            foreach (var range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break; // Dừng tìm kiếm sau khi tìm thấy phạm vi phù hợp
                }
            }
            experienceCap += experienceCapIncrease; // Cập nhật giới hạn kinh nghiệm mới

            UpdateLevelText();
            GameManager.Ins.StartLevelUp();
            if (experience >= experienceCap)
                LevelUpChecker();
        }
    }

    // Cập nhật thanh kinh nghiệm UI
    void UpdateExperienceBar()
    {
        experienceBar.fillAmount = (float)experience / experienceCap;
    }

    // Cập nhật thông tin cấp độ UI
    void UpdateLevelText()
    {
        levelTxt.text = "LEVEL " + level.ToString();
    }

    // Hàm nhận sát thương
    public override void TakeDamage(float dmg)
    {
        if (!_isInvincible)
        {
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                CurrentHealth -= dmg;
                if (damageEffect)
                    Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                if (blockedEffect)
                    Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            _invincibilityTimer = invincibilityDuration;
            _isInvincible = true;
        }
    }

    // Cập nhật thanh máu UI
    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }


    // Hàm xử lý khi nhân vật chết
    public override void Kill()
    {
        if (!GameManager.Ins.isGameOver)
        {
            GameManager.Ins.AssignLevelReachedUI(level);

            GameManager.Ins.GameOver();
        }
    }

    // Hàm phục hồi máu khi nhận vật phẩm 
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

    // Hàm phục hồi máu dần dần theo thời gian
    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            {
                CurrentHealth += Stats.recovery * Time.deltaTime;

                if (CurrentHealth > actualStats.maxHealth)
                {
                    CurrentHealth = actualStats.maxHealth;
                }
            }
        }
    }
}
