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

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

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
        if (canUse)
        {
            if (PlayerPrefs.HasKey("materVolume"))
            {
                float loadedVolume = PlayerPrefs.GetFloat("materVolume");
                volumeTextValue.text = loadedVolume.ToString("0.0");
                volumeSlider.value = loadedVolume;
                AudioListener.volume = loadedVolume;
            }
            else
            {
                menuController.ResetButton("Audio");
            }
            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int loadedQuality = PlayerPrefs.GetInt("masterQuality");
                qualityDropdown.value = loadedQuality;
                QualitySettings.SetQualityLevel(loadedQuality);
            }
            if (PlayerPrefs.HasKey("masterFullScreen"))
            {
                int loadedFullScreen = PlayerPrefs.GetInt("masterFullScreen");
                if (loadedFullScreen == 1)
                {
                    fullScreenToggle.isOn = true;
                    Screen.fullScreen = true;
                }
                else
                {
                    fullScreenToggle.isOn = false;
                    Screen.fullScreen = false;
                }
            }
            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float loadedBrightness = PlayerPrefs.GetFloat("masterBrightness");
                brightnessTextValue.text = loadedBrightness.ToString("0.0");
                brightnessSlider.value = loadedBrightness;
            }
            if (PlayerPrefs.HasKey("masterSen"))
            {
                float loadSensitivity = PlayerPrefs.GetFloat("masterSen");
                controllerSenTextValue.text = loadSensitivity.ToString("0");
                menuController.mainControllerSen = Mathf.RoundToInt(loadSensitivity);
            }
            if (PlayerPrefs.HasKey("masterInvertY"))
            { 
                if (PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    invertYToggle.isOn = true;
                }
                else
                {
                    invertYToggle.isOn = false;
                }
            }
        }
    }
}