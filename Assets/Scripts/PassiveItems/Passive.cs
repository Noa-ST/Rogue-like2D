using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public struct Modifier
    {
        public string name, desription;
        public CharacterData.Stats boosts;
    }

    public override void Initialise(ItemData data)
    {
        base.Initialise(data);

        if (data is PassiveData passiveData)
        {
            this.data = passiveData; // Gán giá trị cho Passive.data
            currentBoosts = passiveData.baseStats.boosts; // Khởi tạo boosts nếu cần
        }
        else
        {
            Debug.LogError("Invalid data type passed to Passive.Initialise. Expected PassiveData.");
        }
    }


    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached ", name, currentLevel, data.maxLevel));
            return false;
        }

        currentBoosts += data.GetLevelData(++currentLevel).boosts;
        return true;
    }
}
