using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFG_Videojocs.ACC_HighContrast;
using TFG_Videojocs.ACC_RemapControls;
using TFG_Videojocs.ACC_Sound;
using TFG_Videojocs.ACC_Sound.ACC_Example;
using TMPro;
using Unity.VisualScripting;
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
        public static void CreatePrefab(string feature, string jsonFile = "")
        {
            if(Application.isPlaying) return;
            var gameObject = CreateGameObject(feature, jsonFile);
            
            //gameObject.transform.SetParent(GameObject.Find("ACC_Canvas").transform, false);
            var folder = "ACC_" + feature + "/";
            var name = "ACC_" + feature + "Manager.prefab";
            
            Directory.CreateDirectory("Assets/Resources/ACC_Prefabs/" + folder);
            
            var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
            GameObject.DestroyImmediate(gameObject, true);
            AssetDatabase.Refresh();
        }
        public static GameObject InstantiatePrefabAsChild(string feature, GameObject parent, string jsonFile="")
        {
            var folder = "ACC_" + feature + "/";
            var name = "ACC_" + feature + "Manager";
            var prefabPath = "ACC_Prefabs/" + folder + name;
            
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.Log("Prefab couldn't be loaded in " + prefabPath);
                return null;
            }

            return GameObject.Instantiate(prefab, parent.transform);
        }
        private static GameObject CreateGameObject(string feature, string jsonFile = "")
        {
            string name = "ACC_" + feature + "Manager";
            bool newPrefab = false;
            GameObject gameObject = GetPrefabWithTag("ACC_" + feature + "Manager", "Assets/Resources/ACC_Prefabs/"+ "ACC_" + feature + "/");
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
                    CreateRemapControlsManager(gameObject, jsonFile);
                    break;
                case "Audio":
                    CreateAudioManager(gameObject, jsonFile);
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
            subtitleTextMeshProUGUI.color = new Color(1f, 0f, 0f, 1);

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
            backgroundColorImage.color = new Color(1, 1, 1, 0);
            
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
            visualNotificationTextMeshProUGUI.color = new Color(1f, 0f, 0f, 0);
            
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
        private static void CreateRemapControlsManager(GameObject remapControlsManager, string jsonFile)
        {
            RectTransform remapControlsManagerRectTransform = remapControlsManager.AddComponent<RectTransform>();
            remapControlsManagerRectTransform.anchorMin = new Vector2(0, 0);
            remapControlsManagerRectTransform.anchorMax = new Vector2(1, 1);
            remapControlsManagerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            remapControlsManagerRectTransform.anchoredPosition = Vector3.zero;
            remapControlsManagerRectTransform.sizeDelta = new Vector2(0, 0);
    
            var remapControlsManagerComponent = remapControlsManager.AddComponent<ACC_RemapControlsManager>();
            remapControlsManagerComponent.defaultMenu = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/ACC_DeviceManager.prefab");
            remapControlsManagerComponent.rebindUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/Rebinding UI/RebindUIPrefab.prefab");
            remapControlsManagerComponent.rebindOverlay = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_RemapControls/WaitForInputPanel.prefab");
            
            var waitForInputPanel = GameObject.Instantiate(remapControlsManagerComponent.rebindOverlay, remapControlsManager.transform, true);
            waitForInputPanel.transform.localScale = new Vector3(1, 1, 1);
            waitForInputPanel.transform.localPosition = new Vector3(0, 0, 0);
            
            try
            {
                var loadedData = ACC_JSONHelper.LoadJson<ACC_ControlSchemeData>("ACC_ControlSchemesConfiguration/" + jsonFile);
                remapControlsManagerComponent.loadedData = loadedData;
                SetControlSchemes(loadedData, remapControlsManager);
            }
            #pragma warning disable 0168
            catch (Exception e)
            {
            }
            #pragma warning restore 0168
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
                var deviceManager = GameObject.Instantiate(defaultMenu, remapControlsManager.transform, true);
                deviceManager.name = device;
                
                RectTransform rectTransform = deviceManager.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1, 1, 1);
                rectTransform.localPosition = new Vector3(0, 0, 0);
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                deviceManager.transform.Find("Title").GetComponent<TextMeshProUGUI>().text =
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
                var rebindingsScroll = deviceManager.transform.Find("RebindsScroll");

                var controlSchemeParent = new GameObject
                {
                    name = controlScheme,
                    tag = "ACC_ScrollList"
                };
                var rectTransform = controlSchemeParent.AddComponent<RectTransform>();

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

                var verticalLayoutGroup = controlSchemeParent.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.spacing = 0;

                var contentSizeFitter = controlSchemeParent.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                foreach (var binding in loadedData.bindingsList.Items)
                {
                    if (controlScheme == binding.key.controlScheme)
                    {
                        var uiPrefab = Object.Instantiate(rebindUIPrefab, controlSchemeParent.transform, true);
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
                    }
                }
            }
        }
        private static void CreateAudioManager(GameObject audioManager, string jsonFile)
        {
            RectTransform audioManagerManagerRectTransform = audioManager.AddComponent<RectTransform>();
            audioManagerManagerRectTransform.anchorMin = new Vector2(0, 0);
            audioManagerManagerRectTransform.anchorMax = new Vector2(1, 1);
            audioManagerManagerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            audioManagerManagerRectTransform.anchoredPosition = Vector3.zero;
            audioManagerManagerRectTransform.sizeDelta = new Vector2(0, 0);
            
            GameObject audioSettings = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioSettings.prefab"), audioManager.transform, true);
            
            var loadedData = ACC_JSONHelper.LoadJson<ACC_AudioManagerData>("ACC_AudioManager/" + jsonFile);
            if(loadedData == null) return;
            
            var audioManagerComponent = audioManager.AddComponent<ACC_AudioManager>();
            audioManagerComponent.audioSources = loadedData.audioSources;
            
            GameObject audioSources = new GameObject("ACC_AudioSources");
            audioSources.transform.SetParent(audioManager.transform);
            
            for (int i = 0; i < loadedData.audioSources.Items.Count; i++)
            {
                if (loadedData.audioSources.Items[i].value.is3D) { continue; }
                CreateAudioSource(audioSources, audioSettings, loadedData.audioSources.Items[i].value);
            }

            for (int i = 0; i < loadedData.audioClips.Items.Count; i++)
            {
                if (loadedData.audioSources.Items[i].value.is3D) { continue; }
                var audioClips = new ACC_SerializableDictiornary<int, AudioClip>();
                for (int j = 0; j < loadedData.audioClips.Items[i].value.Items.Count; j++)
                {
                    var audioClipPath = AssetDatabase.GUIDToAssetPath(loadedData.audioClips.Items[i].value.Items[j].value);
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
                    audioClips.AddOrUpdate(j, audioClip);
                }
                audioManagerComponent.audioClips.AddOrUpdate(i, audioClips);
            }
        }
        private static void CreateAudioSource(GameObject audioSources, GameObject audioSettings, ACC_AudioSourceData audioSourceData)
        {
            GameObject audioSource = new GameObject {tag = "ACC_AudioSource", name = audioSourceData.name};
            audioSource.transform.SetParent(audioSources.transform);
                
            var audioSourceComponent = audioSource.AddComponent<AudioSource>();
            audioSourceComponent.volume = audioSourceData.volume;
            
            var audioSourcesList = audioSettings.transform.Find("AudioSourcesScroll").transform.Find("AudioSourcesList");
            var audioSourceSlider = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TFG_Videojocs/ACC_Sound/ACC_AudioSourceVolumeSlider.prefab"), audioSourcesList.transform, true);
            
            audioSourceSlider.name = audioSourceData.name;
            audioSourceSlider.transform.Find("ACC_AudioSourceName").GetComponent<TextMeshProUGUI>().text = audioSourceData.name;
            
            var audioSourceSliderComponent = audioSourceSlider.transform.Find("ACC_AudioSourceVolumeSlider").GetComponent<Slider>();
            audioSourceSliderComponent.value = audioSourceData.volume;
            
            audioSourceSliderComponent.onValueChanged.AddListener((value) =>
            {
                audioSourceComponent.volume = value;
            });
        }
        public static void Create3DAudioSource(ACC_AudioSourceData audioSourceData)
        {
            var objectGUID = audioSourceData.sourceObjectGUID;
            var sourceObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(objectGUID));
            if (audioSourceData.prefabGUID != "") AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(audioSourceData.prefabGUID));

            GameObject parentObject;
            AudioSource audioSourceComponent;
            var gameObjectName = sourceObject != null ? sourceObject.name : "EmptyObject";
            
            if (sourceObject != null)
            {
                parentObject =  GameObject.Instantiate(sourceObject);
                
                GameObject audioSource = new GameObject {tag = "ACC_AudioSource", name = "ACC_AudioSource_" + gameObjectName + "_3D"};
                audioSource.transform.SetParent(parentObject.transform);
                audioSource.transform.localPosition = Vector3.zero;
            
                audioSourceComponent = audioSource.AddComponent<AudioSource>();
            }
            else
            {
                audioSourceData.sourceObjectGUID = "-1";
                parentObject = new GameObject {tag = "ACC_AudioSource", name = "ACC_AudioSource_" + gameObjectName + "_3D"};

                audioSourceComponent = parentObject.AddComponent<AudioSource>();
            }
            
            audioSourceComponent.volume = audioSourceData.volume;
            audioSourceComponent.spatialBlend = 1;
            
            var folder = "ACC_Audio/ACC_3DAudioSources/";
            var name = "ACC_3DAS-" + audioSourceData.name + "_GO-" + gameObjectName + ".prefab"; 
            
            Directory.CreateDirectory("Assets/Resources/ACC_Prefabs/" + folder);
            var prefabPath = "Assets/Resources/ACC_Prefabs/" + folder + name;
            
            PrefabUtility.SaveAsPrefabAsset(parentObject, prefabPath);
            GameObject.DestroyImmediate(parentObject);
            AssetDatabase.Refresh();
            
            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
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
    }
}