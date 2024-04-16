using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TFG_Videojocs.ACC_RemapControls
{
    public class ACC_RebindControlsManager:MonoBehaviour
    {
        private List<GameObject> deviceManagerList = new List<GameObject>();
        private ACC_ControlSchemeData loadedData;
        [SerializeField] private GameObject defaultMenu;
        [SerializeField] private GameObject rebindUIPrefab;
        
        public void LoadRebindControlsManager(string jsonFile)
        {
            string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_ControlSchemesConfiguration/" + jsonFile + ".json");
            loadedData = JsonUtility.FromJson<ACC_ControlSchemeData>(json);
            
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
            
            foreach (var device in devices)
            {
                var deviceManager = Instantiate(defaultMenu, transform, true);
                deviceManager.name = device;
                deviceManager.transform.localScale = new Vector3(1, 1, 1);
                deviceManager.transform.localPosition = new Vector3(0, 0, 0);

                deviceManager.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    loadedData.controlSchemesList.Items[0].key;
            }
            
        }
    }
}