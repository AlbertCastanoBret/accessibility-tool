# Introduction
This manual provides the necessary instructions to use the accessibility tool developed for Unity. This tool allows the implementation of various accessibility measures such as control remapping, high contrast mode, subtitles, and visual notifications, as well as audio management. Below are the steps to configure and use these features.

## Initialization of the Accessibility Manager
1. **Open the Initialization Window:**
   - In the Unity interface, go to `Window > Accessibility Tool`.
   - Click the `Create` button.
   
   This will create the Accessibility Manager object in the scene. You can also do this from the main window of the asset by clicking the `Refresh Accessibility Manager` button.
   
2. **Configure the InputActionAsset:**
   - Select the `Accessibility Manager` object in the hierarchy.
   - Go to the `Inspector` and find the `Remap Controls Asset` field.
   - Add the `InputActionAsset` you want to use in the scene.
   
   This enables the control remapping accessibility measure.

## High Contrast Mode
1. **Configure High Contrast Mode:**
   - Go to the `VisualAccessibility` section in the main window of the tool.
   - Select the `Settings` option in the High Contrast box.
   - Enable the `Shaders Added` option.
   
   From here, the high contrast mode will be activated and ready to use.

## Subtitles and Visual Notifications
1. **Configure Subtitles:**
   - Configure subtitles from the corresponding floating window.
   - Once configured, to call a subtitle, use the following function:
     ```csharp
     public void PlaySubtitle(string name)
     ```
     Replace `ConfigurationName` with the name of the subtitle configuration.
     
   Ensure that the subtitle functionality is enabled.
   
2. **Configure Visual Notifications:**
   - Configure visual notifications from the corresponding floating window.
   - To call a visual notification, use the following function:
     ```csharp
     public void PlayVisualNotification(string visualNotification)
     ```
     Replace `ConfigurationName` with the name of the visual notification configuration.
     
   Ensure that both the visual notification functionality and the AudioManager state are enabled.

## Control Remapping
1. **Configure the InputActionAsset:**
   - Configure the `InputActionAsset` according to your needs.
   - If you want to use the template provided by the asset, open the control scheme configuration window.
   - Select the actions you want to be remappable.
   
   Once done, the template will be properly configured. Open the control remapping menu to verify that it works correctly. You can also reconfigure actions using other API functions. Do not forget to enable the control remapping measure state.

## AudioManager Management
1. **Configure Audio Sources:**
   - Configure the audio sources and their respective audio clips.
   - In the AudioManager template, a slider will appear to change the volume of each audio source.
   
   This only works for 2D audio sources. 3D audio sources will be created as prefabs and can be accessed through the `Resources/ACC_Prefabs/ACC_Audio/ACC_3DAudioSources` folder. When these sources are added to the scene, they will have a unique, non-modifiable name that will be used as a parameter for 3D audio source functions.
   
2. **Play Sounds:**
   - To play a sound, use the following function:
     ```csharp
     public void PlaySound(string audioSource, string audioClip, bool showVisualNotification = true);
     ```
   - To play a 3D sound, use the following function:
     ```csharp
     public void Play3DSound(string audioSource, string audioClip, string gameObject);
     ```
     
   Replace the parameters according to your needs. Ensure that the measure state is enabled.

## Additional Options and Configuration
The API offers many more functions for changing configurations, data persistence, etc. You can access the [API documentation](/api/ACC_API.html) to understand what each function does.

When a configuration value of a measure is changed, a PlayerPref is created. To remove configured PlayerPrefs, use the buttons available in the `AccessibilityManager` editor. To enable and disable measures, use the editor as this does not create PlayerPrefs. From the same editor, you can also display the configuration menu of each accessibility measure.
