using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_RemapControls;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TFG_Videojocs.ACC_Utilities
{
    public class ACC_PrefabHelper
    {
        #if UNITY_EDITOR
        public static void CreatePrefab(string feature, string jsonFile = "")
        {
            if(Application.isPlaying) return;
            var gameObject = CreateGameObject(feature, jsonFile);
            
            var folder = "ACC_" + feature + "/";
            var name = "ACC_" + feature + "Manager.prefab";
            if (!string.IsNullOrEmpty(jsonFile) && feature != "Audio") name = "ACC_" + feature + "Manager_" + jsonFile + ".prefab";
            
            Directory.CreateDirectory("Assets/Resources/ACC_Prefabs/" + folder);
            
            var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
            
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
            Object.DestroyImmediate(gameObject, true);
            AssetDatabase.Refresh();
        }
        #endif
        public static GameObject InstantiatePrefabAsChild(string feature, GameObject parent, string jsonFile="")
        {
            var folder = "ACC_" + feature + "/";
            var name = "ACC_" + feature + "Manager";
            if (!string.IsNullOrEmpty(jsonFile) && feature != "Audio") name = "ACC_" + feature + "Manager_" + jsonFile;
            var prefabPath = "ACC_Prefabs/" + folder + name;
            
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.Log("Prefab couldn't be loaded in " + prefabPath);
                return null;
            }

            return GameObject.Instantiate(prefab, parent.transform);
        }
        #if UNITY_EDITOR
        private static GameObject CreateGameObject(string feature, string jsonFile = "")
        {
            string name = "ACC_" + feature + "Manager";
            if (!string.IsNullOrEmpty(jsonFile) && feature != "Audio") name = "ACC_" + feature + "Manager_" + jsonFile;
            bool newPrefab = false;
            GameObject gameObject = GetPrefabWithTag("ACC_" + feature + "Manager", "Assets/Resources/ACC_Prefabs/"+ "ACC_" + feature + "/");
            if(!string.IsNullOrEmpty(jsonFile) && feature != "Audio") gameObject = GetPrefabWithNameAndTag(name, "ACC_" + feature + "Manager", "Assets/Resources/ACC_Prefabs/"+ "ACC_" + feature + "/");
            if (gameObject == null)
            {
                newPrefab = true;
                gameObject = new GameObject(){name = name, tag = "ACC_" + feature + "Manager"};
            }
            
            switch (feature)
            {
                case "Subtitles":
                    gameObject = CreateSubtitleManager(gameObject, newPrefab);
                    break;
                case "VisualNotification":
                    gameObject = CreateVisualNotificationManager(gameObject, newPrefab);
                    break;
                case "HighContrast":
                    gameObject = CreateHighContrastManager(gameObject, newPrefab);
                    break;
                case "RemapControls":
                    gameObject = CreateRemapControlsManager(gameObject, jsonFile, newPrefab);
                    break;
                case "Audio":
                    gameObject = CreateAudioManager(gameObject, jsonFile, newPrefab);
                    break;
            }
            return gameObject;
        }
        public static bool FileNameAlreadyExists(string feature, string name)
        {
            var folder = "ACC_" + feature + "/";
            string filePath = "Assets/Resources/ACC_Prefabs/" + folder + name + ".prefab";
            return File.Exists(filePath);
        }
        
        public static GameObject GetPrefabWithTag(string tag, string directoryPath)
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] {directoryPath});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (gameObject.CompareTag(tag)) return gameObject;
            }
            return null;
        }
        public static GameObject GetPrefabWithNameAndTag(string name, string tag, string directoryPath)
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] {directoryPath});
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (gameObject.CompareTag(tag) && gameObject.name == name) return gameObject;
            }
            return null;
        }
        
        private static GameObject CreateSubtitleManager(GameObject subtitleManager, bool newPrefab)
        {
            if (!newPrefab)
            {
                subtitleManager = GameObject.Instantiate(subtitleManager);
            }
            
            RectTransform subtitleManagerTextRectTransform = subtitleManager.GetComponent<RectTransform>() == null ? subtitleManager.AddComponent<RectTransform>() : subtitleManager.GetComponent<RectTransform>();
            subtitleManagerTextRectTransform.anchorMin = new Vector2(0, 0);
            subtitleManagerTextRectTransform.anchorMax = new Vector2(1, 1);
            subtitleManagerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            subtitleManagerTextRectTransform.anchoredPosition = Vector3.zero;
            subtitleManagerTextRectTransform.sizeDelta = new Vector2(0, 0);

            if (GetChildWithTag(subtitleManager, "ACC_Prefab") == null) GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_Subtitles/ACC_SubtitleSettings.prefab"), subtitleManager.transform, true);

            var subtitleBackground = GetChildWithTag(subtitleManager, "ACC_SubtitleBackground");
            if (subtitleBackground == null)
            {
                subtitleBackground = new GameObject()
                    { name = "ACC_SubtitleBackground", tag = "ACC_SubtitleBackground" };
                subtitleBackground.transform.SetParent(subtitleManager.transform, false);
            }
            
            var backgroundColorImage = subtitleBackground.GetComponent<Image>() ?? subtitleBackground.AddComponent<Image>();
            backgroundColorImage.color = new Color(0, 0, 0, 0);
            
            RectTransform backgroundTextRectTransform = subtitleBackground.GetComponent<RectTransform>() ?? subtitleBackground.AddComponent<RectTransform>();
            backgroundTextRectTransform.anchorMin = new Vector2(0.15f, 0);
            backgroundTextRectTransform.anchorMax = new Vector2(0.85f, 0);
            backgroundTextRectTransform.pivot = new Vector2(0.5f, 0);
            backgroundTextRectTransform.anchoredPosition = new Vector2(0, 70);
            backgroundTextRectTransform.sizeDelta = new Vector2(0, 40);

            var subtitleText = GetChildWithTag(subtitleManager, "ACC_SubtitleText");
            if (subtitleText == null)
            {
                subtitleText = new GameObject() { name = "ACC_SubtitleText", tag = "ACC_SubtitleText" };
                subtitleText.transform.SetParent(subtitleManager.transform, false);
            }

            TextMeshProUGUI subtitleTextMeshProUGUI = subtitleText.GetComponent<TextMeshProUGUI>() ?? subtitleText.AddComponent<TextMeshProUGUI>();
            subtitleTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            subtitleTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
            subtitleTextMeshProUGUI.enableWordWrapping = true;
            subtitleTextMeshProUGUI.color = new Color(0, 0f, 0f, 0);

            RectTransform subtitleTextRectTransform = subtitleText.GetComponent<RectTransform>() ?? subtitleText.AddComponent<RectTransform>();
            subtitleTextRectTransform.anchorMin = new Vector2(0.15f, 0);
            subtitleTextRectTransform.anchorMax = new Vector2(0.85f, 0);
            subtitleTextRectTransform.pivot = new Vector2(0.5f, 0);
            subtitleTextRectTransform.anchoredPosition = new Vector2(0, 70);
            subtitleTextRectTransform.sizeDelta = new Vector2(0, 40);

            var accSubtitlesManager = subtitleManager.GetComponent<ACC_SubtitlesManager>() ?? subtitleManager.AddComponent<ACC_SubtitlesManager>();

            return subtitleManager;
        }
        private static GameObject CreateVisualNotificationManager(GameObject visualNotificationManager, bool newPrefab)
        {
            if (!newPrefab)
            {
                visualNotificationManager = GameObject.Instantiate(visualNotificationManager);
            }
            
            RectTransform visualNotificationManagerTextRectTransform = visualNotificationManager.GetComponent<RectTransform>() == null ? 
                visualNotificationManager.AddComponent<RectTransform>() : visualNotificationManager.GetComponent<RectTransform>();
            visualNotificationManagerTextRectTransform.anchorMin = new Vector2(0, 0);
            visualNotificationManagerTextRectTransform.anchorMax = new Vector2(1, 1);
            visualNotificationManagerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            visualNotificationManagerTextRectTransform.anchoredPosition = Vector3.zero;
            visualNotificationManagerTextRectTransform.sizeDelta = new Vector2(0, 0);
            
            if (GetChildWithTag(visualNotificationManager, "ACC_Prefab") == null) GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_VisualNotification/ACC_VisualNotificationSettings.prefab"), visualNotificationManager.transform, true);
            
            var visualNotificationBackground = GetChildWithTag(visualNotificationManager, "ACC_VisualNotificationBackground");
            if (visualNotificationBackground == null)
            {
                visualNotificationBackground = new GameObject(){name = "ACC_VisualNotificationBackground", tag = "ACC_VisualNotificationBackground"};
                visualNotificationBackground.transform.SetParent(visualNotificationManager.transform, false);
            }
            
            var backgroundColorImage = visualNotificationBackground.GetComponent<Image>() ?? visualNotificationBackground.AddComponent<Image>();
            backgroundColorImage.color = new Color(0, 0, 0, 0);
            
            RectTransform backgroundTextRectTransform = visualNotificationBackground.GetComponent<RectTransform>() ?? visualNotificationBackground.AddComponent<RectTransform>();
            backgroundTextRectTransform.anchorMin = new Vector2(0, 0.5f);
            backgroundTextRectTransform.anchorMax = new Vector2(1, 0.5f);
            backgroundTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            backgroundTextRectTransform.anchoredPosition = new Vector2(0, 0);
            backgroundTextRectTransform.sizeDelta = new Vector2(0, 100);
            
            var visualNotificationText = GetChildWithTag(visualNotificationManager, "ACC_VisualNotificationText");
            if (visualNotificationText == null)
            {
                visualNotificationText = new GameObject(){name = "ACC_VisualNotificationText", tag = "ACC_VisualNotificationText"};
                visualNotificationText.transform.SetParent(visualNotificationManager.transform, false);
            }
            
            TextMeshProUGUI visualNotificationTextMeshProUGUI = visualNotificationText.GetComponent<TextMeshProUGUI>() ?? visualNotificationText.AddComponent<TextMeshProUGUI>();
            visualNotificationTextMeshProUGUI.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            visualNotificationTextMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
            visualNotificationTextMeshProUGUI.enableWordWrapping = true;
            visualNotificationTextMeshProUGUI.color = new Color(0, 0, 0, 0);
            
            RectTransform visualNotificationTextRectTransform = visualNotificationText.GetComponent<RectTransform>() ?? visualNotificationText.AddComponent<RectTransform>();
            visualNotificationTextRectTransform.anchorMin = new Vector2(0, 0.5f);
            visualNotificationTextRectTransform.anchorMax = new Vector2(1, 0.5f);
            visualNotificationTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            visualNotificationTextRectTransform.anchoredPosition = new Vector2(0, 0);
            visualNotificationTextRectTransform.sizeDelta = new Vector2(0, 100);
    
            var accVisualNotificationManager = visualNotificationManager.GetComponent<ACC_VisualNotificationManager>() ?? visualNotificationManager.AddComponent<ACC_VisualNotificationManager>();

            return visualNotificationManager;
        }
        private static GameObject CreateHighContrastManager(GameObject highContrastManager, bool newPrefab)
        {
            if (!newPrefab)
            {
                highContrastManager = GameObject.Instantiate(highContrastManager);
            }
            
            RectTransform subtitleManagerTextRectTransform = highContrastManager.GetComponent<RectTransform>() == null ? highContrastManager.AddComponent<RectTransform>() : highContrastManager.GetComponent<RectTransform>();
            subtitleManagerTextRectTransform.anchorMin = new Vector2(0, 0);
            subtitleManagerTextRectTransform.anchorMax = new Vector2(1, 1);
            subtitleManagerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            subtitleManagerTextRectTransform.anchoredPosition = Vector3.zero;
            subtitleManagerTextRectTransform.sizeDelta = new Vector2(0, 0);
            
            if (GetChildWithTag(highContrastManager, "ACC_Prefab") == null) GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_HighContrast/ACC_HighContrastSettings.prefab"), highContrastManager.transform, true);
            
            var accHighContrastManager = highContrastManager.GetComponent<ACC_HighContrastManager>() ?? highContrastManager.AddComponent<ACC_HighContrastManager>();
            
            return highContrastManager;
        }
        private static GameObject CreateRemapControlsManager(GameObject remapControlsManager, string jsonFile, bool newPrefab)
        {
            if (!newPrefab)
            {
                remapControlsManager = Object.Instantiate(remapControlsManager);
            }
            
            RectTransform remapControlsManagerRectTransform = remapControlsManager.GetComponent<RectTransform>() == null ? 
                remapControlsManager.AddComponent<RectTransform>() : remapControlsManager.GetComponent<RectTransform>();
            remapControlsManagerRectTransform.anchorMin = new Vector2(0, 0);
            remapControlsManagerRectTransform.anchorMax = new Vector2(1, 1);
            remapControlsManagerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            remapControlsManagerRectTransform.anchoredPosition = Vector3.zero;
            remapControlsManagerRectTransform.sizeDelta = new Vector2(0, 0);
    
            var remapControlsManagerComponent = remapControlsManager.GetComponent<ACC_RemapControlsManager>() == null ? 
                remapControlsManager.AddComponent<ACC_RemapControlsManager>() : remapControlsManager.GetComponent<ACC_RemapControlsManager>();
            
            remapControlsManagerComponent.defaultMenu = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/ACC_DeviceManager.prefab");
            remapControlsManagerComponent.rebindUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/Rebinding UI/RebindUIPrefab.prefab");
            remapControlsManagerComponent.rebindOverlay = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/ACC_WaitForInputPanel.prefab");
            
            var waitForInputPanel = GetChildWithTag(remapControlsManager, "ACC_WaitForInputPanel");
            if (waitForInputPanel == null)
            {
               waitForInputPanel = Object.Instantiate(remapControlsManagerComponent.rebindOverlay, remapControlsManager.transform, true);
            }
            waitForInputPanel.transform.localScale = new Vector3(1, 1, 1);
            waitForInputPanel.transform.localPosition = new Vector3(0, 0, 0);
            
            try
            {
                var loadedData = ACC_JSONHelper.LoadJson<ACC_ControlSchemeData>("ACC_ControlSchemesConfiguration/" + jsonFile);
                SetControlSchemes(loadedData, remapControlsManager);
            }
            #pragma warning disable 0168
            catch (Exception e)
            {
            }
            #pragma warning restore 0168
            
            return remapControlsManager;
        }
        private static void SetControlSchemes(ACC_ControlSchemeData loadedData, GameObject remapControlsManager)
        {
            var defaultMenu = remapControlsManager.GetComponent<ACC_RemapControlsManager>().defaultMenu;
            var devices = loadedData.inputActionAsset.controlSchemes
                .Select(scheme => 
                {
                    return scheme.deviceRequirements
                        .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                        .Distinct()
                        .OrderBy(device => device)
                        .Aggregate((current, next) => current + ", " + next);
                })
                .Where(device => device != null)
                .Distinct()
                .ToList();
            
            bool first = true;

            foreach (var device in devices)
            {
                var deviceManager = GetChildWithNameAndTag(remapControlsManager, device, "ACC_Prefab");
                if (deviceManager == null)
                {
                    deviceManager = Object.Instantiate(defaultMenu, remapControlsManager.transform, true);
                }
                deviceManager.transform.SetSiblingIndex(devices.IndexOf(device)+1);
                
                deviceManager.name = device;
                RectTransform rectTransform = deviceManager.GetComponent<RectTransform>() == null ? deviceManager.AddComponent<RectTransform>() : deviceManager.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.localPosition = new Vector3(0, 0, 0);
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                var title = deviceManager.transform.Find("Title").GetComponent<TextMeshProUGUI>() == null ? deviceManager.transform.Find("Title").gameObject.AddComponent<TextMeshProUGUI>() : deviceManager.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                title.text =
                    "CUSTOMIZE CONTROLS - " + device.ToUpper();
                
                deviceManager.SetActive(first);
                first = false;
                
                var controlSchemes = loadedData.inputActionAsset.controlSchemes
                    .Where(scheme => String.Join(", ", scheme.deviceRequirements
                        .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                        .Distinct()
                        .OrderBy(d => d)) == device)
                    .Select(scheme => scheme.name)
                    .ToList();
                
                deviceManager.transform.Find("ControlSchemeSelector").Find("CurrentControlSchemeParent").Find("CurrentControlScheme").GetComponent<TextMeshProUGUI>().text =
                    controlSchemes[0];
                
                SetBindings(loadedData, deviceManager, controlSchemes, remapControlsManager);
            }
        }
        private static void SetBindings(ACC_ControlSchemeData loadedData, GameObject deviceManager,
            List<string> controlSchemes, GameObject remapControlsManager)
        {
            var rebindUIPrefab = remapControlsManager.GetComponent<ACC_RemapControlsManager>().rebindUIPrefab;
            var rebindOverlay = remapControlsManager.GetComponent<ACC_RemapControlsManager>().rebindOverlay;
            var rebindPrompt = remapControlsManager.transform.GetChild(0).Find("WaitForInputText").GetComponent<TextMeshProUGUI>();
            
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(loadedData.inputActionAsset));
            List<InputActionReference> inputActionReferences = new List<InputActionReference>();
            foreach (Object obj in subAssets) {
                if (obj is InputActionReference inputActionReference && (inputActionReference.hideFlags & HideFlags.HideInHierarchy) == 0) {
                    inputActionReferences.Add(inputActionReference);
                }
            }
            
            bool first = true;
            foreach (var controlScheme in controlSchemes)
            {
                var rebindingsScroll = deviceManager.transform.Find("ACC_RebindsScroll");

                var controlSchemeParent = GetChildWithNameAndTag(rebindingsScroll.gameObject, controlScheme, "ACC_ScrollList");
                if (controlSchemeParent == null)
                {
                    controlSchemeParent = new GameObject
                    {
                        name = controlScheme,
                        tag = "ACC_ScrollList"
                    };
                }
                var rectTransform = controlSchemeParent.GetComponent<RectTransform>() == null ? controlSchemeParent.AddComponent<RectTransform>() : controlSchemeParent.GetComponent<RectTransform>();

                rectTransform.SetParent(rebindingsScroll);
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(0, 0);

                if (first)
                {
                    rebindingsScroll.GetComponent<ScrollRect>().content = rectTransform;
                    first = false;
                }
                else controlSchemeParent.SetActive(false);

                var verticalLayoutGroup = controlSchemeParent.GetComponent<VerticalLayoutGroup>() == null ? controlSchemeParent.AddComponent<VerticalLayoutGroup>() : controlSchemeParent.GetComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.spacing = 0;

                var contentSizeFitter = controlSchemeParent.GetComponent<ContentSizeFitter>() == null ? controlSchemeParent.AddComponent<ContentSizeFitter>() : controlSchemeParent.GetComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                foreach (var binding in loadedData.bindingsList.Items)
                {
                    if (controlScheme == binding.key.controlScheme)
                    {
                        GameObject uiPrefab;
                        if (controlSchemeParent.transform.Find(binding.key.id) == null)
                        {
                            uiPrefab = Object.Instantiate(rebindUIPrefab, controlSchemeParent.transform, true);
                        }
                        else uiPrefab = controlSchemeParent.transform.Find(binding.key.id).gameObject;
                        uiPrefab.transform.SetSiblingIndex(loadedData.bindingsList.Items.IndexOf(binding));
                        
                        uiPrefab.name = binding.key.id;
                        uiPrefab.transform.localScale = new Vector3(1, 1, 1);

                        var rebindUI = uiPrefab.GetComponent<RebindActionUI>();
                        
                        InputAction action = loadedData.inputActionAsset.FindAction(binding.key.actionId);
                        InputActionReference actionReference = inputActionReferences
                            .Find(reference => reference.action.id == action.id);
                        actionReference.Set(action);
                        rebindUI.actionReference = actionReference;
                        rebindUI.bindingId = binding.key.id;
                        rebindUI.rebindOverlay = rebindOverlay;
                        rebindUI.rebindPrompt = rebindPrompt;
                        if (!binding.value || loadedData.controlSchemesList.Items
                                .Find(scheme => scheme.key == controlScheme).value == false)
                        {
                            var button = uiPrefab.transform.Find("TriggerRebindButton");
                            button.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                            button.GetComponent<Button>().enabled = false;

                            var resetButton = uiPrefab.transform.Find("ResetToDefaultButton");
                            resetButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                            resetButton.GetComponent<Button>().enabled = false;
                        }
                        else
                        {
                            var button = uiPrefab.transform.Find("TriggerRebindButton");
                            button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            button.GetComponent<Button>().enabled = true;

                            var resetButton = uiPrefab.transform.Find("ResetToDefaultButton");
                            resetButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            resetButton.GetComponent<Button>().enabled = true;
                        
                        }
                    }
                }
            }
        }
        private static GameObject CreateAudioManager(GameObject audioManager, string jsonFile, bool newPrefab)
        {
            if (!newPrefab)
            {
                audioManager = GameObject.Instantiate(audioManager);
            }
            
            RectTransform audioManagerManagerRectTransform = audioManager.GetComponent<RectTransform>() == null ? audioManager.AddComponent<RectTransform>() : audioManager.GetComponent<RectTransform>();
            audioManagerManagerRectTransform.anchorMin = new Vector2(0, 0);
            audioManagerManagerRectTransform.anchorMax = new Vector2(1, 1);
            audioManagerManagerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            audioManagerManagerRectTransform.anchoredPosition = Vector3.zero;
            audioManagerManagerRectTransform.sizeDelta = new Vector2(0, 0);
            
            GameObject audioSettings = GetChildWithTag(audioManager, "ACC_Prefab");
            if (audioSettings == null) audioSettings = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioSettings.prefab"), audioManager.transform, true);
            
            var loadedData = ACC_JSONHelper.LoadJson<ACC_AudioManagerData>("ACC_AudioManager/" + jsonFile);
            if(loadedData == null) throw new Exception("AudioManagerData couldn't be loaded");
            //Debug.Log(loadedData.audioClips.Items[0].value.Items[1].value);;
            
            var audioManagerComponent = audioManager.GetComponent<ACC_AudioManager>() ?? audioManager.AddComponent<ACC_AudioManager>();
            audioManagerComponent.audioSources = new ACC_SerializableDictiornary<int, ACC_AudioSourceData>();
            audioManagerComponent.audioSources = loadedData.audioSources;
            audioManagerComponent.audioClips = new ACC_SerializableDictiornary<int, ACC_SerializableDictiornary<int, AudioClip>>();
            
            GameObject audioSources = GetChildWithTag(audioManager, "ACC_AudioSources");
            if (audioSources == null)
            {
                audioSources = new GameObject() { name = "ACC_AudioSources", tag = "ACC_AudioSources" };
                audioSources.transform.SetParent(audioManager.transform);
            }

            for (int i = 0; i < loadedData.audioSources.Items.Count; i++)
            {
                CreateAudioSource(audioSources, audioSettings, loadedData.audioSources.Items[i].value, i);
            }
            
            DeleteAudioSources(audioSources, audioSettings, loadedData.audioSources);

            for (int i = 0; i < loadedData.audioClips.Items.Count; i++)
            {
                // if (loadedData.audioSources.Items[i].value.is3D) { continue; }
                var audioClips = new ACC_SerializableDictiornary<int, AudioClip>();
                for (int j = 0; j < loadedData.audioClips.Items[i].value.Items.Count; j++)
                {
                    var audioClipPath = AssetDatabase.GUIDToAssetPath(loadedData.audioClips.Items[i].value.Items[j].value);
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
                    audioClips.AddOrUpdate(j, audioClip);
                }
                audioManagerComponent.audioClips.AddOrUpdate(i, audioClips);
            }
            
            return audioManager;
        }
        private static void CreateAudioSource(GameObject audioSources, GameObject audioSettings, ACC_AudioSourceData audioSourceData, int index)
        {
            GameObject audioSource;
            if (audioSources.transform.Find(audioSourceData.name) == null)
            {
                if (audioSourceData.is3D) return;
                audioSource = new GameObject {tag = "ACC_AudioSource", name = audioSourceData.name};
                audioSource.transform.SetParent(audioSources.transform);
            } 
            else if (audioSourceData.is3D)
            {
                audioSource = audioSources.transform.Find(audioSourceData.name).gameObject;
                Object.DestroyImmediate(audioSource);
                var scroll = GetChildWithTag(audioSettings, "ACC_Scroll");
                var scrollList = GetChildWithTag(scroll, "ACC_ScrollList");
                var slider = scrollList.transform.Find(audioSourceData.name).gameObject;
                Object.DestroyImmediate(slider);
                return;
            }
            else
            {
                audioSource = audioSources.transform.Find(audioSourceData.name).gameObject;
            }
            audioSource.transform.SetSiblingIndex(index);
            
            var audioSourceComponent = audioSource.GetComponent<AudioSource>() == null ? audioSource.AddComponent<AudioSource>() : audioSource.GetComponent<AudioSource>();
            audioSourceComponent.volume = audioSourceData.volume;
            
            var audioSourceScroll = GetChildWithTag(audioSettings, "ACC_Scroll");
            var audioSourcesList = GetChildWithTag(audioSourceScroll, "ACC_ScrollList");
            
            GameObject audioSourceSlider;
            if (audioSourcesList.transform.Find(audioSourceData.name) == null)
            {
                audioSourceSlider = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioSourceVolumeSlider.prefab"), audioSourcesList.transform, true);
                audioSourceSlider.name = audioSourceData.name;
            }
            else audioSourceSlider = audioSourcesList.transform.Find(audioSourceData.name).gameObject;
            audioSourceSlider.transform.SetSiblingIndex(index);
            
            var text = audioSourceSlider.transform.Find("ACC_AudioSourceName").GetComponent<TextMeshProUGUI>() ?? audioSourceSlider.transform.Find("ACC_AudioSourceName").gameObject.AddComponent<TextMeshProUGUI>();
            text.text = audioSourceData.name;
            
            var audioSourceSliderComponent = audioSourceSlider.transform.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>() ?? audioSourceSlider.transform.Find("ACC_AudioSourceVolumeSlider").gameObject.AddComponent<Slider>();
            audioSourceSliderComponent.value = audioSourceData.volume;
            
            audioSourceSliderComponent.onValueChanged.AddListener((value) =>
            {
                audioSourceComponent.volume = value;
            });
        }
        private static void DeleteAudioSources(GameObject audioSourcesGameObject, GameObject audioSettings,
            ACC_SerializableDictiornary<int,ACC_AudioSourceData> audioSources)
        {
            foreach (Transform child in audioSourcesGameObject.transform)
            {
                if (audioSources.Items.All(item => item.value.name != child.name))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            var audioSourceScroll = GetChildWithTag(audioSettings, "ACC_Scroll");
            var audioSourcesList = GetChildWithTag(audioSourceScroll, "ACC_ScrollList");
            
            foreach (Transform child in audioSourcesList.transform)
            {
                if (audioSources.Items.All(item => item.value.name != child.name))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        public static void Create3DAudioSource(ACC_AudioSourceData audioSourceData)
        {
            var objectGUID = audioSourceData.sourceObjectGUID;
            var sourceObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(objectGUID));

            GameObject parentObject;
            AudioSource audioSourceComponent;
            var gameObjectName = sourceObject != null ? sourceObject.name : "EmptyObject";
            
            if (sourceObject != null)
            {
                parentObject =  Object.Instantiate(sourceObject);
                parentObject.tag = "ACC_3DAudioSource";
                
                GameObject audioSource = new GameObject {tag = "ACC_AudioSource", name = "ACC_AudioSource_" + gameObjectName + "_3D"};
                audioSource.transform.SetParent(parentObject.transform);
                audioSource.transform.localPosition = Vector3.zero;
            
                audioSourceComponent = audioSource.AddComponent<AudioSource>();
            }
            else
            {
                audioSourceData.sourceObjectGUID = "-1";
                parentObject = new GameObject {tag = "ACC_3DAudioSource", name = "ACC_AudioSource_" + gameObjectName + "_3D"};

                audioSourceComponent = parentObject.AddComponent<AudioSource>();
            }
            
            audioSourceComponent.volume = audioSourceData.volume;
            audioSourceComponent.spatialBlend = 1;
            
            var folder = "ACC_Audio/ACC_3DAudioSources/";
            var name = "ACC_3DAS-" + audioSourceData.name + "_GO-" + gameObjectName + ".prefab"; 
            
            Directory.CreateDirectory("Assets/Resources/ACC_Prefabs/" + folder);
            var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
            
            PrefabUtility.SaveAsPrefabAsset(parentObject, prefabPath);
            Object.DestroyImmediate(parentObject);
            AssetDatabase.Refresh();
            
            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
            if (audioSourceData.prefabGUID != guid) AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(audioSourceData.prefabGUID));
            
            audioSourceData.prefabGUID = guid;
        }
        public static void Delete3DAudioSource(ACC_AudioSourceData audioSourceData)
        {
            if (string.IsNullOrEmpty(audioSourceData.prefabGUID)) return;
            var prefabPath = AssetDatabase.GUIDToAssetPath(audioSourceData.prefabGUID);
            AssetDatabase.DeleteAsset(prefabPath);
            AssetDatabase.Refresh();
            audioSourceData.prefabGUID = "";
            
        }
        private static GameObject GetChildWithTag(GameObject parent, string tag)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }
            return null;
        }
        private static GameObject GetChildWithNameAndTag(GameObject parent, string name, string tag)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag(tag) && child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }
        #endif
    }
}
