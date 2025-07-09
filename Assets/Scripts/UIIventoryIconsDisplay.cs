using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LayoutGroup))]
public class UIIventoryIconsDisplay : MonoBehaviour
{
    public GameObject slotTemplate;
    public uint maxSlot = 6;
    public bool showLevels = true;
    [SerializeField] private PlayerInventory inventory; 
    public GameObject[] slots;

    [Header("Path")]
    public string iconPath;
    public string levelTextPath;
    [HideInInspector] public string targetedItemList;

    private void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        // Không sử dụng FindObjectOfType ở đây, để GameManager gán
    }

    private void OnEnable()
    {
        //UpdatePlayerReference(); // Loại bỏ vì sẽ được gọi từ GameManager
        Refresh();
    }

    public void SetInventory(PlayerInventory playerInventory)
    {
        inventory = playerInventory;
        if (inventory != null)
        {
            Debug.Log("UIIventoryIconsDisplay inventory set to: " + inventory.name);
        }
        else
        {
            Debug.LogWarning("Attempted to set null inventory for UIIventoryIconsDisplay!");
        }
        Refresh();
    }

    public void Refresh()
    {
        if (inventory == null)
        {
            PlayerInventory newInventory = FindObjectOfType<PlayerInventory>();
            if (newInventory != null)
            {
                SetInventory(newInventory);
            }
            else
                return;
        }

        Type t = typeof(PlayerInventory);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        if (field == null)
        {
            return;
        }

        List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);

        for (int i = 0; i < Mathf.Min(items.Count, slots.Length); i++) // Giới hạn bởi số slot UI
        {
            Item item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);
            if (iconObj)
            {
                Image icon = iconObj.GetComponentInChildren<Image>();
                if (icon != null)
                {
                    if (!item) icon.color = new Color(1, 1, 1, 0);
                    else
                    {
                        icon.color = new Color(1, 1, 1, 1);
                        icon.sprite = item.data.icon;
                    }
                }
            }

            Transform levelObj = slots[i].transform.Find(levelTextPath);
            if (levelObj)
            {
                TextMeshProUGUI levelTxt = levelObj.GetComponentInChildren<TextMeshProUGUI>();
                if (levelTxt != null)
                {
                    if (!item || !showLevels) levelTxt.text = "";
                    else levelTxt.text = item.currentLevel.ToString();
                }
            }
        }
    }
}