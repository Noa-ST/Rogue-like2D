using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
    VerticalLayoutGroup verticalLayout;

    public RectTransform upgradeOptionTemplate;
    public TextMeshProUGUI tooltipTemplate;

    [Header("Settings")]
    public int maxOptions = 4;
    public string newText = "New!";
    public Color newTextColor = Color.yellow;
    public Color levelTextColor = Color.white;

    [Header("Paths")]
    public string iconPath = "Icon";
    public string namePath = "Name", descriptionPath = "Description", buttonPath = "Button", levelPath = "Level";

    RectTransform rectTransform;
    float optionHeight;
    List<RectTransform> upgradeOptions = new List<RectTransform>();
    int activeOptions = 0;
    Vector2 lastScreen;

    void Awake()
    {
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
        if (tooltipTemplate) tooltipTemplate.gameObject.SetActive(false);
        if (upgradeOptionTemplate) upgradeOptions.Add(upgradeOptionTemplate);
        rectTransform = (RectTransform)transform;
    }

    void Update()
    {
        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2(Screen.width, Screen.height);
        }
    }

    public void SetUpgrades(PlayerInventory inventory, List<ItemData> possibleUpgrades, int pick = 3, string tooltip = "")
    {
        pick = Mathf.Min(maxOptions, pick);

        if (maxOptions > upgradeOptions.Count)
        {
            for (int i = upgradeOptions.Count; i < maxOptions; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }

        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(!string.IsNullOrWhiteSpace(tooltip));

        activeOptions = 0;
        int totalPossibleUpgrades = possibleUpgrades.Count;

        foreach (RectTransform r in upgradeOptions)
        {
            if (activeOptions < pick && activeOptions < totalPossibleUpgrades)
            {
                r.gameObject.SetActive(true);
                ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selected);
                Item item = inventory.Get(selected);

                // Kiểm tra null cho từng thành phần
                Transform nameTransform = r.Find(namePath);
                if (nameTransform == null)
                {
                    Debug.LogError($"Could not find object at path: {namePath} in {r.name}");
                    continue;
                }
                TextMeshProUGUI name = nameTransform.GetComponent<TextMeshProUGUI>();
                if (name == null)
                {
                    Debug.LogError($"TextMeshProUGUI not found on object: {nameTransform.name}");
                    continue;
                }
                name.text = selected.name;

                Transform levelTransform = r.Find(levelPath);
                if (levelTransform == null)
                {
                    Debug.LogError($"Could not find object at path: {levelPath} in {r.name}");
                    continue;
                }
                TextMeshProUGUI level = levelTransform.GetComponent<TextMeshProUGUI>();
                if (level == null)
                {
                    Debug.LogError($"TextMeshProUGUI not found on object: {levelTransform.name}");
                    continue;
                }

                if (item)
                {
                    if (item.currentLevel >= item.maxLevel)
                    {
                        level.text = "Max!";
                        level.color = levelTextColor;
                    }
                    else
                    {
                        level.text = selected.GetLevelData(item.currentLevel + 1).name;
                        level.color = levelTextColor;
                    }
                }
                else
                {
                    level.text = newText;
                    level.color = newTextColor;
                }

                Transform descTransform = r.Find(descriptionPath);
                if (descTransform == null)
                {
                    Debug.LogError($"Could not find object at path: {descriptionPath} in {r.name}");
                    continue;
                }
                TextMeshProUGUI desc = descTransform.GetComponent<TextMeshProUGUI>();
                if (desc == null)
                {
                    Debug.LogError($"TextMeshProUGUI not found on object: {descTransform.name}");
                    continue;
                }
                desc.text = item != null
                    ? selected.GetLevelData(item.currentLevel + 1).description
                    : selected.GetLevelData(1).description;

                Transform iconTransform = r.Find(iconPath);
                if (iconTransform == null)
                {
                    Debug.LogError($"Could not find object at path: {iconPath} in {r.name}");
                    continue;
                }
                Image icon = iconTransform.GetComponent<Image>();
                if (icon == null)
                {
                    Debug.LogError($"Image component not found on object: {iconTransform.name}");
                    continue;
                }
                icon.sprite = selected.icon;

                Transform buttonTransform = r.Find(buttonPath);
                if (buttonTransform == null)
                {
                    Debug.LogError($"Could not find object at path: {buttonPath} in {r.name}");
                    continue;
                }
                Button b = buttonTransform.GetComponent<Button>();
                if (b == null)
                {
                    Debug.LogError($"Button component not found on object: {buttonTransform.name}");
                    continue;
                }

                b.onClick.RemoveAllListeners();
                if (item)
                    b.onClick.AddListener(() => inventory.LevelUp(item));
                else
                    b.onClick.AddListener(() => inventory.Add(selected));

                activeOptions++;
            }
            else
            {
                r.gameObject.SetActive(false);
            }
        }

        RecalculateLayout();
    }



    void RecalculateLayout()
    {
        optionHeight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom -
                       (maxOptions - 1) * verticalLayout.spacing);

        if (activeOptions == maxOptions && tooltipTemplate.gameObject.activeSelf)
            optionHeight /= maxOptions + 1;
        else optionHeight /= maxOptions;

        if (tooltipTemplate.gameObject.activeSelf)
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHeight);
            tooltipTemplate.transform.SetAsFirstSibling();
        }

        foreach (RectTransform r in upgradeOptions)
        {
            if (!r.gameObject.activeSelf) continue;
            r.sizeDelta = new Vector2(r.sizeDelta.x, optionHeight);
        }
    }

    void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
    }
}
