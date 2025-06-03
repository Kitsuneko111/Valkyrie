using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;

namespace Project.Shooter.Enemy.Scripts{
    [CreateAssetMenu(menuName="FSM/Decisions/PatrolDecision")]
    public class PatrolDecision : Decision
    {
        // Start is called before the first frame update
        public override bool Decide(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();

            enemyStateMachine.currentTarget = enemyStateMachine.patrolPoints[enemyStateMachine.point];
            if (Vector3.Distance(stateMachine.transform.position, enemyStateMachine.currentTarget.transform.position) < 1f)
            {
                Debug.Log("TRUE");
                enemyStateMachine.point += 1;
                Debug.Log(enemyStateMachine.point);
                enemyStateMachine.point = enemyStateMachine.point % enemyStateMachine.patrolPoints.Count;
                enemyStateMachine.currentTarget = enemyStateMachine.patrolPoints[enemyStateMachine.point];
                return true;
            }
            return false;
        }
    }
    
}
