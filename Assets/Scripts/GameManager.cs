using System;
using System.Collections;
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

    [Header("Damage Text Settings")]
    public Canvas damageTxtCanvas;
    public float txtFontSize = 30;
    public TMP_FontAsset txtFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    int _stackedLevelUps = 0;

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
    public GameObject playerObject;

    public bool isGameOver 
    { 
        get 
        { 
            return currentState == GameState.GameOver; 
        }
    }
    public bool choosingUpgrade
    {
        get
        {
            return currentState == GameState.LevelUp;
        }
    }

    List<GameObject> _activePickups = new List<GameObject>(); // Danh sách các pickup hiện tại

    void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETED");
            Destroy(gameObject);
        }

        DisableScreen();
    }

    void Update()
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

            case GameState.LevelUp:
                break;

            default:
                Debug.LogWarning("STATE DOES NOT EXITST");
                break;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (Ins.damageTxtCanvas == null || target == null || Ins.referenceCamera == null) return;  // Kiểm tra các đối tượng quan trọng trước

        Ins.StartCoroutine(Ins.GenerateFloatingCoroutine(text, target, duration, speed));
    }


    IEnumerator GenerateFloatingCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        if (target == null) yield break; // Nếu target null, kết thúc Coroutine ngay

        GameObject txtObj = new GameObject("Damage Floating Text");
        RectTransform rect = txtObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = txtObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = txtFontSize;
        if (txtFont) tmPro.font = txtFont;

        // Set vị trí ban đầu của đối tượng
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(txtObj, duration); // Tự động hủy đối tượng sau thời gian `duration`

        txtObj.transform.SetParent(Ins.damageTxtCanvas.transform);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;

        while (t < duration)
        {
            // Kiểm tra rect có null không trước khi thay đổi thuộc tính
            if (rect == null || rect.gameObject == null)
            {
                yield break; // Nếu rect bị hủy, kết thúc Coroutine
            }

            yield return w;
            t += Time.deltaTime;

            // Kiểm tra nếu target đã bị hủy
            if (target == null || rect == null) yield break;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset));
        }

        // Kiểm tra nếu đối tượng còn tồn tại trước khi hủy
        if (txtObj != null)
        {
            Destroy(txtObj);
        }
    }

    public void ChangeState(GameState newState)
    {
        previousState = currentState;
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
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = previousState;
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
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
        Time.timeScale = 0f;
        DisplayResults();
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponAndPassiveItemUI(List<PlayerInventory.Slot> chosenWeaponsData, List<PlayerInventory.Slot> chosenPassivesData)
    {
        if (chosenWeaponsData.Count != chosenWeaponUI.Count || chosenPassivesData.Count != chosenWeaponUI.Count) return;

        for (int i = 0; i < chosenWeaponUI.Count; i++)
        {
            if (chosenWeaponsData[i].image.sprite)
            {
                chosenWeaponUI[i].enabled = true;
                chosenWeaponUI[i].sprite = chosenWeaponsData[i].image.sprite;
            }
            else
            {
                chosenWeaponUI[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemUI.Count; i++)
        {
            if (chosenPassivesData[i].image.sprite)
            {
                chosenPassiveItemUI[i].enabled = true;
                chosenPassiveItemUI[i].sprite = chosenPassivesData[i].image.sprite;
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

        if (levelUpScreen.activeSelf) _stackedLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            playerObject.SendMessage("RemoveAndApplyUpgrades");
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

        if (_stackedLevelUps > 0)
        {
            _stackedLevelUps--;
            StartLevelUp();
        }
    }

    public void RegisterPickup(GameObject pickup)
    {
        _activePickups.Add(pickup); // Đăng ký pickup
    }

    void CleanupPickups()
    {
        foreach (var pickup in _activePickups)
        {
            if (pickup != null)
            {
                Destroy(pickup);
            }
        }
        _activePickups.Clear();
    }

    void OnApplicationQuit()
    {
        CleanupPickups();
    }
}
