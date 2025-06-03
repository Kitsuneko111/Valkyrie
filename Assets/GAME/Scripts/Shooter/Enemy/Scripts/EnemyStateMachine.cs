using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utilities.FSM;
using Project.Character;
using Project.Shooter.Targets;

namespace Project.Shooter.Enemy.Scripts{
    
    public class EnemyStateMachine : BaseStateMachine
    {
        public GameObject currentTarget;
        public float speed = 1;
        public List<GameObject> patrolPoints;
        public int point;
        public LayerMask layerMask;

        private void Start()
        {
            _cachedComponents.Add(typeof(Character.CharacterController), FindObjectOfType<Character.CharacterController>());
            GetComponent<Target>();
        }
    }
    
}
