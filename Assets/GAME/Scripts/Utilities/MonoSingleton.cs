using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /*protected MonoSingleton()
        {

        }*/

        private static readonly object padlock = new object();

        private static T _instance;
        public static T Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).FullName);
                        _instance = go.AddComponent<T>();
                    }
                    return _instance;
                }
            }
        }

        private void Awake()
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    _instance = GetComponent<T>();
                }
                else if (_instance != null && _instance != GetComponent<T>())
                {
                    Destroy(this);
                }
            }
        }
    }
}