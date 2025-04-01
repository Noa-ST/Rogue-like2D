using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public static CharacterData selected;
    public UIStatsDisplay statsUI;

    [Header("Template")]
    public Toggle toggleTemplate;
    public string characterNamePath = "Character Name";
    public string weaponIconPath = "Weapon Icon";
    public string characterIconPath = "Character Icon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("DescriptionBox")]
    public TextMeshProUGUI characterFullName;
    public TextMeshProUGUI characterDescription;
    public Image selectedCharacterIcon;
    public Image selectedCharacterWeapon;

    void Start()
    {
        if (defaultCharacter) Select(defaultCharacter);
    }

    // Phương thức tĩnh để lấy dữ liệu của nhân vật hiện tại
    public static CharacterData[] GetAllCharacterDataAssets()
    {
        List<CharacterData> characters = new List<CharacterData>();

#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                if (characterData != null)
                {
                    characters.Add(characterData);
                }
            }
        }
#else 
            Debug.LogWarning(" Chức năng không thể  gọi khi builds.");
#endif
        return characters.ToArray();
    }


    public static CharacterData GetData()
    {
        if (selected)
           return selected;
        else
        {
            CharacterData[] characters = GetAllCharacterDataAssets();
            if (characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    public void Select(CharacterData character)
    {
        selected = statsUI.character = character;
        statsUI.UpdateStatFields();
        characterFullName.text = character.FullName;
        characterDescription.text = character.CharacterDescription;
        selectedCharacterIcon.sprite = character.Icon;
        selectedCharacterWeapon.sprite =character.StartingWeapon.icon;
    }
}
