using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities{
    
    public class Limiter : MonoSingleton<Limiter>
    {
        [SerializeField]
        public int targetFrameRate = 30;
        // Update is called once per frame
        void Update()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
    }
    
}
