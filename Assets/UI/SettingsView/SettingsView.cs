using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SettingsView : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private List<Resolution> _resolutions = new();
    private List<string> _resolutionLabels = new();
    private int _minimumResWidth = 800;
    private int _minimumResHeight = 600;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Resolution res in Screen.resolutions)
        {
            int resWidth = res.width;
            int resHeight = res.height;

            if(resWidth >= _minimumResWidth && resHeight >= _minimumResHeight)
            {
                string resLabel = resWidth.ToString() + " x " + resHeight.ToString();

                if(!_resolutionLabels.Contains(resLabel))
                {
                    _resolutionLabels.Add(resLabel);
                    _resolutions.Add(res);
                }
            }
        }

        Resolution defaultRes = _resolutions.Last();

        resolutionDropdown.AddOptions(_resolutionLabels);
        resolutionDropdown.SetValueWithoutNotify(_resolutions.Count - 1);
        Screen.SetResolution(defaultRes.width, defaultRes.height, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Audio
    public void UpdateMasterGain(float newSliderValue)
    {
        // Requires a slider range (0.0001 to 1) to handle sound well
        float newMasterGain = _SliderValueToGain(newSliderValue);
        Debug.Log("Master Gain: " + newMasterGain);
    }

    public void UpdateMusicGain(float newSliderValue)
    {
        // Requires a slider range (0.0001 to 1) to handle sound well
        float newMusicGain = _SliderValueToGain(newSliderValue);
        Debug.Log("Music Gain: " + newMusicGain);
    }

    public void UpdateSfxGain(float newSliderValue)
    {
        // Requires a slider range (0.0001 to 1) to handle sound well
        float newSfxGain = _SliderValueToGain(newSliderValue);
        Debug.Log("SFX Gain: " + newSfxGain);
    }

    private float _SliderValueToGain(float newSliderValue)
    {
        return Mathf.Log10(newSliderValue) * 20;
    }

    // Resolution
    public void UpdateResolution(int dropdownIdx)
    {
        Resolution defaultRes = _resolutions.ElementAt(dropdownIdx);
        Screen.SetResolution(defaultRes.width, defaultRes.height, true);
    }
}
