using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities.FSM{

    [CreateAssetMenu(menuName = "FSM/State")]
    public sealed class State : BaseState
    {
        public List<FSMAction> Actions = new();
        public List<Transition> Transitions = new();

        public override void Execute(BaseStateMachine stateMachine)
        {
            foreach ( FSMAction action in Actions ) 
            { 
                action.Execute(stateMachine);
            }

            foreach ( Transition transition in Transitions )
            {
                transition.Execute(stateMachine);
            }
        }
    }
    
}
