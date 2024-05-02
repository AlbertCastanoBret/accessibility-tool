using System.Collections.Generic;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

namespace TFG_Videojocs
{
    public enum MultifiunctionalFeatures
    {
        AudioManager
    }
    
    public class ACC_MultifunctionalAccessibility
    {
        private ACC_AudioManager accAudioManager;
        public ACC_MultifunctionalAccessibility()
        {
            accAudioManager = ACC_PrefabHelper.InstantiatePrefabAsChild("Audio", ACC_AccessibilityManager.Instance.gameObject).GetComponent<ACC_AudioManager>();
        }
        
        /// <summary>
        /// Toggles the state of a specified feature within the audio management system.
        /// </summary>
        /// <param name="feature">The feature to be modified.</param>
        /// <param name="state">The state to set for the feature (true for enabled, false for disabled).</param>
        public void SetFeatureState(MultifiunctionalFeatures feature, bool state)
        {
            switch (feature)
            {
                case MultifiunctionalFeatures.AudioManager:
                    accAudioManager.SetAudioManager(state);
                    break;
            }
        }
        
        /// <summary>
        /// Retrieves an AudioSource object based on a specified identifier.
        /// </summary>
        /// <param name="audioSource">The name of the audio source to retrieve.</param>
        /// <returns>The AudioSource corresponding to the given name.</returns>
        public AudioSource GetAudioSource(string audioSource)
        {
            return accAudioManager.GetAudioSource(audioSource);
        }

        /// <summary>
        /// Retrieves a list of all AudioSource objects currently managed by the audio manager.
        /// </summary>
        /// <returns>A list of all AudioSource objects.</returns>
        public List<AudioSource> GetAllAudioSources()
        {
            return accAudioManager.GetAllAudioSources();
        }

        /// <summary>
        /// Plays a sound using the specified audio source and clip.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="audioClip">The name of the audio clip to play.</param>
        public void PlaySound(string audioSource, string audioClip)
        {
            accAudioManager.PlaySound(audioSource, audioClip);
        }
        
        /// <summary>
        /// Plays a sound clip once using the specified audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="audioClip">The name of the audio clip to play.</param>
        public void PlayOneShot(string audioSource, string audioClip)
        {
            accAudioManager.PlayOneShot(audioSource, audioClip);
        }
        
        /// <summary>
        /// Stops playing the sound from the specified audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source to stop.</param>
        public void StopSound(string audioSource)
        {
            accAudioManager.StopSound(audioSource);
        }
        
        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            accAudioManager.StopAllSounds();
        }
        
        /// <summary>
        /// Sets the volume for a specific audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="volume">The volume level to set (range typically from 0.0 to 1.0).</param>
        public void SetVolume(string audioSource, float volume)
        {
            accAudioManager.SetVolume(audioSource, volume);
        }
        
        /// <summary>
        /// Adjusts the pitch of the specified audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="pitch">The pitch level to set.</param>
        public void SetPitch(string audioSource, float pitch)
        {
            accAudioManager.SetPitch(audioSource, pitch);
        }
        
        /// <summary>
        /// Enables or disables looping for the specified audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="loop">Whether to loop the sound (true to loop, false to not loop).</param>
        public void SetLoop(string audioSource, bool loop)
        {
            accAudioManager.SetLoop(audioSource, loop);
        }
        
        /// <summary>
        /// Mutes or unmutes the specified audio source.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="mute">Whether to mute the sound (true to mute, false to unmute).</param>
        public void SetMute(string audioSource, bool mute)
        {
            accAudioManager.SetMute(audioSource, mute);
        }
    }
}