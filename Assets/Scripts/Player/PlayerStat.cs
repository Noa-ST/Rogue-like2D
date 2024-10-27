using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    CharacterScriptObject _characterData;

    // Các chỉ số hiện tại của nhân vật

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
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

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
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }

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

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            currentHealth -= dmg;
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
            if (currentHealth <= 0)
            {
                Kill();
            }
        }    
    }

    private void Kill()
    {
        Debug.Log("Player isdead");
    }

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

    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnWeapon.transform.SetParent(transform);
        spawnedWeapons.Add(spawnWeapon);
    }
}
