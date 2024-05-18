using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            LoadVolume();
        }
        else
        {
            SetVolume();
        }
    }

    public void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        SetVolume();
    }

    public void SetVolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }
}
