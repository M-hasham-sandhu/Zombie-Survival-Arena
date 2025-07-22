using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all game audio: SFX and music. Singleton for global access.
/// Use PlaySFX and PlayMusic to trigger sounds by name.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
    }

    [Header("Sound Library")]
    public Sound[] sfxSounds;
    public Sound[] musicTracks;

    private Dictionary<string, Sound> sfxDict;
    private Dictionary<string, Sound> musicDict;

    private AudioSource sfxSource;
    private AudioSource musicSource;

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

        // Setup audio sources
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // Build lookup dictionaries
        sfxDict = new Dictionary<string, Sound>();
        foreach (var s in sfxSounds)
            if (!string.IsNullOrEmpty(s.name)) sfxDict[s.name] = s;

        musicDict = new Dictionary<string, Sound>();
        foreach (var m in musicTracks)
            if (!string.IsNullOrEmpty(m.name)) musicDict[m.name] = m;
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
    /// Stop the current music.
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
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
