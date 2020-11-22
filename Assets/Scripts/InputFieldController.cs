/*
 * Copyright 2020 Scott Hwang. All Rights Reserved.
 * Licensed under the Apache License, Version 2.0.
 */

/* Attach this to an InputField. This modifies the behavior such that
 * hitting the return key will send the text to the InputField in a target
 * GameObject. For example, I use this to trigger my TextToSpeech prefab
 * to convert text to audio speech.
 */
  
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{

    // The output target GameObject to receive text from this gameObject.
    [SerializeField]
    private GameObject targetGameObject;

    private InputField textInput; // The text input field the user types text into

    void Start()
    {
        // Set textInput to the InputField of this gameObject.
        textInput = gameObject.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if this gameObject is selected and if the Return key is pressed
        if (EventSystem.current.currentSelectedGameObject == gameObject && Input.GetKeyDown(KeyCode.Return))
        {
            // Check if the target GameObject has an InputField then place text into it.
            if (targetGameObject != null)
            {
                InputField target = targetGameObject.GetComponent<InputField>();
                if (target != null)
                {
                    target.text = textInput.text;
                }
            }

            // Clear the text field for the next input
            textInput.text = string.Empty;

            // Keep the current gameObject selected
            textInput.ActivateInputField();
            textInput.Select();
        }
    }
}
