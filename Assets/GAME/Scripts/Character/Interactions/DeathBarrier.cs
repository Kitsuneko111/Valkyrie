using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Character.Interactions
{

    public class DeathBarrier : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                other.GetComponentInParent<CharacterController>().Death();
            }
        }

    }

}
