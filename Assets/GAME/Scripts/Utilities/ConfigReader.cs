using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Camera;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

namespace Project.Utilities{
    
    public class ConfigReader : MonoSingleton<ConfigReader>
    {
        new CameraController camera;

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
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Debug.Log("Refreshing");
                string path = $"{Application.persistentDataPath}/settings/";
                if (!File.Exists(path + "sensitivity.json"))
                {
                    Directory.CreateDirectory(path);
                    string data = JsonConvert.SerializeObject(new Dictionary<string, float> { { "xSensitivity", camera.xSensitivity }, { "ySensitivity", camera.ySensitivity }, { "framerate", 60f } });
                    File.WriteAllText(path + "sensitivity.json", data);
                }
                else
                {
                    string data = File.ReadAllText(path + "sensitivity.json");
                    Dictionary<string, float> config = JsonConvert.DeserializeObject<Dictionary<string, float>>(data);
                    if (config.ContainsKey("xSensitivity")) camera.xSensitivity = config["xSensitivity"];
                    if (config.ContainsKey("ySensitivity")) camera.ySensitivity = config["ySensitivity"];
                    if (config.ContainsKey("framerate")) Limiter.Instance.targetFrameRate = config["framerate"] > 0f ? (int)config["framerate"] : 99999;
                }
            }
        }
    }
    
}
