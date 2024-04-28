using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Unity.Burst.Intrinsics.X86;
using System.Linq;

namespace Project.UI{
    
    public class FPS : MonoBehaviour
    {
        TMP_Text text;
        Queue<(double, float)> historic = new Queue<(double, float)>();

        private void Start()
        {
            text = GetComponent<TMP_Text>();
        }
        void Update()
        {
            double now = Time.realtimeSinceStartup;
            historic.Enqueue((now, Time.deltaTime));
            while (historic.Peek().Item1 < now-10) historic.Dequeue();
            float averageFPS = historic.Average(x => x.Item2);
            text.text = $"FPS: {(int)(1/averageFPS)} (avg/10s)";
        }
    }
    
}
