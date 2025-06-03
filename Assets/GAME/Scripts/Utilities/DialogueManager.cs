using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

namespace Project.Utilities{
    
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        GameObject dialogue;
        TMP_Text dialogueText;
        [SerializeField]
        GameObject popup;
        [SerializeField]
        TMP_Text popupButtonText;
        TMP_Text popupPreText;
        [SerializeField]
        TMP_Text popupActionText;
        int maxLength = 100;
        Timer interScreenTimer = new Timer("interScreenTimer", 1.5f, false);
        Timer interCharacterTimer = new Timer("interCharacterTimer", 0.05f, false);
        Timer popupTimer = new Timer("popupTimer", 5f, false);

        [SerializeField]
        PlayerInput playerInput;

        bool runningDialogue = false;
        int currentLine = 0;
        int currentChar = 0;

        public delegate void Callback();

        private void Start()
        {
            dialogue = transform.GetChild(0).gameObject;
            popup = transform.GetChild(1).gameObject;
            dialogueText = dialogue.GetComponentInChildren<TMP_Text>();
            playerInput = FindObjectOfType<PlayerInput>();
            popupPreText = popup.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
            popupButtonText = popup.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
            popupActionText = popup.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
            popupTimer.callbacks += () => { popup.SetActive(false); };
        }

        public void Say(string text, Callback callback)
        {
            dialogue.SetActive(true);
            if (runningDialogue)
            {
                Cancel();
            }
            List<string> sentences = new(text.Split("  "));
            Debug.Log(sentences.Count);
            Debug.Log("");
            List<string> fullSentences = new() { sentences[0] };
            sentences.RemoveAt(0);
            int index = 0;
            foreach (string sentence in sentences)
            {
                Debug.Log(sentence.Length);
                if ((fullSentences[index]+" "+sentence).Length < maxLength)
                {
                    fullSentences[index] += " " + sentence;
                }
                else
                {
                    fullSentences.Add(sentence);
                    index++;
                }
            }
            Debug.Log(fullSentences[0]);
            StartCoroutine(SayDialogue(fullSentences, callback));
        }

        private IEnumerator SayDialogue(List<string> dialogueList, Callback callback)
        {
            Debug.Log(dialogueList.Count);
            runningDialogue = true;
            currentLine = 0;
            while(runningDialogue && currentLine < dialogueList.Count)
            {
                dialogueText.text = "";
                currentChar = 0;
                while(runningDialogue && currentLine < dialogueList.Count && currentChar < dialogueList[currentLine].Length)
                {
                    if (currentChar < 0) currentChar = 0;
                    dialogueText.text += dialogueList[currentLine][currentChar];
                    StartCoroutine(interCharacterTimer.Start());
                    while(interCharacterTimer.running)
                    {
                        yield return null;
                    }
                    currentChar++;
                }
                if (runningDialogue && dialogueText.text != "")
                {
                    StartCoroutine(interScreenTimer.Start());
                    while (interScreenTimer.running)
                    {
                        yield return null;
                    }
                }
                currentLine++;
            }
            dialogue.SetActive(false);
            callback();
        }

        public void Skip()
        {
            if (runningDialogue)
            {
                if (interScreenTimer.running) interScreenTimer.Cancel();
                else
                {
                    interCharacterTimer.Cancel();
                    currentLine++;
                    currentChar = -1;
                    dialogueText.text = "";
                }
            }
        }

        public void Cancel()
        {
            if (runningDialogue)
            {
                runningDialogue = false;
                interCharacterTimer.Cancel();
                interScreenTimer.Cancel();
            }
        }

        Dictionary<string, List<string>> controlMap = new()
        {
            { "Move", new(){"WASD", "Left Stick" } },
            { "Look", new(){"Mouse", "Right Stick" } },
            { "Jump", new(){"Space", "A" } },
            { "Sprint", new(){"Shift", "Left Stick"} },
            { "Skip", new(){"Tab", "Y"} },
            { "Reload", new(){"R", "X"} },
            { "Shoot", new(){"Left/Right Click", "Left/Right Trigger"}},
            { "Grapple", new(){"Q/E", "Left/Right Bumper"}}
        };

        Dictionary<string, int> schemeMap = new()
        {
            { "Keyboard&Mouse", 0 },
            { "Gamepad", 1 }
        };

        public void Popup(string popupText)
        {
            Debug.Log(playerInput.currentControlScheme);
            popup.SetActive(true);
            List<string> displayText = new(popupText.Split("|"));
            popupPreText.text = displayText[0];
            popupButtonText.text = controlMap[displayText[1]][schemeMap[playerInput.currentControlScheme]];
            popupActionText.text = displayText[2];
            StartCoroutine(popupTimer.Start());
        }
    }
    
}
