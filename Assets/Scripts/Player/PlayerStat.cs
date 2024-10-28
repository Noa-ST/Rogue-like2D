using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    CharacterScriptObject _characterData;

    // Các chỉ số hiện tại của nhân vật
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

    // spawn vũ khí 
    public List<GameObject> spawnedWeapons;

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

    private void Awake()
    {
        _characterData = CharacterSelector.GetData();
        CharacterSelector.instance.destroySingleTon();

        // Khởi tạo chỉ số hiện tại từ ScriptableObject characterData
        currentHealth = _characterData.MaxHealth;
        currentRecovery = _characterData.Recovery;
        currentMoveSpeed = _characterData.MoveSpeed;
        currentMight = _characterData.Might;
        currentProjectileSpeed = _characterData.ProjectTileSpeed;
        currentMagnet = _characterData.Magnet;

        SpawnWeapon(_characterData.StartingWeapon);
    }

    private void Start()
    {
        // Thiết lập giới hạn kinh nghiệm ban đầu từ phạm vi cấp độ đầu tiên
        experienceCap = levelRanges[0].experienceCapIncrease;
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
        }
    }

    // Hàm nhận sát thương
    public void TakeDamage(float dmg)
    {
        if (!_isInvincible)
        {
            currentHealth -= dmg;
            _invincibilityTimer = invincibilityDuration;
            _isInvincible = true;
            if (currentHealth <= 0)
            {
                Kill();
            }
        }    
    }

    // Hàm xử lý khi nhân vật chết
    private void Kill()
    {
        Debug.Log("Player isdead");
    }

    // Hàm phục hồi máu khi nhận vật phẩm 
    public void Restore(int healthToRestore)
    {
        if (currentHealth < _characterData.MaxHealth)
        {
            currentHealth += healthToRestore;
            if (currentHealth > _characterData.MaxHealth)
            {
                currentHealth = _characterData.MaxHealth;
            }
        }
    }

    // Hàm phục hồi máu dần dần theo thời gian
    void Recover()
    {
        if (currentHealth < _characterData.MaxHealth)
        {
            {
                currentHealth += currentRecovery * Time.deltaTime;

                if (currentHealth > _characterData.MaxHealth)
                {
                    currentHealth = _characterData.MaxHealth;
                }
            }
        }
    }

    // Hàm spawn vũ khí cho người chơi
    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnWeapon.transform.SetParent(transform);
        spawnedWeapons.Add(spawnWeapon);
    }
}
