using System;
using UnityEngine;

public class SettingsView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
