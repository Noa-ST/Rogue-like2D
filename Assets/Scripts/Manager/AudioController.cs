using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    [Header("Main Settings:")]
    [Range(0, 1)]
    public float musicVolume = 0.3f;
    [Range(0, 1)]
    public float sfxVolume = 1f;

    public AudioSource musicAus;
    public AudioSource sfxAus;

    [Header("Background Musics (by Context):")]
    public AudioClip mainMenuMusic;        // Nhạc nền cho menu chính
    public AudioClip characterSelectionMusic; // Nhạc nền cho menu chọn nhân vật
    // Thêm mảng hoặc clip cho gameplay nếu cần sau này

    [Header("Game Sounds:")]
    public AudioClip gotCollectable;            // Âm thanh thu thập vật phẩm
    public AudioClip buttonClickSound;          // Âm thanh khi nhấn nút
    public AudioClip pauseSound;                // Âm thanh khi tạm dừng
    public AudioClip levelUpSound;              // Âm thanh khi lên cấp
    public AudioClip evolutionSound;            // Âm thanh khi tiến hóa
    public AudioClip damageTakenSound;          // Âm thanh khi nhận sát thương
    public AudioClip enemySpawnSound;           // Âm thanh khi kẻ thù spawn

    public override void Awake()
    {
        MakeSingleton(true);
        UpdateAudioState();
    }

    public override void Start()
    {
        PlayBackgroundMusic(); // Phát nhạc mặc định (có thể là mainMenuMusic)
    }

    private void UpdateAudioState()
    {
        if (musicAus) musicAus.mute = !Pref.MusicEnabled;
        if (sfxAus) sfxAus.mute = !Pref.SoundEnabled;
    }

    public void PlaySound(AudioClip[] clips, AudioSource aus = null)
    {
        if (!aus) aus = sfxAus;
        if (clips != null && clips.Length > 0 && aus && Pref.SoundEnabled)
        {
            var randomIdx = Random.Range(0, clips.Length);
            aus.PlayOneShot(clips[randomIdx], sfxVolume);
        }
    }

    public void PlaySound(AudioClip clip, AudioSource aus = null)
    {
        if (!aus) aus = sfxAus;
        if (clip != null && aus && Pref.SoundEnabled)
        {
            aus.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayMusic(AudioClip music, bool loop = true)
    {
        if (musicAus && music != null && Pref.MusicEnabled)
        {
            musicAus.clip = music;
            musicAus.loop = loop;
            musicAus.volume = musicVolume;
            musicAus.Play();
        }
    }

    public void SetMusicVolume(float vol)
    {
        if (musicAus) musicAus.volume = vol;
    }

    public void StopPlayMusic()
    {
        if (musicAus) musicAus.Stop();
    }

    /// <summary>
    /// Phát nhạc nền theo bối cảnh
    /// </summary>
    /// <param name="context">Bối cảnh (MainMenu, CharacterSelection, v.v.)</param>
    public void PlayBackgroundMusic(string context = "MainMenu")
    {
        switch (context.ToLower())
        {
            case "mainmenu":
                PlayMusic(mainMenuMusic, true);
                break;
            case "characterselection":
                PlayMusic(characterSelectionMusic, true);
                break;
            default:
                PlayMusic(mainMenuMusic, true); // Mặc định là mainMenuMusic
                break;
        }
    }

    public void PlayCollectableSound()
    {
        PlaySound(gotCollectable);
    }

    public void PlayButtonClickSound()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayPauseSound()
    {
        PlaySound(pauseSound);
    }

    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound);
    }

    public void PlayEvolutionSound()
    {
        PlaySound(evolutionSound);
    }

    public void PlayDamageTakenSound()
    {
        PlaySound(damageTakenSound);
    }

    public void PlayEnemySpawnSound()
    {
        PlaySound(enemySpawnSound);
    }
}