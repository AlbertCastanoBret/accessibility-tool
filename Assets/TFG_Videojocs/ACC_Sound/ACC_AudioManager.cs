using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TFG_Videojocs;
using Unity.Collections;
using UnityEngine;

public class ACC_AudioManager : MonoBehaviour
{
    public static ACC_AudioManager Instance;
    
    [SerializeField] private List<ACC_Sound> musicSounds, sfxSounds;
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
        PlaySFX("Alarm");
    }

    private void OnValidate()
    {
        UpdateSoundNames(musicSounds);
        UpdateSoundNames(sfxSounds);
        OnSoundsChanged?.Invoke();
    }
    
    private void UpdateSoundNames(List<ACC_Sound> sounds)
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
        ACC_Sound accSound = musicSounds.Find(x => x.name == name);
        if (accSound != null)
        {
            musicSource.clip = accSound.audioClip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name)
    {
        ACC_Sound accSound = sfxSounds.Find( x => x.name == name);
        if (accSound != null)
        {
            ACC_AccessibilityManager.Instance.AudioAccessibilityManager().PlayVisualNotification(accSound);
            sfxSource.clip = accSound.audioClip;
            sfxSource.Play();
        }
    }

    public List<ACC_Sound> GetMusicSounds()
    {
        return musicSounds.GroupBy(sound => sound.name).Select(group => group.First()).ToList();
    }
    
    public List<ACC_Sound> GetSFXSounds()
    {
        return sfxSounds.GroupBy(sound => sound.name).Select(group => group.First()).ToList();
    }

    public void AddMusicSound(ACC_Sound accSound)
    {
        musicSounds.Add(accSound);
    }
    
    public void AddMusicSound(AudioClip audioClip)
    {
        musicSounds.Add(new ACC_Sound(audioClip.name, audioClip));
    }
    
    public void AddSFXSound(ACC_Sound accSound)
    {
        sfxSounds.Add(accSound);
        OnSoundsChanged?.Invoke();
    }

    public void AddSFXSound(AudioClip audioClip)
    {
        sfxSounds.Add(new ACC_Sound(audioClip.name, audioClip));
        OnSoundsChanged?.Invoke();
    }
}
