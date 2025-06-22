using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    [Header("Volume Settings")]
    public AudioSource musicSource;

    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    public float volumeStep = 0.1f;
    private float minVolume = 0f;
    private float maxVolume = 1f;

    [Header("Font Settings")]
    public TextMeshProUGUI[] textsToResize;
    public float fontSizeStep = 2f;

    void Start()
    {
        GameSettingsManager.LoadSettings();

        musicSource.volume = GameSettingsManager.Settings.musicVolume;

        foreach (var txt in textsToResize)
        {
            txt.fontSize = GameSettingsManager.Settings.fontSize;
        }

        UpdateVolumeTexts();
    }

    public void OpenSettings() => settingsPanel.SetActive(true);

    public void ClosingSettings() => settingsPanel.SetActive(false);

    public void IncreaseMusicVolume()
    {
        musicSource.volume = Mathf.Clamp(musicSource.volume + volumeStep, minVolume, maxVolume);
        GameSettingsManager.Settings.musicVolume = musicSource.volume;
        GameSettingsManager.SaveSettings();
        UpdateVolumeTexts();
    }

    public void DecreaseMusicVolume()
    {
        musicSource.volume = Mathf.Clamp(musicSource.volume - volumeStep, minVolume, maxVolume);
        GameSettingsManager.Settings.musicVolume = musicSource.volume;
        GameSettingsManager.SaveSettings();
        UpdateVolumeTexts();
    }

    public void IncreaseSFXVolume()
    {
        float newVolume = Mathf.Clamp(GameSettingsManager.Settings.sfxVolume + volumeStep, minVolume, maxVolume);
        GameSettingsManager.Settings.sfxVolume = newVolume;
        GameSettingsManager.SaveSettings();
        UpdateVolumeTexts();
    }

    public void DecreaseSFXVolume()
    {
        float newVolume = Mathf.Clamp(GameSettingsManager.Settings.sfxVolume - volumeStep, minVolume, maxVolume);
        GameSettingsManager.Settings.sfxVolume = newVolume;
        GameSettingsManager.SaveSettings();
        UpdateVolumeTexts();
    }

    private void UpdateVolumeTexts()
    {
        musicVolumeText.text = $"Music: {(int)(musicSource.volume * 100)}%";
        sfxVolumeText.text = $"SFX: {(int)(GameSettingsManager.Settings.sfxVolume * 100)}%";
    }

    public void IncreaseFontSize()
    {
        foreach (var txt in textsToResize)
        {
            txt.fontSize += fontSizeStep;
        }

        GameSettingsManager.Settings.fontSize = textsToResize[0].fontSize;
        GameSettingsManager.SaveSettings();
    }

    public void DecreaseFontSize()
    {
        foreach (var txt in textsToResize)
        {
            txt.fontSize = Mathf.Max(10f, txt.fontSize - fontSizeStep);
        }

        GameSettingsManager.Settings.fontSize = textsToResize[0].fontSize;
        GameSettingsManager.SaveSettings();
    }
}
