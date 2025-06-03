using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities.FSM{
    
    public abstract class BaseState : ScriptableObject
    {
        public virtual void Execute(BaseStateMachine stateMachine) { }
    }
    
}
