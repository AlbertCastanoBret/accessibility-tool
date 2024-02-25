using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TFG_Videojocs;
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
        PlaySFX("Alarm");
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
            if (sound.audioClip != null)
            {
                sound.name = sound.audioClip.name;
            }
        }
    }

    public void PlayMusic(string name)
    {
        ACC_Sound accSound = Array.Find(musicSounds, x => x.name == name);
        if (accSound != null)
        {
            musicSource.clip = accSound.audioClip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name)
    {
        ACC_Sound accSound = Array.Find(sfxSounds, x => x.name == name);
        if (accSound != null)
        {
            ACC_AccessibilityManager.Instance.AudioAccessibilityManager().PlayVisualNotification(accSound);
            sfxSource.clip = accSound.audioClip;
            sfxSource.Play();
        }
    }

    public ACC_Sound[] GetSFXSounds()
    {
        return sfxSounds;
    }
}
