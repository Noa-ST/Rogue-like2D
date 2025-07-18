using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Setting")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Toggle Setting")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Graphics Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt;

    [Header("Levels To Load")]
    public string menuScene; 
    private string levelToLoad;
    [SerializeField] private GameObject noSaveGameDialog = null;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        Pref.InitializeGameState();
        LoadVolumeValue();
        LoadControllerSenValue();
        LoadBrightnessValue();
        LoadQualityValue();
        LoadFullScreenValue();
        LoadInvertYValue();
        if (AudioController.Ins != null)
        {
            AudioController.Ins.PlayBackgroundMusic("mainMenuMusic");
        }
    }

    private void LoadVolumeValue()
    {
        float volumeValue = PlayerPrefs.GetFloat("masterVolume");
        if (volumeValue == 0) volumeValue = defaultVolume;
        volumeSlider.value = volumeValue;
        AudioListener.volume = volumeValue;
        volumeTextValue.text = volumeValue.ToString("0.0");
        if (AudioController.Ins != null)
        {
            AudioController.Ins.SetMusicVolume(volumeValue);
            AudioController.Ins.sfxAus.volume = volumeValue;
        }
    }

    private void LoadControllerSenValue()
    {
        int controllerSenValue = PlayerPrefs.GetInt("masterControllerSen");
        if (controllerSenValue == 0) controllerSenValue = defaultSen;
        mainControllerSen = controllerSenValue;
        controllerSenSlider.value = controllerSenValue;
        controllerSenTextValue.text = controllerSenValue.ToString("0");
    }

    private void LoadBrightnessValue()
    {
        float brightnessValue = PlayerPrefs.GetFloat("masterBrightness");
        if (brightnessValue == 0) brightnessValue = defaultBrightness;
        brightnessSlider.value = brightnessValue;
        _brightnessLevel = brightnessValue;
        brightnessTextValue.text = brightnessValue.ToString("0.0");
    }

    private void LoadQualityValue()
    {
        _qualityLevel = PlayerPrefs.GetInt("masterQualityLevel");
        if (qualityDropdown != null) qualityDropdown.value = _qualityLevel;
        QualitySettings.SetQualityLevel(_qualityLevel);
    }

    private void LoadFullScreenValue()
    {
        _isFullScreen = PlayerPrefs.GetInt("masterFullScreen") == 1;
        if (fullScreenToggle != null) fullScreenToggle.isOn = _isFullScreen;
        Screen.fullScreen = _isFullScreen;
    }

    private void LoadInvertYValue()
    {
        int invertYValue = PlayerPrefs.GetInt("masterInvertY");
        if (invertYToggle != null) invertYToggle.isOn = invertYValue == 1;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionDropdown != null)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    public void LoadMenuScene()
    {
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        SceneManager.LoadScene(menuScene);
    }

    public void ExitButton()
    {
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        Pref.SaveGameState(SceneManager.GetActiveScene().name);
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        if (AudioController.Ins != null)
        {
            AudioController.Ins.SetMusicVolume(volume);
            AudioController.Ins.sfxAus.volume = volume;
        }
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = mainControllerSen.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle != null && invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }
        PlayerPrefs.SetFloat("masterControllerSen", mainControllerSen);
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        PlayerPrefs.SetInt("masterQualityLevel", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullScreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        if (MenuType == "Graphics")
        {
            if (brightnessSlider != null) brightnessSlider.value = defaultBrightness;
            if (brightnessTextValue != null) brightnessTextValue.text = defaultBrightness.ToString("0.0");
            if (qualityDropdown != null) qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);
            if (fullScreenToggle != null) fullScreenToggle.isOn = false;
            Screen.fullScreen = false;
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            if (resolutionDropdown != null) resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            if (volumeSlider != null)
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeTextValue.text = defaultVolume.ToString("0.0");
                if (AudioController.Ins != null)
                {
                    AudioController.Ins.SetMusicVolume(defaultVolume);
                    AudioController.Ins.sfxAus.volume = defaultVolume;
                }
            }
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            if (controllerSenSlider != null)
            {
                controllerSenTextValue.text = defaultSen.ToString("0");
                controllerSenSlider.value = defaultSen;
                mainControllerSen = defaultSen;
            }
            if (invertYToggle != null) invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        if (confirmationPrompt != null) confirmationPrompt.SetActive(true);
        if (AudioController.Ins != null) AudioController.Ins.PlayButtonClickSound();
        yield return new WaitForSeconds(2);
        if (confirmationPrompt != null) confirmationPrompt.SetActive(false);
    }
}