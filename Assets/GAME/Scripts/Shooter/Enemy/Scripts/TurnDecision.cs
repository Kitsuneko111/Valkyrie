using Project.Utilities;
using Project.Utilities.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Shooter.Enemy.Scripts{

    [CreateAssetMenu(menuName = "FSM/Decisions/turnDecision")]
    public class TurnDecision : Decision
    {
        public override bool Decide(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();
            Vector3 target = Vector3.Normalize(enemyStateMachine.currentTarget.transform.position.Flat() - enemyStateMachine.transform.position.Flat());
            //Debug.Log(Vector3.Angle(target, enemyStateMachine.transform.forward));
            if (Vector3.Angle(target, enemyStateMachine.transform.forward) < 2f)
            {
                return true;
            }
            return false;
        }
    }
    
}
