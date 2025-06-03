using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI{
    
    public class BarFiller : MonoBehaviour
    {
        [SerializeField]
        Image bar;
        [SerializeField]
        float max;
        public float progress
        {
            set
            {
                bar.fillAmount = Mathf.Clamp01(value*max);
            }
        }
    }
    
}
