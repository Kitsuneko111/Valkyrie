using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;
using Project.Utilities;
using System.Linq;

namespace Project.Shooter.Enemy.Scripts
{

    [CreateAssetMenu(menuName = "FSM/Decisions/Forced")]
    public class ForceTransition : Decision
    {
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return true;
        }
    }

}
