using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICharacterSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public CharacterData[] allCharacters; // Thêm danh sách để chạy được khi build
    public static CharacterData selected;
    public UIStatsDisplay statsUI;

    [Header("Template")]
    public Toggle toggleTemplate;
    public string characterNamePath = "Character Name";
    public string weaponIconPath = "Weapon Icon";
    public string characterIconPath = "Character Icon";
    public string costTextPath = "Cost Text";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("DescriptionBox")]
    public TextMeshProUGUI characterFullName;
    public TextMeshProUGUI characterDescription;
    public Image selectedCharacterIcon;
    public Image selectedCharacterWeapon;
    public Button buyButton;

    void Start()
    {
        Pref.InitializeDefaultCharacter();
        
        // Chỉ chọn nhân vật mặc định, việc mở khóa miễn phí sẽ do Pref.InitializeDefaultCharacter() đảm nhận cho nhân vật đầu tiên (ID 0)
        if (defaultCharacter) 
        {
            Select(defaultCharacter);
        }
        
        InitializeToggles();

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked); 
            buyButton.gameObject.SetActive(false); 
        }
    }

    // Khởi tạo toggle và kiểm tra trạng thái mở khóa
    private void InitializeToggles()
    {
        CharacterData[] characters = GetAllCharacterDataAssets();

        // Sắp xếp: Đã mở khóa lên đầu, sau đó theo CharacterId
        System.Array.Sort(characters, (a, b) =>
        {
            bool aUnlocked = Pref.IsCharacterUnlocked(a.CharacterId);
            bool bUnlocked = Pref.IsCharacterUnlocked(b.CharacterId);
            if (aUnlocked && !bUnlocked) return -1;
            if (!aUnlocked && bUnlocked) return 1;
            return a.CharacterId.CompareTo(b.CharacterId);
        });

        for (int i = 0; i < characters.Length; i++)
        {
            var character = characters[i];
            Toggle toggle = selectableToggles.Find(t => t.name == character.Name);
            if (toggle != null)
            {
                // Đưa các nhân vật đã sở hữu lên đầu trong UI
                toggle.transform.SetSiblingIndex(i);

                Transform lockIcon = toggle.transform.Find("Lock Icon");
                if (lockIcon != null)
                {
                    Image lockImage = lockIcon.GetComponent<Image>();
                    lockImage.enabled = !Pref.IsCharacterUnlocked(character.CharacterId);
                }
                Transform costText = toggle.transform.Find(costTextPath);
                if (costText != null && costText.TryGetComponent(out TextMeshProUGUI costTmp))
                {
                    bool isUnlocked = Pref.IsCharacterUnlocked(character.CharacterId);
                    costTmp.text = character.Cost.ToString();
                    costTmp.gameObject.SetActive(!isUnlocked);
                }
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy Toggle cho nhân vật: {character.Name}. Hãy đảm bảo tên GameObject của Toggle trùng với thuộc tính Name của CharacterData.");
            }
        }
    }

    // Phương thức tĩnh để lấy dữ liệu của nhân vật hiện tại
    public static CharacterData[] GetAllCharacterDataAssets()
    {
        UICharacterSelector instance = FindObjectOfType<UICharacterSelector>();
        if (instance != null && instance.allCharacters != null && instance.allCharacters.Length > 0)
        {
            return instance.allCharacters;
        }

        List<CharacterData> characters = new List<CharacterData>();
#if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:CharacterData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (data != null) characters.Add(data);
        }
#else 
        Debug.LogWarning("Chức năng không thể gọi khi builds. Hãy gán allCharacters trong Inspector.");
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
        if (character == null) { Debug.LogWarning("CharacterData được chọn là null!"); return; }

        // Cập nhật UI ngay cả khi nhân vật bị khóa
        selected = statsUI.character = character;
        if (statsUI != null) statsUI.UpdateStatFields();

        if (characterFullName) characterFullName.text = character.FullName;
        if (characterDescription) characterDescription.text = character.CharacterDescription;
        if (selectedCharacterIcon) selectedCharacterIcon.sprite = character.Icon;
        if (selectedCharacterWeapon && character.StartingWeapon) selectedCharacterWeapon.sprite = character.StartingWeapon.icon;

        // Hiển thị hoặc ẩn nút Buy dựa trên trạng thái mở khóa
        if (buyButton != null)
        {
            buyButton.gameObject.SetActive(!Pref.IsCharacterUnlocked(character.CharacterId));
        }

        if (GameStateManager.Instance != null) GameStateManager.Instance.selectedCharacter = character;
        Debug.Log($"Đã chọn nhân vật: {character.Name} (Locked: {!Pref.IsCharacterUnlocked(character.CharacterId)})");
        if (AudioController.Ins != null)
        {
            AudioController.Ins.PlayButtonClickSound();
        }
    }

    private void OnBuyButtonClicked()
    {
        if (selected != null)
        {
            if (Pref.UnlockCharacter(selected.CharacterId, selected.Cost))
            {
                // Cập nhật UI sau khi mua thành công
                InitializeToggles();
                Select(selected); // Làm mới UI để ẩn nút Buy và lock icon
                if (AudioController.Ins != null)
                {
                    AudioController.Ins.PlayButtonClickSound();
                }
                Debug.Log($"Đã mở khóa nhân vật: {selected.Name}");
            }
            else
            {
                Debug.LogWarning($"Không đủ coin để mở khóa nhân vật: {selected.Name}. Cần: {selected.Cost}, Hiện có: {Pref.Coins}");
            }
        }
    }


    public void StartGame()
    {
        if (selected != null && Pref.IsCharacterUnlocked(selected.CharacterId))
        {
            if (AudioController.Ins != null)
            {
                AudioController.Ins.PlayButtonClickSound();
            }
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogWarning("Cannot start game with locked character: " + (selected != null ? selected.Name : "None") + " or no character selected!");
        }
    }
}