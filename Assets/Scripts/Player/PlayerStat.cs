using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStat : MonoBehaviour
{
    CharacterData _characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

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
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curHealthDisplay.text = string.Format("Health: {0} / {1}", _health, actualStats.maxHealth);
                }
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curHealthDisplay.text = string.Format("Health: {0} / {1}", _health, actualStats.maxHealth);
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }

        set { Recovery = value; }
    }

    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }

    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }

    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMightDisplay.text = "Might: " + actualStats.might;
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }

    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }

    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
            }
        }
    }
    #endregion

    public ParticleSystem damageEffect;

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
        Debug.Log(_characterData.StartingWeapon);
        baseStats = actualStats = _characterData.stats;
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

        GameManager.Ins.curHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.Ins.curRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.Ins.curMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.Ins.curMightDisplay.text = "Might: " + CurrentMight;
        GameManager.Ins.curProjectileSpeedDisplay.text = "Projectil Speed: " + CurrentProjectileSpeed;
        GameManager.Ins.curMagnetDisplay.text = "Magnet: " + CurrentMagnet;

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
            CurrentHealth -= dmg;

            if (damageEffect)
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

            _invincibilityTimer = invincibilityDuration;
            _isInvincible = true;
            if (CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
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
                CurrentHealth += CurrentRecovery * Time.deltaTime;

                if (CurrentHealth > actualStats.maxHealth)
                {
                    CurrentHealth = actualStats.maxHealth;
                }
            }
        }
    }

    [System.Obsolete("Old fuction that is kept to maintain compatibility with the InventoryManager. Will be remove soon.")]
    // Hàm spawn vũ khí cho người chơi
    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= _inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        GameObject spawnWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnWeapon.transform.SetParent(transform);
        //_inventory.AddWeapon(weaponIndex, spawnWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    }

    [System.Obsolete("No need to spawn passive items directly now.")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveitemIndex >= _inventory.passiveSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        //_inventory.AddPassiveItem(passiveitemIndex, spawnedPassiveItem.GetComponent<PassiveItems>());
        passiveitemIndex++;
    }
}
