using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins;
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Current Stat Displays")]
    public TMP_Text curHealthDisplay;
    public TMP_Text curRecoveryDisplay;
    public TMP_Text curMoveSpeedDisplay;
    public TMP_Text curMightDisplay;
    public TMP_Text curProjectileSpeedDisplay;
    public TMPro.TMP_Text curMagnetDisplay;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurviedDisplay;
    public List<Image> chosenWeaponUI = new List<Image>(6);
    public List<Image> chosenPassiveItemUI = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit;
    float stopwatchTime;
    public TMP_Text stopwatchDisplay;

    public bool isGameOver = false;
    public bool choosingUpgrade;
    public GameObject playerObject;

    private List<GameObject> activePickups = new List<GameObject>(); // Danh sách các pickup hiện tại


    private void Awake()
    {
        if(Ins == null)
        {
            Ins = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETED");
        }
        DisableScreen();
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopWatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game is over");
                    CleanupPickups();
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXITST");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        if (newState == GameState.GameOver || newState == GameState.Paused)
        {
            CleanupPickups();
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            currentState = GameState.Paused;
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game is paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = previousState;
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            Debug.Log("Game is resumed");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void DisableScreen()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurviedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterScriptObject chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponAndPassiveItemUI(List<Image> chosenWeaponsData, List<Image> chosenPassivesData)
    {
        if (chosenWeaponsData.Count != chosenWeaponUI.Count || chosenPassivesData.Count != chosenWeaponUI.Count) return;

        for (int i = 0; i < chosenWeaponUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponUI[i].enabled = true;
                chosenWeaponUI[i].sprite = chosenWeaponsData[i].sprite;
            } else
            {
                chosenWeaponUI[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemUI.Count; i++)
        {
            if (chosenPassivesData[i].sprite)
            {
                chosenPassiveItemUI[i].enabled = true;
                chosenPassiveItemUI[i].sprite = chosenPassivesData[i].sprite;
            }
            else
            {
                chosenPassiveItemUI[i].enabled = false;
            }
        }
    }

    void UpdateStopWatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopWatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    // cập nhật giao diện hiển thị thời gian của đồng hồ bấm giờ, chuyển đổi thời gian từ giây thành định dạng phút
    void UpdateStopWatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60); // Chia `stopwatchTime` cho 60 để lấy số phút, sau đó dùng `Mathf.FloorToInt` để lấy giá trị nguyên gần nhất (không làm tròn).
        int seconds = Mathf.FloorToInt(stopwatchTime % 60); // Lấy phần dư của `stopwatchTime` khi chia cho 60 để tính số giây.

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Định dạng chuỗi để hiển thị thời gian dưới dạng `phút:giây`, ví dụ "05:30".
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrade");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

    public void RegisterPickup(GameObject pickup)
    {
        activePickups.Add(pickup); // Đăng ký pickup
    }

    void CleanupPickups()
    {
        foreach (var pickup in activePickups)
        {
            if (pickup != null)
            {
                Destroy(pickup);
            }
        }
        activePickups.Clear();
    }

    void OnApplicationQuit()
    {
        CleanupPickups();
    }
}
