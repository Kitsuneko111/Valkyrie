using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Grapple{
    // glorified empty game object with some logic for dev gizmos
    public class GrapplePoint : MonoBehaviour
    {
        public float currentDistance;
        new public Collider collider;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}
