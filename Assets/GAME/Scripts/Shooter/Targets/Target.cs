using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities;

namespace Project.Shooter.Targets{
    
    public class Target : MonoBehaviour
    {
        
        Timer timer = new Timer("hitTimer", 0.5f, false);
        new Renderer renderer;

        protected virtual void Start()
        {
            renderer = GetComponent<Renderer>();
            timer.callbacks += () => { renderer.material.SetColor("_BaseColor", Color.black); };
        }

        public virtual void Hit(float damage)
        {
            renderer.material.SetColor("_BaseColor", new Color(255, 102, 0));
            Debug.Log("HIT");
            EnemyManager.Instance.StartCoroutine(timer.Start());
        }
    }
    
}
