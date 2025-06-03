using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Character;

// https://www.toptal.com/unity-unity3d/unity-ai-development-finite-state-machine-tutorial

namespace Project.Utilities.FSM{
    
    public class BaseStateMachine : MonoBehaviour
    {
        [SerializeField] protected BaseState _initialState;

        public BaseState CurrentState;

        protected Dictionary<Type, Component> _cachedComponents;
        
        protected void Awake()
        {
            CurrentState = _initialState;
            _cachedComponents = new Dictionary<Type, Component>();
        }
    
        // Update is called once per frame
        protected void Update()
        {
            CurrentState.Execute(this);
        }

        public new T GetComponent<T>() where T : Component
        {
            if (_cachedComponents.ContainsKey(typeof(T)))
            {
                return _cachedComponents[typeof(T)] as T;
            }

            var component = base.GetComponent<T>();
            if(component != null)
            {
                _cachedComponents.Add(typeof(T), component);
            }
            return component;
        }
    }
    
}
