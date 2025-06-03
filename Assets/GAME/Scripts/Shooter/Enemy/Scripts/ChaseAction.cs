using Project.Utilities;
using Project.Utilities.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Shooter.Enemy.Scripts{

    [CreateAssetMenu(menuName = "FSM/Actions/Chase")]
    public class ChaseAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();

            Quaternion target = Quaternion.LookRotation(enemyStateMachine.currentTarget.transform.position.Flat() - stateMachine.transform.position.Flat());
            stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, target, Time.deltaTime*1.5f);
            stateMachine.transform.position = Vector3.Lerp(stateMachine.transform.position, enemyStateMachine.currentTarget.transform.position, enemyStateMachine.speed * Time.deltaTime);
        }
    }
    
}
