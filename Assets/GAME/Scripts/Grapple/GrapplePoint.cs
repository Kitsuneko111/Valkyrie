using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Grapple{
    
    public class GrapplePoint : MonoBehaviour
    {
        public float currentDistance;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}
