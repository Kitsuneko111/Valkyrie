using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utilities{
    
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        private int _count;
        public int count
        {
            get { return _count; }
            set { _count = value;
                if (_count <= 0) AllDead();
            }
        }

        public void AllDead()
        {
            Project.Utilities.SceneManager.Instance.NextScene();
        }
    }
    
}
