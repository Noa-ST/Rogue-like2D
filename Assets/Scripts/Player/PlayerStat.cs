using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public CharacterScriptObject characterData;

    // Các chỉ số hiện tại của nhân vật
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;

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
        // Khởi tạo chỉ số hiện tại từ ScriptableObject characterData
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectTileSpeed;
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

    internal void Restore(int healthToRestore)
    {
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += healthToRestore;
            if (currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }
}
