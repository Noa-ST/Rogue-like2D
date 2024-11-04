using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItems> passiveItemSlots = new List<PassiveItems>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItems)
    {
        passiveItemSlots[slotIndex] = passiveItems;
        passiveItemLevels[slotIndex] = passiveItems.passiveitemData.Level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItems.passiveitemData.Icon;
    }

    public void LevelUpWeapon(int slotIndex)
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
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItems passiveItem = passiveItemSlots[slotIndex];
            if (!passiveItem.passiveitemData.NextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL FOR" + passiveItem.name);
            }
            GameObject upgradePassiveItem = Instantiate(passiveItem.passiveitemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradePassiveItem.GetComponent<PassiveItems>());
            Destroy(passiveItem.gameObject);
            weaponLevels[slotIndex] = upgradePassiveItem.GetComponent<PassiveItems>().passiveitemData.Level;
        }
    }
}
