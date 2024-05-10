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
        private GameObject audioSettings, audioSettingsScrollList;
        private GameObject audioSourcesContainer;
        private bool isEnabled;
        public ACC_SerializableDictiornary<int, ACC_AudioSourceData> audioSources = new ();
        public ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, AudioClip>> audioClips = new ();
        
        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("ACC_Prefab"))
                {
                    audioSettings = child.gameObject;
                    foreach (Transform settingComponent in audioSettings.transform)
                    {
                        if (settingComponent.CompareTag("ACC_Scroll"))
                        {
                            foreach (Transform scrollComponent in settingComponent)
                            {
                                if (scrollComponent.CompareTag("ACC_ScrollList")) 
                                    audioSettingsScrollList = scrollComponent.gameObject;
                            }
                        }
                    }
                }
                if (child.CompareTag("ACC_AudioSources")) audioSourcesContainer = child.gameObject;
            }
        }
        private void Start()
        {
            foreach (Transform child in transform)
            {
                if(child.CompareTag("ACC_Prefab"))
                {
                    foreach (Transform settingComponent in audioSettings.transform)
                    {
                        if(settingComponent.CompareTag("ACC_Scroll"))
                        {
                            foreach (Transform scrollComponent in settingComponent)
                            {
                                if (scrollComponent.CompareTag("ACC_ScrollList"))
                                {
                                    foreach (Transform settingsOption in scrollComponent)
                                    {
                                        var slider = settingsOption.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>();
                                        slider.onValueChanged.AddListener(delegate 
                                            { ACC_AccessibilityManager.Instance.MultifunctionalAccessibility.SetVolume(settingsOption.name, slider.value); });
                                    }
                                }
                            }
                        }
                        
                        if(settingComponent.CompareTag("ACC_Button"))
                        {
                            var button = settingComponent.GetComponent<Button>();
                            button.onClick.AddListener(ResetAudioManagerConfiguration);
                        }
                    }
                }
            }
        }
        
        public void InitializeAudioManager(bool state)
        {
            if (state) isEnabled = true;
            else
            {
                StopAllSounds();
                isEnabled = false;
            }
            
            ACC_AccessibilityManager.Instance.audioManagerEnabled = state;
        }
        public void SetAudioManager(bool state)
        {
            if (state) isEnabled = true;
            else
            {
                StopAllSounds();
                isEnabled = false;
            }
            
            ACC_AccessibilityManager.Instance.audioManagerEnabled = state;
            PlayerPrefs.SetInt(ACC_AccessibilitySettingsKeys.AudioManagerEnabled, state ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void ResetAudioManagerState()
        {
            StopAllSounds();
            isEnabled = false;
            
            ACC_AccessibilityManager.Instance.audioManagerEnabled = false;
            PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled);
            PlayerPrefs.Save();
        }
        
        public void EnableAudioManagerMenu()
        {
            if (audioSettings != null)
            {
                audioSettings.SetActive(true);
            }
        }
        public void DisableAudioManagerMenu()
        {
            if (audioSettings != null)
            {
                audioSettings.SetActive(false);
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
            if (audioSourcesContainer.transform.Find(audioSource) == null)
            {
                Debug.LogError("Audio source not found");
                return;
            }
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
                    ACC_AccessibilityManager.Instance.AudioAccessibility.PlayVisualNotification(audioSource, audioClip);
                }
                else Debug.LogError("Audio clip not found");
            }
            else Debug.LogError("Audio source not found");
        }
        public void PlayOneShot(string audioSource, string audioClip)
        {
            if (!isEnabled) return;
            if (audioSourcesContainer.transform.Find(audioSource) == null)
            {
                Debug.LogError("Audio source not found");
                return;
            }
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                var indexCurrentAudioSource = audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name)!.key;
                if (audioClips.Items[indexCurrentAudioSource].value.Items
                        .FirstOrDefault(x => x.value.name == audioClip) != null)
                {
                    var currentAudioClip = audioClips.Items[indexCurrentAudioSource].value.Items.FirstOrDefault(x => x.value.name == audioClip)?.value;
                    currentAudioSource.PlayOneShot(currentAudioClip);
                    ACC_AccessibilityManager.Instance.AudioAccessibility.PlayVisualNotification(audioSource, audioClip);
                }
                else Debug.LogError("Audio clip not found");
            }
            else Debug.LogError("Audio source not found");
        }
        public void StopSound(string audioSource)
        {
            if (!isEnabled) return;
            if (audioSourcesContainer.transform.Find(audioSource) == null)
            {
                Debug.LogError("Audio source not found");
                return;
            }
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
            if (audioSourcesContainer.transform.Find(audioSource) == null)
            {
                Debug.LogError("Audio source not found");
                return;
            }
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
            if (audioSourcesContainer.transform.Find(audioSource) == null)
            {
                Debug.LogError("Audio source not found");
                return 0;
            }
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
            if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled))
            {
                SetAudioManager(PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.AudioManagerEnabled) == 1);
            }
            
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                var currentAudioSource = audioSource.GetComponent<AudioSource>();
                if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
                {
                    currentAudioSource.volume = GetVolume(audioSource.name);
                }
            }

            if (audioSettingsScrollList != null)
            {
                foreach (Transform settingsOption in audioSettingsScrollList.transform)
                {
                    var slider = settingsOption.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>();
                    slider.value = GetVolume(settingsOption.name);
                }
            }
        }
        public void ResetAudioManagerConfiguration()
        {
            if (!isEnabled) return;
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                var currentAudioSource = audioSource.GetComponent<AudioSource>();
                if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
                {
                    currentAudioSource.volume = audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name)!.value.volume;
                    PlayerPrefs.DeleteKey(ACC_AccessibilitySettingsKeys.AudioSourceVolume + audioSource.name);
                    PlayerPrefs.Save();
                }
            }

            if (audioSettingsScrollList != null)
            {
                foreach (Transform settingsOption in audioSettingsScrollList.transform)
                {
                    var slider = settingsOption.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>();
                    slider.value = audioSources.Items.FirstOrDefault(x => x.value.name == settingsOption.name)!.value.volume;
                }
            }
        }
    }
}