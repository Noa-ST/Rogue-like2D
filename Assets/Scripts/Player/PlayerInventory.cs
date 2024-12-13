using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    PlayerStat _player;

    private void Start()
    {
        _player = GetComponent<PlayerStat>();
    }

    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
                return p;
        }
        return null;
    }

    public Weapon Get(Weapon type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
                return w;
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

            spawnedWeapon.Intitalise(data);
            Debug.LogWarning(weaponType);
            Debug.LogWarning(data.baseStats.name);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;

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

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}.", weapon.name));
                return;
            }
        }

        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            GameManager.Ins.EndLevelUp();
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive passive = passiveSlots[slotIndex].item as Passive;

            if (!passive.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}.", passive.name));
                return;
            }
        }

        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            GameManager.Ins.EndLevelUp();

        _player.RecalculateStats();
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponData> availableWeaponUpgrade = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveUpgrade = new List<PassiveData>(availablePassives);

        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            if (availableWeaponUpgrade.Count == 0 && availablePassiveUpgrade.Count == 0) return;

            int upgradeType;
            if (availableWeaponUpgrade.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveUpgrade.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrade[UnityEngine.Random.Range(0, availableWeaponUpgrade.Count)];
                availableWeaponUpgrade.Remove(chosenWeaponUpgrade);

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUi(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;                           
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }

             else if (upgradeType == 2)
            {
                PassiveData chosenPassiveUpgrade = availablePassiveUpgrade[UnityEngine.Random.Range(0, availablePassiveUpgrade.Count)];
                availablePassiveUpgrade.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUi(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.desription;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.desription;
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }    
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    private void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    private void EnableUpgradeUi(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
