using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance; // Biến tĩnh để lưu singleton instance của CharacterSelector
    public CharacterData characterData;

    private void Awake()
    {
        // Kiểm tra nếu chưa có instance nào
        if (instance == null)
        {
            instance = this; // Gán instance hiện tại cho biến tĩnh
            DontDestroyOnLoad(gameObject); // Giữ đối tượng này không bị hủy khi chuyển cảnh
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    // Phương thức tĩnh để lấy dữ liệu của nhân vật hiện tại
    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
            return instance.characterData;
        else
        {
#if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();

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

            if (characters.Count > 0)
                return characters[Random.Range(0, characters.Count)];
#endif

            //CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            //if (characters.Length > 0) 
            //{
            //    return characters[Random.Range(0, characters.Length)];
            //}
        }
        return null;
    }

    // Phương thức để chọn một nhân vật mới
    public void SelectCharacter(CharacterData character)
    {
        if (character == null)
        {
            Debug.LogError("Selected character is null!");
            return;
        }

        characterData = character;
        Debug.Log($"Character selected: {character.name}");
    }


    // Phương thức để hủy singleton và đối tượng
    public void DestroySingleTon()
    {
        instance = null; // Đặt instance thành null
        Destroy(gameObject); // Hủy đối tượng
    }
}
