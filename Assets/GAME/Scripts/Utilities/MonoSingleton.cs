using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

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
                    _instance = (T)this;
                }
                else if (_instance != null && _instance != GetComponent<T>())
                {
                    Destroy(this);
                }
            }
        }
    }
}