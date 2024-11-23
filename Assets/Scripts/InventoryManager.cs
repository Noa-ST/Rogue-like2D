using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItems> passiveItemSlots = new List<PassiveItems>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemsScriptableobject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> UpgradeUIOptions = new List<UpgradeUI>();

    public List<WeaponEvolutionBluePrint> weaponEvolutions = new List<WeaponEvolutionBluePrint>();

    PlayerStat _player;

    private void Start()
    {
        _player = GetComponent<PlayerStat>();
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
        {
            GameManager.Ins.EndLevelUp();
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItems)
    {
        // Kiểm tra danh sách và đối tượng
        if (passiveItemSlots == null || slotIndex >= passiveItemSlots.Count) return;
        if (passiveItems == null || passiveItems.passiveItemData == null) return;
        // Gán giá trị
        passiveItemSlots[slotIndex] = passiveItems;
        passiveItemLevels[slotIndex] = passiveItems.passiveItemData.Level;
        // Cập nhật UI
        if (passiveItemUISlots[slotIndex] != null)
        {
            passiveItemUISlots[slotIndex].enabled = true;
            passiveItemUISlots[slotIndex].sprite = passiveItems.passiveItemData.Icon;
        }
        // Kết thúc nâng cấp nếu có
        if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
        {
            GameManager.Ins.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if (!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL FOR" + weapon.name);
            }
            GameObject upgradeWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradeWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradeWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradeWeapon.GetComponent<WeaponController>().weaponData.Level;

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradeWeapon.GetComponent<WeaponController>().weaponData;

            if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            {
                GameManager.Ins.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItems passiveItem = passiveItemSlots[slotIndex];
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL FOR" + passiveItem.name);
            }
            GameObject upgradePassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradePassiveItem.GetComponent<PassiveItems>());
            Destroy(passiveItem.gameObject);
            weaponLevels[slotIndex] = upgradePassiveItem.GetComponent<PassiveItems>().passiveItemData.Level;


            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradePassiveItem.GetComponent<PassiveItems>().passiveItemData;

            if (GameManager.Ins != null && GameManager.Ins.choosingUpgrade)
            {
                GameManager.Ins.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availabelWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availabelPassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in UpgradeUIOptions)
        {
            if (availabelWeaponUpgrades.Count == 0 && availabelWeaponUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;

            if (availabelWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availabelPassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = availabelWeaponUpgrades[Random.Range(0, availabelWeaponUpgrades.Count)];

                availabelWeaponUpgrades.Remove(chosenWeaponUpgrade);

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex));
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => _player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = availabelPassiveItemUpgrades[Random.Range(0, availabelPassiveItemUpgrades.Count)];

                availabelPassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);

                if (chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;
                            if (!newPassiveItem)
                            {
                                    if (!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                    {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                    }

                                    upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex));
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => _player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in UpgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrade()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

    public List<WeaponEvolutionBluePrint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBluePrint> possibleEvolutions = new List<WeaponEvolutionBluePrint>();

        foreach (WeaponController weapon in weaponSlots)
        {
            if (weapon != null)
            {
                foreach (PassiveItems catalyst in passiveItemSlots)
                {
                    if (catalyst != null)
                    {
                        foreach (WeaponEvolutionBluePrint evolution in weaponEvolutions)
                        {
                            if (weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPasiveItemData.Level)
                            {
                                possibleEvolutions.Add(evolution);
                            }
                        }
                    }
                }
            }
        }
        return possibleEvolutions;
    }

    public void EvolveWeapon(WeaponEvolutionBluePrint evolution)
    {
        for (int weaponSlotIndex = 0; weaponSlotIndex < weaponSlots.Count; weaponSlotIndex++)
        {
            WeaponController weapon = weaponSlots[weaponSlotIndex];

            if (!weapon)
            {
                continue;
            }

            for (int catalystSlotIndex = 0; catalystSlotIndex < passiveItemSlots.Count; catalystSlotIndex++)
            {
                PassiveItems catalyst = passiveItemSlots[catalystSlotIndex];

                if (!catalyst)
                {
                    continue;
                }

                if (weapon && catalyst && weapon.weaponData.Level  >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPasiveItemData.Level)
                {
                    GameObject evolveWeapon = Instantiate(evolution.evolveWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolveWeapon.GetComponent<WeaponController>();

                    evolveWeapon.transform.SetParent(transform);
                    AddWeapon(weaponSlotIndex, evolvedWeaponController);
                    Destroy(weapon.gameObject);

                    // update level and icon
                    weaponLevels[weaponSlotIndex] = evolvedWeaponController.weaponData.Level;
                    weaponUISlots[weaponSlotIndex].sprite = evolvedWeaponController.weaponData.Icon;

                    // update the upgrade option
                    weaponUpgradeOptions.RemoveAt(evolvedWeaponController.weaponData.EvolvedUpgradeToRemove);

                    Debug.LogWarning("Evoled");
                }
            }
        }
    }
}
