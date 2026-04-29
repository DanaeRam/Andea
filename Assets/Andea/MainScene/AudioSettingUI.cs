using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider slider;

    void Start()
    {
        float saved = PlayerPrefs.GetFloat("MasterVolume", 1f);
        slider.value = saved;
        SetVolume(saved);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        float db = Mathf.Log10(value) * 20;
        audioMixer.SetFloat("MasterVolume", db);

        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}