using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;

namespace Project.Shooter.Enemy.Scripts{

    [CreateAssetMenu(menuName = "FSM/Actions/Patrol")]
    public class PatrolAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();
            enemyStateMachine.transform.position = Vector3.Lerp(stateMachine.transform.position, enemyStateMachine.currentTarget.transform.position, enemyStateMachine.speed*Time.deltaTime);
        }
    }
    
}
