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
            accAudioManager = ACC_PrefabHelper.InstantiatePrefabAsChild("Audio", ACC_AccessibilityManager.Instance.accCanvas).GetComponent<ACC_AudioManager>();
        }
        
        internal void InitializeState(MultifiunctionalFeatures feature, bool state)
        {
            switch (feature)
            {
                case MultifiunctionalFeatures.AudioManager:
                    accAudioManager.InitializeAudioManager(state);
                    break;
            }
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
        /// Loads and applies the user's accessibility preferences related to multifunctional features.
        /// </summary>
        public void LoadUserPreferences()
        {
            LoadPreferencesAudioSourceVolumes();
        }

        #region AudioManager

        /// <summary>
        /// Enables the audio manager menu through the associated high contrast manager.
        /// </summary>
        public void EnableAudioManagerMenu()
        {
            accAudioManager.EnableAudioManagerMenu();
        }
        
        /// <summary>
        /// Disables the high contrast menu through the associated high contrast manager.
        /// </summary>
        public void DisableAudioManagerMenu()
        {
            accAudioManager.DisableAudioManagerMenu();
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
        /// Retrieves the volume level for a specified audio source using the AudioManager.
        /// If a PlayerPrefs audioSourceKey exists for the audio source, it returns the saved value.
        /// </summary>
        /// <param name="audioSource">The name of the audio source for which the volume is queried.</param>
        /// <returns>The volume level of the specified audio source as a float.</returns>
        public float GetVolume(string audioSource)
        {
            return accAudioManager.GetVolume(audioSource);
        }
        
        /// <summary>
        /// Resets the volumes of all audio sources managed by the AudioManager.
        /// This function calls the ResetAudioManagerConfiguration method on the AudioManager instance to revert audio source volumes to their default settings.
        /// </summary>
        public void ResetAudioManagerConfiguration()
        {
            accAudioManager.ResetAudioManagerConfiguration();
        }
        
        private void LoadPreferencesAudioSourceVolumes()
        {
            accAudioManager.SetAllVolumes();
        }
        
        #endregion
    }
}