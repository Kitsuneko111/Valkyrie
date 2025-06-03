using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Project.Utilities.UI{
    
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField]
        Canvas canvas;
        List<Canvas> menus;
        PlayerInput playerInput;
        EventSystem eventSystem;
        InputSystemUIInputModule inputSystemUIInputModule;
        [SerializeField]
        GameObject lastSelected;

        [SerializeField]
        LayerMask layerMask;

        private void Start()
        {
            canvas = GameObject.FindWithTag("PauseMenu").GetComponent<Canvas>();
            menus = new(FindObjectsOfType<Canvas>());
            menus.Remove(canvas);
            playerInput = FindObjectOfType<PlayerInput>();
            eventSystem = FindObjectOfType<EventSystem>();
            inputSystemUIInputModule = FindObjectOfType<InputSystemUIInputModule>();
            lastSelected = eventSystem.firstSelectedGameObject;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
            } else
            {
                inputSystemUIInputModule.enabled = false;
            }
        }

        public void OnMenuToggle(InputValue inputValue)
        {
            Debug.Log("A");
            ToggleMenu();
        }

        public void ToggleMenu()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
            {
                if (!canvas.enabled)
                {
                    inputSystemUIInputModule.enabled = true;
                    playerInput.SwitchCurrentActionMap("UI");
                    Cursor.lockState = CursorLockMode.Confined;
                    canvas.enabled = true;
                    foreach (Canvas menu in menus)
                    {
                        menu.enabled = false;
                    }
                    Time.timeScale = 0f;
                }
                else
                {
                    inputSystemUIInputModule.enabled = false;
                    playerInput.SwitchCurrentActionMap("Player");
                    Cursor.lockState = CursorLockMode.Locked;
                    canvas.enabled = false;
                    foreach (Canvas menu in menus)
                    {
                        menu.enabled = true;
                    }
                    Time.timeScale = 1f;
                }
            }
        }
    }
    
}
