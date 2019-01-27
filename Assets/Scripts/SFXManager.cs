using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SFXManager : MonoBehaviour
{

    public enum SFXType
    {
        MeleeWepon,
        MeleeWeaponBlock,
        CannonShoot,
        CannonImpact,
        CannonImpactBlock,
        Footsteps,  // TODO: implement
        EquipShell,
        Death,
        ButtonsSwitch, 
        ButtonsClick,  // TODO: implement
        ShellBreak,
        PaguroVoice,  // TODO: implement
        Win
    }

    [SerializeField]
    private Settings settings;
    [SerializeField]
    private SFXHolder[] soundEffects;

    private static SFXManager _instance;

    private Dictionary<SFXType, AudioClip[]> clipsByType;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("There should be only one SFXManager in a scene.");
            DestroyImmediate(gameObject);
        }

        clipsByType = soundEffects.ToDictionary(holder => holder.type, holder => holder.clips);

        GetComponent<AudioSource>().volume = settings.musicVolume;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = settings.sfxVolume;
    }

    public static void PlaySFX(SFXType type)
    {
        if (_instance.clipsByType.ContainsKey(type))
        {
            AudioClip[] clips = _instance.clipsByType[type];
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            _instance.sfxSource.PlayOneShot(randomClip);
        }
        else
        {
            Debug.LogWarning("Trying to play an SFX of type " + type + " but SFXManager has no clips");
        }
    }

    [System.Serializable]
    private struct SFXHolder
    {
        public SFXType type;
        public AudioClip[] clips;
    }
}
