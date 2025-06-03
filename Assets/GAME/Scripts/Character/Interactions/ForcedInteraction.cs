using Project.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Character.Interactions{

    public class ForcedInteraction : Interaction
    {
        [SerializeField]
        LayerMask layerMask;
        public void OnInteract() { }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") Interact();

        }

        private void Start()
        {
            layerMask = LayerMask.GetMask("Player");
            StartCoroutine(SlightDelay());
        }

        private IEnumerator SlightDelay()
        {
            yield return new WaitForSeconds(0.25f);
            if (Physics.CheckBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, layerMask))
            {
                Interact();
            }
        }
    }
}
