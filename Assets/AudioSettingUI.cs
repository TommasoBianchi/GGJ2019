using UnityEngine;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{

    [SerializeField]
    private Settings settings;

    public void SetMusicVolume(Slider slider)
    {
        settings.musicVolume = slider.value;
    }

    public void SetSFXVolume(Slider slider)
    {
        settings.sfxVolume = slider.value;
    }
}
