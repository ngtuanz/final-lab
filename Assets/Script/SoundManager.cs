using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject muteMusic;   // Icon mute Music
    public GameObject muteSFX;     // Icon mute SFX

    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public AudioSource clickSound; // âm thanh click (SFX)

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    void Start()
    {
        // Ẩn icon mute khi bắt đầu
        muteMusic.SetActive(false);
        muteSFX.SetActive(false);

        // Load volume từ mixer
        if (mainMixer.GetFloat("Music", out float musicVol))
            musicSlider.value = Mathf.Pow(10f, musicVol / 20f);

        if (mainMixer.GetFloat("SFX", out float sfxVol))
            sfxSlider.value = Mathf.Pow(10f, sfxVol / 20f);

        // Gắn event cho slider
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // ---------------- MUSIC ----------------
    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mainMixer.SetFloat("Music", Mathf.Log10(value) * 20f);
    }

    public void ToggleMuteMusic()
    {
        if (!isMusicMuted)
        {
            // Tắt ngay
            mainMixer.SetFloat("Music", -80f);
            muteMusic.SetActive(true);
            isMusicMuted = true;
        }
        else
        {
            // Bật lại theo slider
            SetMusicVolume(musicSlider.value);
            muteMusic.SetActive(false);
            isMusicMuted = false;
        }
    }

    // ---------------- SFX ----------------
    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mainMixer.SetFloat("SFX", Mathf.Log10(value) * 20f);
    }

    public void ToggleMuteSFX()
    {
        if (!isSFXMuted)
        {
            // Tắt ngay
            mainMixer.SetFloat("SFX", -80f);
            muteSFX.SetActive(true);
            isSFXMuted = true;
        }
        else
        {
            // Bật lại theo slider
            SetSFXVolume(sfxSlider.value);
            muteSFX.SetActive(false);
            isSFXMuted = false;
        }
    }

    // ---------------- TEST CLICK ----------------
    public void PlayClickSound()
    {
        if (clickSound != null)
            clickSound.Play();
    }
}
