using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // IMPORTANT : ajout du using pour Slider

public class SettingsView : MonoBehaviour
{
    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private List<Resolution> _resolutions = new();
    private List<string> _resolutionLabels = new();
    private int _minimumResWidth = 800;
    private int _minimumResHeight = 600;

    // Start est appelé une seule fois
    void Start()
    {
        // === AUDIO SETUP ===
        SetupAudioSliders();

        // === RESOLUTION SETUP ===
        SetupResolutionDropdown();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetupAudioSliders()
    {
        // Setup slider listeners
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 100f;
            masterVolumeSlider.value = 50f;
            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 100f;
            musicVolumeSlider.value = 50f;
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 100f;
            sfxVolumeSlider.value = 50f;
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolume);
        }
    }

    private void SetupResolutionDropdown()
    {
        foreach (Resolution res in Screen.resolutions)
        {
            int resWidth = res.width;
            int resHeight = res.height;

            if (resWidth >= _minimumResWidth && resHeight >= _minimumResHeight)
            {
                string resLabel = resWidth.ToString() + " x " + resHeight.ToString();

                if (!_resolutionLabels.Contains(resLabel))
                {
                    _resolutionLabels.Add(resLabel);
                    _resolutions.Add(res);
                }
            }
        }

        if (_resolutions.Count > 0)
        {
            Resolution defaultRes = _resolutions.Last();
            resolutionDropdown.AddOptions(_resolutionLabels);
            resolutionDropdown.SetValueWithoutNotify(_resolutions.Count - 1);
            Screen.SetResolution(defaultRes.width, defaultRes.height, true);
        }
    }

    // === AUDIO METHODS ===
    public void UpdateMasterVolume(float sliderValue)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(sliderValue);
        }
    }

    public void UpdateMusicVolume(float sliderValue)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(sliderValue);
        }
    }

    public void UpdateSfxVolume(float sliderValue)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(sliderValue);
        }
    }

    // === RESOLUTION METHOD ===
    public void UpdateResolution(int dropdownIdx)
    {
        if (dropdownIdx >= 0 && dropdownIdx < _resolutions.Count)
        {
            Resolution selectedRes = _resolutions.ElementAt(dropdownIdx);
            Screen.SetResolution(selectedRes.width, selectedRes.height, true);
        }
    }
}