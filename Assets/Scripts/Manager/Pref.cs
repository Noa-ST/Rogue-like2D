using UnityEngine;

public class Pref
{
    public static int CurPlayerId
    {
        set => PlayerPrefs.SetInt(PrefConst.CUR_PLAYER_ID, value);
        get => PlayerPrefs.GetInt(PrefConst.CUR_PLAYER_ID);
    }

    public static int Coins
    {
        set => PlayerPrefs.SetInt(PrefConst.COIN_KEY, value);
        get => PlayerPrefs.GetInt(PrefConst.COIN_KEY);
    }

    public static bool MusicEnabled
    {
        get => PlayerPrefs.GetInt(PrefConst.MUSIC_ENABLED_KEY.ToString(), 1) == 1; // Mặc định là 1 (true)
        set => PlayerPrefs.SetInt(PrefConst.MUSIC_ENABLED_KEY.ToString(), value ? 1 : 0);
    }

    public static bool SoundEnabled
    {
        get => PlayerPrefs.GetInt(PrefConst.SOUND_ENABLED_KEY.ToString(), 1) == 1; // Mặc định là 1 (true)
        set => PlayerPrefs.SetInt(PrefConst.SOUND_ENABLED_KEY.ToString(), value ? 1 : 0);
    }

public static void SetBool(string key, bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt(key, 1);
        }
        else
        {
            PlayerPrefs.SetInt(key, 0);
        }
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1 ? true : false;
    }

    // Thêm phương thức để mở khóa character
    public static bool UnlockCharacter(int characterId, int unlockCost)
    {
        if (Coins >= unlockCost)
        {
            Coins -= unlockCost;
            string unlockKey = "CharacterUnlocked_" + characterId; // Key để lưu trạng thái mở khóa
            SetBool(unlockKey, true);
            PlayerPrefs.Save(); // Lưu ngay lập tức
            Debug.Log("Character " + characterId + " unlocked! Remaining coins: " + Coins);
            return true;
        }
        Debug.LogWarning("Not enough coins to unlock character " + characterId + ". Required: " + unlockCost + ", Available: " + Coins);
        return false;
    }

    // Kiểm tra xem character đã được mở khóa chưa
    public static bool IsCharacterUnlocked(int characterId)
    {
        string unlockKey = "CharacterUnlocked_" + characterId;
        return GetBool(unlockKey);
    }

    public static void InitializeDefaultCharacter()
    {
        CharacterData[] characters = UICharacterSelector.GetAllCharacterDataAssets();
        if (characters.Length > 0)
        {
            // Sắp xếp theo characterId để lấy nhân vật đầu tiên
            System.Array.Sort(characters, (a, b) => a.CharacterId.CompareTo(b.CharacterId));
            int firstCharacterId = characters[0].CharacterId;

            if (!IsCharacterUnlocked(firstCharacterId))
            {
                // Mặc định mở khóa nhân vật đầu tiên miễn phí
                string unlockKey = "CharacterUnlocked_" + firstCharacterId;
                SetBool(unlockKey, true);
                PlayerPrefs.Save();
                Debug.Log("Default character with ID " + firstCharacterId + " unlocked for free.");
            }
            else
            {
                Debug.Log("Default character with ID " + firstCharacterId + " is already unlocked.");
            }
        }
        else
        {
            Debug.LogWarning("No CharacterData assets found to initialize default character!");
        }
    }

    public static void InitializeGameState()
    {
        if (!PlayerPrefs.HasKey(PrefConst.COIN_KEY))
        {
            Coins = 0;
            MusicEnabled = true; 
            SoundEnabled = true;
            InitializeDefaultCharacter();
            Debug.Log("Initialized default game state.");
        }
    }

    public static void SaveGameState(string currentScene)
    {
        PlayerPrefs.SetString("SavedLevel", currentScene);
        PlayerPrefs.Save();
        Debug.Log("Game state saved. Current scene: " + currentScene + ", Coins: " + Coins);
    }
}