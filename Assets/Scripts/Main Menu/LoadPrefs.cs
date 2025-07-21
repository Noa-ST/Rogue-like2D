using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Music Volume Settings")]
    [SerializeField] private TMP_Text musicVolumeTextValue = null;

    [Header("Sound Volume Settings")]
    [SerializeField] private TMP_Text soundVolumeTextValue = null;

    [Header("Brightness Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;

    [Header("Quality level Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Sensitivity Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;

    [Header("Invert Y Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Awake()
    {
        if (canUse && menuController != null)
        {
            // Load Music Volume
            if (PlayerPrefs.HasKey("MusicVolume"))
            {
                float loadedMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
                if (musicVolumeTextValue != null) musicVolumeTextValue.text = loadedMusicVolume.ToString("0.0");
            }

            // Load Sound Volume
            if (PlayerPrefs.HasKey("SoundVolume"))
            {
                float loadedSoundVolume = PlayerPrefs.GetFloat("SoundVolume");
                if (soundVolumeTextValue != null) soundVolumeTextValue.text = loadedSoundVolume.ToString("0.0");
            }

            // Load Quality
            if (PlayerPrefs.HasKey("masterQualityLevel"))
            {
                int loadedQuality = PlayerPrefs.GetInt("masterQualityLevel");
                if (qualityDropdown != null)
                {
                    qualityDropdown.value = loadedQuality;
                    QualitySettings.SetQualityLevel(loadedQuality);
                }
            }

            // Load Fullscreen
            if (PlayerPrefs.HasKey("masterFullScreen"))
            {
                int loadedFullScreen = PlayerPrefs.GetInt("masterFullScreen");
                if (fullScreenToggle != null)
                {
                    fullScreenToggle.isOn = loadedFullScreen == 1;
                    Screen.fullScreen = loadedFullScreen == 1;
                }
            }

            // Load Brightness
            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float loadedBrightness = PlayerPrefs.GetFloat("masterBrightness");
                if (brightnessTextValue != null) brightnessTextValue.text = loadedBrightness.ToString("0.0");
                if (brightnessSlider != null) brightnessSlider.value = loadedBrightness;
            }

            // Load Sensitivity
            if (PlayerPrefs.HasKey("masterControllerSen"))
            {
                float loadSensitivity = PlayerPrefs.GetFloat("masterControllerSen");
                if (controllerSenTextValue != null) controllerSenTextValue.text = loadSensitivity.ToString("0");
                if (menuController != null) menuController.mainControllerSen = Mathf.RoundToInt(loadSensitivity);
            }

            // Load Invert Y
            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if (invertYToggle != null)
                {
                    invertYToggle.isOn = PlayerPrefs.GetInt("masterInvertY") == 1;
                }
            }
        }
        else
        {
            Debug.LogWarning("LoadPrefs: canUse is false or menuController is null!");
        }
    }
}