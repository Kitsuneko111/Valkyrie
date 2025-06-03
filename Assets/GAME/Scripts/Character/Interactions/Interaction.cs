using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Project.Utilities;

namespace Project.Character.Interactions{
    
    public class Interaction : MonoBehaviour
    {
        bool canInteract;
        [SerializeField]
        protected UnityEvent interactionFunctions;
        [SerializeField]
        protected UnityEvent postDialogueFunctions;

        [SerializeField]
        bool singleUse = false;
        bool interacted;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player") canInteract = true;
        }

        private void OnTriggerExit(Collider other)
        {
            canInteract = false;
        }

        public void OnInteract(InputValue input)
        {
            if (canInteract && input.isPressed)
            {
                Interact();
            }
        }

        protected void Interact()
        {
            if (!(singleUse && interacted))
            {
                Debug.Log("Interacting");
                interactionFunctions.Invoke();
                interacted = true;
            }
        }

        public void NextScene()
        {
            SceneManager.Instance.NextScene();
        }

        public void SayDialogue(string text)
        {
            if(!interacted) DialogueManager.Instance.Say(text, () => { postDialogueFunctions.Invoke(); });
        }
    }
    
}
