using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Project.Utilities;

namespace Project.Shooter.Targets{
    
    public class Enemy : Target
    {
        List<SubEnemy> subParts;
        [SerializeField]
        float maxHealth = 100f;
        public float health;
        Timer deathTimer = new Timer("deathTimer", 2f, false);
        [SerializeField]
        float damageReception = 1f;
        [SerializeField]
        UnityEvent deathEvents;
        public bool required = true;

        protected override void Start()
        {
            base.Start();
            subParts = new(GetComponentsInChildren<SubEnemy>());
            foreach(SubEnemy subEnemy in subParts)
            {
                subEnemy.master = this;
            }
            
            health = maxHealth;
            if(required) Project.Utilities.EnemyManager.Instance.count += 1;
        }

        public void Damage(float damage)
        {
            health -= damage;
            if (health <= 0) Death();
        }

        public void Death()
        {
            //EnemyManager.Instance.StartCoroutine(deathTimer.Start());
            gameObject.SetActive(false);
            deathEvents.Invoke();
            if(required) Project.Utilities.EnemyManager.Instance.count -= 1;
        }

        public override void Hit(float damage)
        {
            base.Hit(damage);
            Debug.Log(damage);
            Damage(damage*damageReception);
        }
    }
    
}
