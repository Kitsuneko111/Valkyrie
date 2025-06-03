using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;
using Project.Utilities;
using System.Linq;

namespace Project.Shooter.Enemy.Scripts
{

    [CreateAssetMenu(menuName = "FSM/Actions/ChaseEndAction")]
    public class ChaseEndAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();
            Debug.Log("CHASE CHANGING");
            GameObject newPoint = enemyStateMachine.patrolPoints.OrderBy(x => Vector3.Distance(x.transform.position, enemyStateMachine.transform.position)).First();
            enemyStateMachine.point = enemyStateMachine.patrolPoints.IndexOf(newPoint);
            enemyStateMachine.currentTarget = enemyStateMachine.patrolPoints[enemyStateMachine.point];
        }
    }

}
