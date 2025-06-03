using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Utilities.UI{
    
    public class CustomisedButton : Button
    {
        float offset = 20f;
        TMP_Text text;
        MenuHandler menuHandler;

        protected override void Start()
        {
            base.Start();
            text = GetComponentInChildren<TMP_Text>();
            menuHandler = FindObjectOfType<MenuHandler>();
        }
        public override void OnSelect(BaseEventData eventData)
        {
            transform.Translate(new Vector3(offset, 0f, 0f));
            base.OnSelect(eventData);
            text.color = colors.selectedColor;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            transform.Translate(new Vector3(-offset, 0f, 0f));
            base.OnDeselect(eventData);
            text.color = colors.normalColor;
        }

        private void Update()
        {
            if (IsHighlighted())
            {
                Select();   
            }
        }

        public void ToggleMenu()
        {
            menuHandler.ToggleMenu();
        }

        public void Quit()
        {
            SceneManager.Instance.Quit();
        }

        public void Restart()
        {
            SceneManager.Instance.Restart();
        }

        public void MainMenu()
        {
            SceneManager.Instance.Load(0);
        }

        public void NextScene()
        {
            SceneManager.Instance.NextScene();
        }

        public void LoadScene(int id)
        {
            SceneManager.Instance.Load(id);
        }
    }
}
