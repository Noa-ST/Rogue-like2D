using UnityEngine;
using UnityEngine.UI;

public class SettingAudio : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;           // Slider điều chỉnh âm lượng nhạc
    public Slider soundSlider;           // Slider điều chỉnh âm lượng hiệu ứng

    [Header("References")]
    [SerializeField] private MenuController menuController; // Để cập nhật text

    private void Start()
    {
        // Gán giá trị ban đầu từ PlayerPrefs
        LoadSettings();

        // Đăng ký sự kiện cho slider
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.3f); // Giá trị mặc định 0.3
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
        if (soundSlider != null)
        {
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f); // Giá trị mặc định 1.0
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        }

        ApplySettings();
    }

    private void LoadSettings()
    {
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        if (soundSlider != null) soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
    }

    private void OnMusicSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        UpdateTextDisplay();
        ApplySettings();
        Debug.Log("Music volume changed to: " + value);
    }

    private void OnSoundSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
        UpdateTextDisplay();
        ApplySettings();
        Debug.Log("Sound volume changed to: " + value);
    }

    private void UpdateTextDisplay()
    {
        if (menuController != null)
        {
            menuController.UpdateVolumeText(PlayerPrefs.GetFloat("MusicVolume", 0.3f), PlayerPrefs.GetFloat("SoundVolume", 1f));
        }
    }

    public void ApplySettings()
    {
        if (AudioController.Ins != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
            float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            bool musicEnabled = Pref.MusicEnabled;
            bool soundEnabled = Pref.SoundEnabled;

            AudioController.Ins.SetMusicVolume(musicVolume);
            if (AudioController.Ins.musicAus != null) AudioController.Ins.musicAus.mute = !musicEnabled;
            if (AudioController.Ins.sfxAus != null) AudioController.Ins.sfxAus.volume = soundVolume;
            if (AudioController.Ins.sfxAus != null) AudioController.Ins.sfxAus.mute = !soundEnabled;

            Debug.Log("Applied settings - Music: " + musicVolume + " (Enabled: " + musicEnabled + "), Sound: " + soundVolume + " (Enabled: " + soundEnabled + ")");
        }
    }

    public void ResetAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", 0.3f);
        PlayerPrefs.SetFloat("SoundVolume", 1f);
        LoadSettings();
        ApplySettings();
        UpdateTextDisplay();
    }
}