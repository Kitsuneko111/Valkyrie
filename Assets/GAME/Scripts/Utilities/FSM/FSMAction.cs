using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities.FSM{
    
    public abstract class FSMAction : ScriptableObject
    {
        public abstract void Execute(BaseStateMachine stateMachine);
    }
    
}
