using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Camera;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

namespace Project.Utilities{
    
    public class SensitivitySetter : MonoSingleton<SensitivitySetter>
    {
        CameraController camera;

        private void Start()
        {
            camera = FindObjectOfType<CameraController>();
            LoadConfig();
        }

        public void OnSettingRefresh(InputValue input)
        {
            Debug.Log("refreshPress");
            if (input.isPressed)
            {
                LoadConfig();
            }
        }

        private void LoadConfig()
        {
            Debug.Log("Refreshing");
            string path = $"{Application.persistentDataPath}/settings/";
            if(!File.Exists(path + "sensitivity.json"))
            {
                Directory.CreateDirectory(path);
                string data = JsonConvert.SerializeObject(new Dictionary<string, float> { { "xSensitivity", camera.xSensitivity }, { "ySensitivity", camera.ySensitivity } });
                File.WriteAllText(path+ "sensitivity.json", data);
            }
            else
            {
                string data = File.ReadAllText(path + "sensitivity.json");
                Dictionary<string, float> sensitivities = JsonConvert.DeserializeObject<Dictionary<string, float>>(data);
                if (sensitivities.ContainsKey("xSensitivity")) camera.xSensitivity = sensitivities["xSensitivity"];
                if (sensitivities.ContainsKey("ySensitivity")) camera.ySensitivity = sensitivities["ySensitivity"];
            }
        }
    }
    
}
