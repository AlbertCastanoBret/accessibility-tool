using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TFG_Videojocs.ACC_Sound
{
    public class ACC_AudioManager: MonoBehaviour
    {
        private GameObject audioSettingsContainer;
        private GameObject audioSourcesContainer;
        private bool isEnabled;
        [HideInInspector] public ACC_SerializableDictiornary<int, ACC_AudioSourceData> audioSources = new ();
        [HideInInspector] public ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, AudioClip>> audioClips = new ();

        private void Awake()
        {
            for(int i=0; i<transform.childCount; i++)
            {
                if(transform.GetChild(i).CompareTag("ACC_AudioSettings"))
                {
                    audioSettingsContainer = transform.GetChild(i).gameObject;
                    for(int j=0; j<audioSettingsContainer.transform.childCount; j++)
                    {
                        var scroll = audioSettingsContainer.transform.GetChild(j);
                        if(scroll.CompareTag("ACC_Scroll"))
                        {
                            for (int k = 0; k < scroll.childCount; k++)
                            {
                                var scrollList = scroll.GetChild(k);
                                if (scrollList.CompareTag("ACC_ScrollList"))
                                {
                                    for (int l = 0; l < scrollList.childCount; l++)
                                    {
                                        var audioSource = scrollList.GetChild(l);
                                        var slider = audioSource.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>();
                                        slider.onValueChanged.AddListener(delegate { SetVolume(audioSource.name, slider.value); });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            audioSourcesContainer = transform.Find("ACC_AudioSources")?.gameObject;
            
        }
        
        public void ShowAudioSettingsMenu(bool state)
        {
            if (audioSettingsContainer != null)
            {
                audioSettingsContainer.SetActive(state);
            }
        }
        public void SetAudioManager(bool state)
        {
            if (state) isEnabled = true;
            else
            {
                StopAllSounds();
                isEnabled = false;
            }
        }
        public AudioSource GetAudioSource(string audioSource)
        {
            if (!isEnabled) return null;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                return currentAudioSource;
            }
            Debug.LogError("Audio source not found");
            return null;
        }
        public List<AudioSource> GetAllAudioSources()
        {
            if (!isEnabled) return null;
            List<AudioSource> audioSourcesList = new ();
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                audioSourcesList.Add(audioSource.GetComponent<AudioSource>());
            }
            return audioSourcesList;
        }
        public void PlaySound(string audioSource, string audioClip)
        {
            if (!isEnabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                var indexCurrentAudioSource = audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name)!.key;
                if (audioClips.Items[indexCurrentAudioSource].value.Items
                        .FirstOrDefault(x => x.value.name == audioClip) != null)
                {
                    var currentAudioClip = audioClips.Items[indexCurrentAudioSource].value.Items.FirstOrDefault(x => x.value.name == audioClip)?.value;
                    currentAudioSource.clip = currentAudioClip;
                    currentAudioSource.Play();
                }
                else Debug.LogError("Audio clip not found");
            }
            else Debug.LogError("Audio source not found");
        }
        public void PlayOneShot(string audioSource, string audioClip)
        {
            if (!isEnabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                var indexCurrentAudioSource = audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name)!.key;
                if (audioClips.Items[indexCurrentAudioSource].value.Items
                        .FirstOrDefault(x => x.value.name == audioClip) != null)
                {
                    var currentAudioClip = audioClips.Items[indexCurrentAudioSource].value.Items.FirstOrDefault(x => x.value.name == audioClip)?.value;
                    currentAudioSource.PlayOneShot(currentAudioClip);
                }
                else Debug.LogError("Audio clip not found");
            }
            else Debug.LogError("Audio source not found");
        }
        public void StopSound(string audioSource)
        {
            if (!isEnabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.Stop();
            }
            else Debug.LogError("Audio source not found");
        }
        public void StopAllSounds()
        {
            if (!isEnabled) return;
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                audioSource.GetComponent<AudioSource>().Stop();
            }
        }
        public void SetVolume(string audioSource, float volume)
        {
            if (!isEnabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.volume = volume;
                PlayerPrefs.SetFloat(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource, volume);
                PlayerPrefs.Save();
            }
            else Debug.LogError("Audio source not found");
        }
        public float GetVolume(string audioSource)
        {
            if (!isEnabled) return 0;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource))
                {
                    return PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource);
                }
                return currentAudioSource.volume;
            }
            Debug.LogError("Audio source not found");
            return 0;
        }
        public void SetAllVolumes()
        {
            if (!isEnabled) return;
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                var currentAudioSource = audioSource.GetComponent<AudioSource>();
                if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
                {
                    if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource.name))
                    {
                        currentAudioSource.volume = PlayerPrefs.GetFloat(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource.name);
                    }
                }
            }
        }
        
        // public void SetPitch(string audioSource, float pitch)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.pitch = pitch;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetLoop(string audioSource, bool loop)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.loop = loop;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetMute(string audioSource, bool mute)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.mute = mute;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetSpatialBlend(string audioSource, float spatialBlend)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.spatialBlend = spatialBlend;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetReverbZoneMix(string audioSource, float reverbZoneMix)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.reverbZoneMix = reverbZoneMix;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetDopplerLevel(string audioSource, float dopplerLevel)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.dopplerLevel = dopplerLevel;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetSpread(string audioSource, float spread)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.spread = spread;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetPriority(string audioSource, int priority)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.priority = priority;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetMinDistance(string audioSource, float minDistance)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.minDistance = minDistance;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetMaxDistance(string audioSource, float maxDistance)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.maxDistance = maxDistance;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetPanStereo(string audioSource, float panStereo)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.panStereo = panStereo;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetSpatialize(string audioSource, bool spatialize)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.spatialize = spatialize;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetSpatializePostEffects(string audioSource, bool spatializePostEffects)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.spatializePostEffects = spatializePostEffects;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetBypassEffects(string audioSource, bool bypassEffects)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.bypassEffects = bypassEffects;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetBypassListenerEffects(string audioSource, bool bypassListenerEffects)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.bypassListenerEffects = bypassListenerEffects;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
        //
        // public void SetBypassReverbZones(string audioSource, bool bypassReverbZones)
        // {
        //     if (!isEnabled) return;
        //     var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
        //     if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
        //     {
        //         currentAudioSource.bypassReverbZones = bypassReverbZones;
        //     }
        //     else Debug.LogError("Audio source not found");
        // }
    }
}