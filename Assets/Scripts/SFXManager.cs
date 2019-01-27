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
        Footsteps,
        EquipShell,
        Death,
        ButtonsSwitch, 
        ButtonsClick,  // TODO: implement
        ShellBreak,
        PaguroVoice,
        Win
    }

    [SerializeField]
    private Settings settings;
    [SerializeField]
    private SFXHolder[] soundEffects;

    [SerializeField, Range(0f, 1f)]
    private float musicVolumeMultiplier;
    [SerializeField, Range(0f, 1f)]
    private float sfxMusicMultiplier;

    private static SFXManager _instance;

    private Dictionary<SFXType, AudioClip[]> clipsByType;
    private AudioSource sfxSource;

    private Dictionary<int, AudioSource> stepsAudioSources;

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
        stepsAudioSources = new Dictionary<int, AudioSource>();

        GetComponent<AudioSource>().volume = settings.musicVolume * musicVolumeMultiplier;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = settings.sfxVolume * sfxMusicMultiplier;
    }

    public static void PlaySFX(SFXType type)
    {
        if (_instance.clipsByType.ContainsKey(type) && _instance.clipsByType[type].Length > 0)
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

    public static void PlayFootsteps(int playerID)
    {
        if (!_instance.stepsAudioSources.ContainsKey(playerID))
        {
            _instance.stepsAudioSources[playerID] = _instance.gameObject.AddComponent<AudioSource>();
            _instance.stepsAudioSources[playerID].volume = _instance.settings.sfxVolume * _instance.sfxMusicMultiplier;
            _instance.stepsAudioSources[playerID].playOnAwake = false;
            _instance.stepsAudioSources[playerID].loop = true;
            _instance.stepsAudioSources[playerID].clip = _instance.clipsByType[SFXType.Footsteps][0];
        }

        if (!_instance.stepsAudioSources[playerID].isPlaying)
        {
            _instance.stepsAudioSources[playerID].Play();
        }
    }

    public static void StopFootsteps(int playerID)
    {
        if (_instance.stepsAudioSources.ContainsKey(playerID))
        {
            if (_instance.stepsAudioSources[playerID].isPlaying)
            {
                _instance.stepsAudioSources[playerID].Stop();
            }
        }
    }

    [System.Serializable]
    private struct SFXHolder
    {
        public SFXType type;
        public AudioClip[] clips;
    }
}
