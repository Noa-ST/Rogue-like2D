using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStat : MonoBehaviour
{
    CharacterScriptObject _characterData;

    // Các chỉ số hiện tại của nhân vật
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties

    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get
        {
            return currentRecovery;
        }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            return currentMoveSpeed;
        }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
            }
        }
    }
    public float CurrentMight
    {
        get
        {
            return currentMight;
        }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMightDisplay.text = "Might: " + currentMight;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get
        {
            return currentProjectileSpeed;
        }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get
        {
            return currentMagnet;
        }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.curMagnetDisplay.text = "Magnet: " + currentMagnet;
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

    InventoryManager _inventory;
    public int weaponIndex;
    public int passiveitemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image experienceBar;
    public TMP_Text levelTxt;

    private void Awake()
    {
        _characterData = CharacterSelector.GetData();
        CharacterSelector.instance.destroySingleTon();

        _inventory = GetComponent<InventoryManager>();

        // Khởi tạo chỉ số hiện tại từ ScriptableObject characterData
        CurrentHealth = _characterData.MaxHealth;
        CurrentRecovery = _characterData.Recovery;
        CurrentMoveSpeed = _characterData.MoveSpeed;
        CurrentMight = _characterData.Might;
        CurrentProjectileSpeed = _characterData.ProjectTileSpeed;
        CurrentMagnet = _characterData.Magnet;

        SpawnWeapon(_characterData.StartingWeapon);
    }

    private void Start()
    {
        // Thiết lập giới hạn kinh nghiệm ban đầu từ phạm vi cấp độ đầu tiên
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.Ins.curHealthDisplay.text = "Health: " + currentHealth;
        GameManager.Ins.curRecoveryDisplay.text = "Recovery: " + currentRecovery;
        GameManager.Ins.curMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        GameManager.Ins.curMightDisplay.text = "Might: " + currentMight;
        GameManager.Ins.curProjectileSpeedDisplay.text = "Projectil Speed: " + currentProjectileSpeed;
        GameManager.Ins.curMagnetDisplay.text = "Magnet: " + currentMagnet;

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
        experienceBar.fillAmount = (float) experience / experienceCap;
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
                Instantiate(damageEffect, transform.position, Quaternion.identity);

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
        healthBar.fillAmount = currentHealth / _characterData.MaxHealth;
    }


    // Hàm xử lý khi nhân vật chết
    private void Kill()
    {
        if (!GameManager.Ins.isGameOver)
        {
            GameManager.Ins.AssignLevelReachedUI(level);
            GameManager.Ins.AssignChosenWeaponAndPassiveItemUI(_inventory.weaponUISlots, _inventory.passiveItemUISlots);
            GameManager.Ins.GameOver();
        }
    }

    // Hàm phục hồi máu khi nhận vật phẩm 
    public void Restore(int healthToRestore)
    {
        if (CurrentHealth < _characterData.MaxHealth)
        {
            CurrentHealth += healthToRestore;
            if (CurrentHealth > _characterData.MaxHealth)
            {
                CurrentHealth = _characterData.MaxHealth;
            }
        }
    }

    // Hàm phục hồi máu dần dần theo thời gian
    void Recover()
    {
        if (CurrentHealth < _characterData.MaxHealth)
        {
            {
                CurrentHealth += CurrentRecovery * Time.deltaTime;

                if (CurrentHealth > _characterData.MaxHealth)
                {
                    CurrentHealth = _characterData.MaxHealth;
                }
            }
        }
    }

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
        _inventory.AddWeapon(weaponIndex, spawnWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveitemIndex >= _inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        _inventory.AddPassiveItem(passiveitemIndex, spawnedPassiveItem.GetComponent<PassiveItems>());
        passiveitemIndex++;
    }
}
