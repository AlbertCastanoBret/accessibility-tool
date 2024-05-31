using System.Collections.Generic;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Utilities;
using UnityEngine;

namespace ACC_API
{
    /// <summary>
    /// Enum representing different multifunctional accessibility features.
    /// </summary>
    public enum MultifunctionalFeatures
    {
        /// <summary>
        /// Features related to managing audio accessibility.
        /// </summary>
        AudioManager
    }
    
    public class ACC_MultifunctionalAccessibility
    {
        private ACC_AudioManager accAudioManager;
        internal ACC_MultifunctionalAccessibility()
        {
            accAudioManager = ACC_PrefabHelper.InstantiatePrefabAsChild("Audio", ACC_AccessibilityManager.Instance.accCanvas).GetComponent<ACC_AudioManager>();
        }
        
        internal void InitializeState(MultifunctionalFeatures feature, bool state)
        {
            switch (feature)
            {
                case MultifunctionalFeatures.AudioManager:
                    accAudioManager.InitializeAudioManager(state);
                    break;
            }
        }
        
        /// <summary>
        /// Toggles the state of a specified feature within the audio management system.
        /// </summary>
        /// <param name="feature">The feature to be modified.</param>
        /// <param name="state">The state to set for the feature (true for enabled, false for disabled).</param>
        public void SetFeatureState(MultifunctionalFeatures feature, bool state)
        {
            switch (feature)
            {
                case MultifunctionalFeatures.AudioManager:
                    accAudioManager.SetAudioManager(state);
                    break;
            }
        }
        
        /// <summary>
        /// Retrieves the enabled state of a specified multifunctional feature.
        /// </summary>
        /// <param name="feature">The multifunctional feature to check, e.g., audio manager.</param>
        /// <returns>True if the specified feature is enabled, false otherwise.</returns>
        public bool GetFeatureState(MultifunctionalFeatures feature)
        {
            switch (feature)
            {
                case MultifunctionalFeatures.AudioManager:
                    if (PlayerPrefs.HasKey(ACC_AccessibilitySettingsKeys.AudioManagerEnabled))
                    {
                        return PlayerPrefs.GetInt(ACC_AccessibilitySettingsKeys.AudioManagerEnabled) == 1;
                    }
                    return ACC_AccessibilityManager.Instance.audioManagerEnabled;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Resets the state of the specified multifunctional feature to its default settings.
        /// </summary>
        /// <param name="feature">The multifunctional feature to reset (e.g., audio manager).</param>
        public void ResetFeatureState(MultifunctionalFeatures feature)
        {
            switch (feature)
            {
                case MultifunctionalFeatures.AudioManager:
                    accAudioManager.ResetAudioManagerState();
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
            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowSubtitles(false);
            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowVisualNotification(false);
            accAudioManager.EnableAudioManagerMenu();
        }
        
        /// <summary>
        /// Disables the high contrast menu through the associated high contrast manager.
        /// </summary>
        public void DisableAudioManagerMenu()
        {
            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowSubtitles(true);
            ACC_AccessibilityManager.Instance.AudioAccessibility.ShowVisualNotification(true);
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
        /// Retrieves a 3D AudioSource object based on a specified GameObject identifier.
        /// </summary>
        /// <param name="gameObject">The name of the GameObject to retrieve the 3D audio source for.</param>
        /// <returns>The 3D AudioSource corresponding to the given GameObject.</returns>
        public AudioSource Get3DAudioSource(string gameObject)
        {
            return accAudioManager.Get3DAudioSource(gameObject);
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
        /// Retrieves a list of all 3D AudioSource objects currently managed by the audio manager.
        /// </summary>
        /// <returns>A list of all 3D AudioSource objects.</returns>
        public List<AudioSource> GetAll3DAudioSources()
        {
            return accAudioManager.GetAll3DAudioSources();
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
        /// Plays a 3D sound using the specified audio source, audio clip, and GameObject.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="audioClip">The name of the audio clip to play.</param>
        /// <param name="gameObject">The name of the GameObject to associate the 3D sound with.</param>
        public void Play3DSound(string audioSource, string audioClip, string gameObject)
        {
            accAudioManager.Play3DSound(audioSource, audioClip, gameObject);
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
        /// Plays a 3D sound clip once using the specified audio source and GameObject.
        /// </summary>
        /// <param name="audioSource">The name of the audio source.</param>
        /// <param name="audioClip">The name of the audio clip to play once.</param>
        /// <param name="gameObject">The name of the GameObject to associate the 3D sound with.</param>
        public void Play3DOneShot(string audioSource, string audioClip, string gameObject)
        {
            accAudioManager.Play3DOneShot(audioSource, audioClip, gameObject);
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
        /// Stops playing the 3D sound associated with the specified GameObject.
        /// </summary>
        /// <param name="gameObject">The name of the GameObject whose 3D sound should stop playing.</param>
        public void Stop3DSound(string gameObject)
        {
            accAudioManager.Stop3DSound(gameObject);
        }
        
        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            accAudioManager.StopAllSounds();
        }
        
        /// <summary>
        /// Stops all currently playing 3D sounds.
        /// </summary>
        public void StopAll3DSounds()
        {
            accAudioManager.StopAll3DSounds();
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
        /// Configures all audio source volumes to the user's saved preferences.
        /// </summary>
        public void LoadPreferencesAudioSourceVolumes()
        {
            accAudioManager.SetAllVolumes();
        }
        
        /// <summary>
        /// Resets the volumes of all audio sources managed by the AudioManager.
        /// This function calls the ResetAudioManagerConfiguration method on the AudioManager instance to revert audio source volumes to their default settings.
        /// </summary>
        public void ResetAudioManagerConfiguration()
        {
            accAudioManager.ResetAudioManagerConfiguration();
        }
        
        #endregion
    }
}