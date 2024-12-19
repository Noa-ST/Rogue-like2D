using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerStat : MonoBehaviour
{
    CharacterData _characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    float _health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get
        {
            return _health;
        }
        set
        {
            if (_health != value)
            {
                _health = value;
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

    // Lớp phụ để thiết lập phạm vi cấp độ và mức tăng giới hạn kinh nghiệm cho từng phạm vi
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease; // Mức tăng giới hạn kinh nghiệm cho phạm vi này
    }

    [Header("I-Frames")]
    public float invincibilityDuration; // Thời gian bất tử sau khi nhận sát thương
    float _invincibilityTimer; // Bộ đếm thời gian bất tử
    bool _isInvincible;

    public List<LevelRange> levelRanges;

    PlayerCollector _collector;
    PlayerInventory _inventory;
    public int weaponIndex;
    public int passiveitemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image experienceBar;
    public TMP_Text levelTxt;

    private void Awake()
    {
        if (CharacterSelector.instance == null)
        {
            Debug.LogError("CharacterSelector instance is null! Ensure the CharacterSelector GameObject is in the scene and not destroyed.");
            return;
        }

        _characterData = CharacterSelector.GetData();
        if (_characterData == null)
        {
            Debug.LogError("Character data is null! Ensure a character is selected in the menu.");
            return;
        }
        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleTon();

        _inventory = GetComponent<PlayerInventory>();
        _collector = GetComponentInChildren<PlayerCollector>();
        Debug.Log(_characterData.StartingWeapon);
        baseStats = actualStats = _characterData.stats;
        _collector.SetRadius(actualStats.magnet);
        _health = actualStats.maxHealth;
    }

    private void Start()
    {
        if (_characterData == null)
        {
            Debug.LogError("CharacterData is null in PlayerStat. Ensure it is passed correctly from CharacterSelector.");
            return;
        }

        if (_characterData.StartingWeapon == null)
        {
            Debug.LogError($"StartingWeapon is null for {_characterData.name}. Ensure the character has a starting weapon assigned.");
            return;
        }

        _inventory.Add(_characterData.StartingWeapon);
        // Thiết lập giới hạn kinh nghiệm ban đầu từ phạm vi cấp độ đầu tiên
        experienceCap = levelRanges[0].experienceCapIncrease;



        GameManager.Ins.AssignChosenCharacterUI(_characterData);

        UpdateHealthBar();
        UpdateExperienceBar();
        UpdateLevelText();
    }

    private void Update()
    {
        if (_invincibilityTimer > 0)
        {
            _invincibilityTimer -= Time.deltaTime;
        }
        else if (_isInvincible)
        {
            _isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in _inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
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

    void UpdateExperienceBar()
    {
        experienceBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelTxt.text = "LEVEL " + level.ToString();
    }

    // Hàm nhận sát thương
    public void TakeDamage(float dmg)
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

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }


    // Hàm xử lý khi nhân vật chết
    public void Kill()
    {
        if (!GameManager.Ins.isGameOver)
        {
            GameManager.Ins.AssignLevelReachedUI(level);
            GameManager.Ins.AssignChosenWeaponAndPassiveItemUI(_inventory.weaponSlots, _inventory.passiveSlots);
            GameManager.Ins.GameOver();
        }
    }

    // Hàm phục hồi máu khi nhận vật phẩm 
    public void Restore(float amount)
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
