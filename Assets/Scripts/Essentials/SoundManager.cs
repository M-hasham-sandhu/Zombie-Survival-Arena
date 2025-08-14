using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all game audio: SFX and music. Singleton for global access.
/// Supports MP3 audio files which will be automatically converted by Unity.
/// Use PlaySFX and PlayMusic to trigger sounds by name.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Serializable]
    public class Sound
    {
        public string name;
        [Tooltip("Supports MP3 files")]
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;

        public bool IsValid => !string.IsNullOrEmpty(name) && clip != null;
    }

    [Header("Sound Library")]
    public Sound[] sfxSounds;
    public Sound[] musicTracks;
    [SerializeField] private string defaultMusicTrack = "GameMusic"; // Add this line

    private Dictionary<string, Sound> sfxDict;
    private Dictionary<string, Sound> musicDict;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void ValidateAudioClips()
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.clip == null)
            {
                Debug.LogWarning($"Missing audio clip for SFX: {sound.name}");
                continue;
            }
            
            if (!sound.IsValid)
            {
                Debug.LogWarning($"Invalid sound configuration for: {sound.name}");
            }
        }

        foreach (var track in musicTracks)
        {
            if (track.clip == null)
            {
                Debug.LogWarning($"Missing audio clip for music track: {track.name}");
                continue;
            }

            if (!track.IsValid)
            {
                Debug.LogWarning($"Invalid music configuration for: {track.name}");
            }
        }
    }

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Validate audio clips
        ValidateAudioClips();

        // Setup audio sources with optimal settings for MP3
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.priority = 0; // Highest priority for music

        // Build lookup dictionaries
        sfxDict = new Dictionary<string, Sound>();
        foreach (var s in sfxSounds)
        {
            if (s.IsValid) sfxDict[s.name] = s;
        }

        musicDict = new Dictionary<string, Sound>();
        foreach (var m in musicTracks)
        {
            if (m.IsValid) musicDict[m.name] = m;
        }
    }

    private void Start()
    {
        // Start playing the default background music
        if (!string.IsNullOrEmpty(defaultMusicTrack))
        {
            PlayMusic(defaultMusicTrack);
        }
    }

    /// <summary>
    /// Play a sound effect by name.
    /// </summary>
    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var s))
        {
            sfxSource.PlayOneShot(s.clip, s.volume);
        }
    }

    /// <summary>
    /// Play a music track by name (stops previous music).
    /// </summary>
    public void PlayMusic(string name)
    {
        if (musicDict.TryGetValue(name, out var m))
        {
            musicSource.clip = m.clip;
            musicSource.volume = m.volume;
            musicSource.loop = m.loop;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stop all sound effects.
    /// </summary>
    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    /// <summary>
    /// Stop the current music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Mute or unmute all sound effects.
    /// </summary>
    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }

    /// <summary>
    /// Mute or unmute music.
    /// </summary>
    public void MuteMusic(bool mute)
    {
        musicSource.mute = mute;
    }

    /// <summary>
    /// Enable or disable all sound effects (volume on/off).
    /// </summary>
    public void EnableSFX(bool enable)
    {
        sfxSource.volume = enable ? 1f : 0f;
    }

    /// <summary>
    /// Enable or disable music (volume on/off).
    /// </summary>
    public void EnableMusic(bool enable)
    {
        musicSource.volume = enable ? 1f : 0f;
    }
}
