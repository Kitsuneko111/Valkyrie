using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Character;
using TMPro;

namespace Project.UI{
    
    public class Speed : MonoBehaviour
    {
        Character.CharacterController character;
        TMP_Text text;
        // Start is called before the first frame update
        void Start()
        {
            character = FindObjectOfType<Character.CharacterController>();
            text = GetComponent<TMP_Text>();
        }
    
        // Update is called once per frame
        void Update()
        {
            text.text = $"Speed: {Mathf.Round(character.rb.velocity.magnitude*100)/100}m/s";
        }
    }
    
}
