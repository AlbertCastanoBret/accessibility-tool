using System;
using System.Collections.Generic;
using System.Linq;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TFG_Videojocs.ACC_Sound
{
    public class ACC_AudioManager: MonoBehaviour
    {
        private GameObject audioSourcesContainer;
        private bool enabled;
        [HideInInspector] public ACC_SerializableDictiornary<int, ACC_AudioSourceData> audioSources = new ();
        [HideInInspector] public ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, AudioClip>> audioClips = new ();

        private void Awake()
        {
            audioSourcesContainer = transform.Find("ACC_AudioSources")?.gameObject;
        }
        
        public void SetAudioManager(bool state)
        {
            if (state) enabled = true;
            else
            {
                StopAllSounds();
                enabled = false;
            }
        }

        public void PlaySound(string audioSource, string audioClip)
        {
            if (!enabled) return;
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
            if (!enabled) return;
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
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.Stop();
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void StopAllSounds()
        {
            if (!enabled) return;
            foreach (Transform audioSource in audioSourcesContainer.transform)
            {
                audioSource.GetComponent<AudioSource>().Stop();
            }
        }
        
        public void SetVolume(string audioSource, float volume)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.volume = volume;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetPitch(string audioSource, float pitch)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.pitch = pitch;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetLoop(string audioSource, bool loop)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.loop = loop;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetMute(string audioSource, bool mute)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.mute = mute;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetSpatialBlend(string audioSource, float spatialBlend)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.spatialBlend = spatialBlend;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetReverbZoneMix(string audioSource, float reverbZoneMix)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.reverbZoneMix = reverbZoneMix;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetDopplerLevel(string audioSource, float dopplerLevel)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.dopplerLevel = dopplerLevel;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetSpread(string audioSource, float spread)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.spread = spread;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetPriority(string audioSource, int priority)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.priority = priority;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetMinDistance(string audioSource, float minDistance)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.minDistance = minDistance;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetMaxDistance(string audioSource, float maxDistance)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.maxDistance = maxDistance;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetPanStereo(string audioSource, float panStereo)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.panStereo = panStereo;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetSpatialize(string audioSource, bool spatialize)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.spatialize = spatialize;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetSpatializePostEffects(string audioSource, bool spatializePostEffects)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.spatializePostEffects = spatializePostEffects;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetBypassEffects(string audioSource, bool bypassEffects)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.bypassEffects = bypassEffects;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetBypassListenerEffects(string audioSource, bool bypassListenerEffects)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.bypassListenerEffects = bypassListenerEffects;
            }
            else Debug.LogError("Audio source not found");
        }
        
        public void SetBypassReverbZones(string audioSource, bool bypassReverbZones)
        {
            if (!enabled) return;
            var currentAudioSource = audioSourcesContainer.transform.Find(audioSource).GetComponent<AudioSource>();
            if (audioSources.Items.FirstOrDefault(x => x.value.name == currentAudioSource.GameObject().name) != null)
            {
                currentAudioSource.bypassReverbZones = bypassReverbZones;
            }
            else Debug.LogError("Audio source not found");
        }
    }
}