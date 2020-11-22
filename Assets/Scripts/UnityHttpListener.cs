/* Copyright 2020 Scott Hwang
 * https://www.linkedin.com/in/snhwang
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License./*
 *
 * Modified from code found at github.com:
 * https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10
 * 
 * Listen at port 3000 for text data to be placed in an InputField.
 * Attach this script to an InputField.
*/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Net;
using System.Threading;

public class UnityHttpListener : MonoBehaviour
{

    private HttpListener listener;
    private Thread listenerThread;

/* Whether or not to set the text to string.Empty immediately after updating the text with
 * the received data. The idea is to use the received text to trigger an event and then
 * have it reset to be empty for the next input text.    
*/
    [SerializeField]
    private bool resetTextEachTime = true;

    // The InputFIeld for placing the received text
    private InputField inputField;

    // Variable to store the received text
    private string text = string.Empty;

    // Using targetGameObject doesn't work
    // The output target GameObject to receive text from this gameObject.
    //[SerializeField]
    //private GameObject targetGameObject;
    //private InputField target;


    void Start()
    {
        //Start listening at port 3000
        listener = new HttpListener();
        //listener.Prefixes.Add("http://localhost:3000/");
        listener.Prefixes.Add("http://*:3000/");
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        listener.Start();

        listenerThread = new Thread(startListener);
        listenerThread.Start();
        Debug.Log("Server Started");

        // Get the InputField for placing received text
        inputField = gameObject.GetComponent<InputField>();

        // *** Using targetGameobject doesn't work. ***

        // Get the targetGameobject to receive the text
        //if (targetGameObject != null)
        //{
        //    target = targetGameObject.GetComponent<InputField>();
        //}

    }

    void Update()
    {

/* The inputField text is set during Update because I didn't work to try to just
 * set it right after receiving text in ListenerCallback. The program gets stuck at
 * that step.*/
        inputField.text = text;

        if (resetTextEachTime)
        {
            text = string.Empty;
        }

        // Using targetGameObject doesn't work
/*        target.text = "how are you doing?";
        if (target != null)
        {
            target.text = text;
        }
*/
    }

    private void startListener()
    {
        while (true)
        {
            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);

        Debug.Log("Method: " + context.Request.HttpMethod);
        Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

        if (context.Request.QueryString.AllKeys.Length > 0)
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                // Store the recieved text
                text = context.Request.QueryString.GetValues(key)[0];
                Debug.Log("Key: " + key + ", Value: " + text);

/* Trying to set the inputField here didn't work. The program gets stuck here.
 * Setting the text in the InputField is thus done in Update.
 */
//                inputField.text = text;
            }

        if (context.Request.HttpMethod == "POST")
        {
            Thread.Sleep(1000);
            var data_text = new StreamReader(context.Request.InputStream,
                                context.Request.ContentEncoding).ReadToEnd();
            Debug.Log(data_text);
        }

        context.Response.Close();
    }

}