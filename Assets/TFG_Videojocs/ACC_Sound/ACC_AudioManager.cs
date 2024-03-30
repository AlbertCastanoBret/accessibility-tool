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
    private List<ACC_Sound> previousSfxSounds;
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
        CheckIfSoundIsEliminated();
        OnSoundsChanged?.Invoke();
    }

    private void Reset()
    {
        musicSounds = new List<ACC_Sound>();
        sfxSounds = new List<ACC_Sound>();
        previousSfxSounds = new List<ACC_Sound>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();
        sfxSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void UpdateSoundNames(List<ACC_Sound> sounds)
    {
        if (sounds == null) return;
        foreach (var sound in sounds)
        {
            if (sound.audioClip != null)
            {
                sound.name = sound.audioClip.name;
            }
        }
    }

    private void CheckIfSoundIsEliminated()
    {
        if (sfxSounds != previousSfxSounds && sfxSounds!=null && previousSfxSounds !=null)
        {
            foreach (var prevSound in previousSfxSounds)
            {
                if (!sfxSounds.Contains(prevSound))
                {
                    Debug.Log(prevSound.name);
                    ACC_JSONHelper.RemoveItemFromListInFile<ACC_VisualNotificationData, ACC_Sound>(
                        "/ACC_JSONVisualNotification",
                        data => data.soundsList,
                        (itemInList, itemToMatch) => itemInList.name == itemToMatch.name,
                        prevSound
                    );
                }
            }
            previousSfxSounds = sfxSounds.Select(sound => (ACC_Sound)sound.Clone()).ToList();
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
        OnValidate();
    }

    public void AddSFXSound(AudioClip audioClip)
    {
        sfxSounds.Add(new ACC_Sound(audioClip.name, audioClip));
        OnSoundsChanged?.Invoke();
    }
}
