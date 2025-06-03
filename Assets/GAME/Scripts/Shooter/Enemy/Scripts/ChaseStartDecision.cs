using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;
using Project.Utilities;
using System.Linq;

namespace Project.Shooter.Enemy.Scripts{

    [CreateAssetMenu(menuName = "FSM/Decisions/ChaseStartCheck")]
    public class ChaseStartDecision : Decision
    {
        public override bool Decide(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();
            List<RaycastHit> hits = new Physics().ConeCastAll(stateMachine.transform.position+7.5f*stateMachine.transform.forward, 20f, stateMachine.transform.forward, 20f, 45f, enemyStateMachine.layerMask);
            if (hits.Count > 0 )
            {
                enemyStateMachine.currentTarget = hits[0].collider.gameObject;
                return true;
            }
            //Debug.Log("CHASE CHANGING");
            //GameObject newPoint = enemyStateMachine.patrolPoints.OrderBy(x => Vector3.Distance(x.transform.position, enemyStateMachine.transform.position)).First();
            //enemyStateMachine.point = enemyStateMachine.patrolPoints.IndexOf(newPoint);
            //enemyStateMachine.currentTarget = enemyStateMachine.patrolPoints[enemyStateMachine.point];
            return false;
        }
    }
    
}
