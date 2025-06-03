using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Project.Utilities{
    
    public class SettingsManager : MonoSingleton<SettingsManager>
    {
        private void Start()
        {
            DontDestroyOnLoad(Instance);
        }

        public Settings settings = new Settings();

        void LoadSettings()
        {
            if(Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Debug.Log("Refreshing");
                string path = $"{Application.persistentDataPath}/settings/";
                if (!File.Exists(path + "sensitivity.json"))
                {
                    Directory.CreateDirectory(path);
                    string data = JsonConvert.SerializeObject(new Settings());
                    File.WriteAllText(path + "sensitivity.json", data);
                }
                else
                {
                    string data = File.ReadAllText(path + "sensitivity.json");
                    settings = JsonConvert.DeserializeObject<Settings>(data);
                    
                }
            }
        }
    }

    [Serializable]
    public struct Settings
    {
        public Vector2 sensitivity; // x/y
        public int releaseType; // 0 = default, 1 = jump to disengage, 2 = auto disengage, 3 = reversed default
        public bool toggleSprint; // t/f
        public int aimMode; // 0 = default, 1 = left fire right aim, 2 = default auto aim
        public bool swapFires; // swap grapple and guns
        
        public Settings(Vector2 sensitivity = new Vector2(), int releaseType = 0, bool toggleSprint = true, int aimMode = 0, bool swapFires = false)
        {
            this.sensitivity = sensitivity != Vector2.zero ? sensitivity : new Vector2(0.3f, 0.4f);
            this.releaseType = releaseType != 0 ? releaseType : 0;
            this.toggleSprint = toggleSprint;
            this.aimMode = aimMode != 0 ? aimMode : 0;
            this.swapFires = swapFires;
        }
    }
}
