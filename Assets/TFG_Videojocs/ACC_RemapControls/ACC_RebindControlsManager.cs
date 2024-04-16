using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TFG_Videojocs.ACC_RemapControls
{
    public class ACC_RebindControlsManager:MonoBehaviour
    {
        private List<GameObject> deviceManagerList = new List<GameObject>();
        private ACC_ControlSchemeData loadedData;
        
        public void LoadDeviceManager(string jsonFile)
        {
            string json = File.ReadAllText("Assets/TFG_Videojocs/ACC_JSON/ACC_ControlsSchemesConfiguration/" + jsonFile + ".json");
            loadedData = JsonUtility.FromJson<ACC_ControlSchemeData>(json);
            
            var devices = loadedData.inputActionAsset.controlSchemes
                .Select(scheme => 
                {
                    if (!scheme.deviceRequirements.Any())
                    {
                        return "All devices";
                    }

                    return scheme.deviceRequirements
                        .Select(requirement => requirement.controlPath.Replace("<", "").Replace(">", ""))
                        .Distinct()
                        .OrderBy(device => device)
                        .Aggregate((current, next) => current + ", " + next);
                })
                .Where(device => device != null)
                .Distinct()
                .ToList();
        }
    }
}