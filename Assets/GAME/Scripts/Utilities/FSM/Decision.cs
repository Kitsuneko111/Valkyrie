using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities.FSM{
    
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(BaseStateMachine stateMachine);
    }
    
}
