using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;
        public void Assign(Item assignedItem)
        {
            item = assignedItem;

            if (item is Weapon)
            {
                Weapon w = item as Weapon;

                if (image == null)
                {
                    Debug.LogError("Slot image is null. Ensure the image is assigned in the Inspector.");
                    return;
                }

                if (w.data == null)
                {
                    Debug.LogError("Weapon data is null. Ensure the Weapon is initialized properly.");
                    return;
                }

                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else if (item is Passive)
            {
                Passive p = item as Passive;

                if (p == null)
                {
                    Debug.LogError("Assigned item is not a Passive. Check the item being assigned.");
                    return;
                }

                if (image == null)
                {
                    Debug.LogError("Slot image is null. Ensure the image is assigned in the Inspector.");
                    return;
                }

                if (p.data == null)
                {
                    Debug.LogError("Passive data is null. Ensure the Passive is initialized with valid data.");
                    return;
                }

                if (p.data.icon == null)
                {
                    Debug.LogWarning($"Icon is null for {p.data.name}. Check the PassiveData asset.");
                    return;
                }

                image.enabled = true;
                image.sprite = p.data.icon;
            }
            else
            {
                Debug.LogError("Assigned item is not of a known type (Weapon or Passive).");
            }

            Debug.Log($"Assigned {item.name} to player.");
        }


        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return item == null; }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();

    PlayerStat _player;
    public UIUpgradeWindow upgradeWindow;

    private void Start()
    {
        _player = GetComponent<PlayerStat>();
    }

    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData)
            return GetWeapon(type as WeaponData);
        else if (type is PassiveData)
            return GetPassive(type as PassiveData);
        return null;
    }

    public Weapon GetWeapon(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
                return w;
        }
        return null;
    }

    public Passive GetPassive(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p && p.data == type)
                return p;
        }
        return null;
    }


    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnEquip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Passive p = weaponSlots[i].item as Passive;
            if (p.data == data)
            {
                weaponSlots[i].Clear();
                p.OnUnEquip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }

    public int Add(WeaponData data)
    {
        if (data == null)
        {
            Debug.LogWarning("WeaponData is null. Cannot add weapon.");
            return -1;
        }

        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0)
        {
            Debug.LogWarning("No available slot to add weapon.");
            return -1;
        }

        Type weaponType = Type.GetType(data.behavior);

        if (weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);

            if (spawnedWeapon == null)
            {
                Debug.LogWarning("Failed to create weapon of type: " + data.behavior);
                return -1;
            }

            Debug.LogWarning(weaponType);
            Debug.LogWarning(data.baseStats.name);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Intitalise(data);

            // Kiểm tra trước khi gọi OnEquip
            if (data != null)
            {
                spawnedWeapon.OnEquip();
            }
            else
            {
                Debug.LogWarning("WeaponData not properly assigned before OnEquip.");
            }

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
                GameManager.Ins.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning($"Invalid weapon type specified for {data.name}. Check behavior: {data.behavior}");
        }

        return -1;
    }


    public int Add(PassiveData data)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);

        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            GameManager.Ins.EndLevelUp();

        _player.RecalculateStats();

        return slotNum;
    }

    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;
    }

    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if (item) return LevelUp(item);
        return false;
    }

    public bool LevelUp(Item item)
    {
        if (!item.DoLevelUp())
        {
            UnityEngine.Debug.LogWarning(string.Format("Failed to level up {0}.", item.name));
            return false;
        }

        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
        {
            GameManager.Ins.EndLevelUp();
        }

        if (item is Passive) _player.RecalculateStats();
        return true;
    }

    int GetSLotsLeft(List<Slot> slots)
    {
        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    void ApplyUpgradeOptions()
    {
        List<ItemData> availbleUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        int weaponSlotLeft = GetSLotsLeft(weaponSlots);
        int passiveSlotLeft = GetSLotsLeft(passiveSlots);

        foreach (ItemData data in allUpgrades)
        {
            Item obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel) availbleUpgrades.Add(data);
            }
            else
            {
                if (data is WeaponData && weaponSlotLeft > 0) availbleUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotLeft > 0) availbleUpgrades.Add(data);
            }
        }

        int availUpgradeCount = availbleUpgrades.Count;
        if (availUpgradeCount > 0)
        {
            bool getExtraItem = 1f - 1f / _player.Stats.luck > UnityEngine.Random.value;

            if (getExtraItem || availUpgradeCount < 4) upgradeWindow.SetUpgrades(this, availbleUpgrades, 4);
            else
                upgradeWindow.SetUpgrades(this, availbleUpgrades, 3, "Increase your Luck stat for a chance to get 4 items!");
        }
        else if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            GameManager.Ins.EndLevelUp();

    }

    public void RemoveAndApplyUpgrades()
    {
        ApplyUpgradeOptions();
    }
}
