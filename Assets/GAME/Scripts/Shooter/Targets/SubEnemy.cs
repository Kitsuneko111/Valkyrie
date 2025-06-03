using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Shooter.Targets{
    
    public class SubEnemy : Target
    {
        public Enemy master;
        [SerializeField]
        float damageReception = 1f;

        public override void Hit(float d)
        {
            base.Hit(d);
            master.Damage(d*damageReception);
        }
    }
    
}
