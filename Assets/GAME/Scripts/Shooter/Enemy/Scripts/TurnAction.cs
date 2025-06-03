using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;
using Project.Utilities;

namespace Project.Shooter.Enemy.Scripts
{

    [CreateAssetMenu(menuName = "FSM/Actions/Turn")]
    public class TurnAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            EnemyStateMachine enemyStateMachine = stateMachine.GetComponent<EnemyStateMachine>();
            
            Quaternion target = Quaternion.LookRotation(enemyStateMachine.currentTarget.transform.position.Flat() - stateMachine.transform.position.Flat());
            stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, target, Time.deltaTime*1.5f);
        }
    }

}
