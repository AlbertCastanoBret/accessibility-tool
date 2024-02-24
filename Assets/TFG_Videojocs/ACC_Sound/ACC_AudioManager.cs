using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class ACC_AudioManager : MonoBehaviour
{
    public static ACC_AudioManager Instance;
    
    [SerializeField] private ACC_Sound[] musicSounds, sfxSounds;
    [SerializeField] private AudioSource musicSource, sfxSource;
    public event Action OnSoundsChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Plumber");
    }

    private void OnValidate()
    {
        UpdateSoundNames(musicSounds);
        UpdateSoundNames(sfxSounds);
        OnSoundsChanged?.Invoke();
    }
    
    private void UpdateSoundNames(ACC_Sound[] sounds)
    {
        foreach (var sound in sounds)
        {
            if (sound.AudioClip != null)
            {
                sound.Name = sound.AudioClip.name;
            }
        }
    }

    public void PlayMusic(string name)
    {
        ACC_Sound accSound = Array.Find(musicSounds, x => x.Name == name);
        if (accSound != null)
        {
            musicSource.clip = accSound.AudioClip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name)
    {
        ACC_Sound accSound = Array.Find(sfxSounds, x => x.Name == name);
        if (accSound != null)
        {
            sfxSource.clip = accSound.AudioClip;
            sfxSource.Play();
        }
    }

    public ACC_Sound[] GetSFXSounds()
    {
        return sfxSounds;
    }
}
