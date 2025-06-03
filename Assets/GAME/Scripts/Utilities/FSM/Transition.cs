using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities.FSM{

    [CreateAssetMenu(menuName = "FSM/Transition")]
    public sealed class Transition : ScriptableObject
    {
        public Decision Decision;
        public BaseState TrueState;
        public BaseState FalseState;

        public void Execute(BaseStateMachine stateMachine)
        {
            bool decision = Decision.Decide(stateMachine);
            //Debug.Log(decision);
            //Debug.Log(Decision.ToString());
            if (decision && TrueState is not RemainInState)
            {
                stateMachine.CurrentState = TrueState;
            } else if(FalseState is not RemainInState)
            {
                stateMachine.CurrentState = FalseState;
            }
        }
    }
    
}
