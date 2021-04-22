/*
 * Copyright 2020, 2019 Scott Hwang. All Rights Reserved.
 * This code was originally modified from example code 
 * in unity-sdk-4.0.0. This continueds to be licensed 
 * under the Apache License, Version 2.0 as noted below.
 *   
 */

/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1;

using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;

public class TextToSpeech : MonoBehaviour
{
    [SerializeField]
    private WatsonSettings settings;

    /* The voices provided by IBM. Voices with V3 are the more advance
     * "neural" voices. You can find a more complete list including voices
     * for other languages at:
     * https://cloud.ibm.com/docs/text-to-speech?topic=text-to-speech-voices#voices
     */
    public enum IBM_voices {
        GB_KateV3Voice,
        US_AllisonVoice,
        US_AllisonV3Voice,
        US_EmilyV3Voice,
        US_HenryV3Voice,
        US_KevinV3Voice,
        US_LisaVoice,
        US_LisaV3Voice,
        US_MichaelVoice,
        US_MichaelV3Voice,
        US_OliviaV3Voice
    }
    [SerializeField]
    private IBM_voices voice = IBM_voices.US_MichaelV3Voice;

    private TextToSpeechService tts_service; // IBM Watson text to speech service
    private IamAuthenticator tts_authenticator; // IBM Watson text to speech authenticator

//Keep track of when the processing of text to speech is complete.
//I don't want processing of text to speech to start until the previous 
//text is processed. Otherwise, short text samples get processed faster
//than longer samples and may be placed on the queue out of order.
//It seems that AudioSource.isPlaying doesn't work reliably.
    public enum ProcessingStatus { Processing, Idle };
    private ProcessingStatus audioStatus;

    [SerializeField]
    private AudioSource outputAudioSource; // The AudioSource for speaking

    [SerializeField]
    private AudioSource oculusLipsyncAudioSource; // AudioSource for Oculus Lipsync

    // A queue for storing the entered texts for conversion to speech audio files
    private Queue<string> textQueue = new Queue<string>();
    // A queue for storing the speech AudioClips for playing
    private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    //public string[] textArray;

    public enum InputFieldTrigger { onValueChanged, onEndEdit };

    [SerializeField]
    private InputField externalInputField;
    [SerializeField]
    private InputFieldTrigger externalTriggerType;

    //[SerializeField]
    private InputField inputField;

    private void Start()
    {
        audioStatus = ProcessingStatus.Idle;
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());

        // Get or make the AudioSource for playing the speech
        if (outputAudioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
            outputAudioSource = gameObject.GetComponent<AudioSource>();
        }

        // Since coroutines can't return values I use the onValueChanged listener
        // to trigger an action after waiting for an input to arrive.
        // I originally used enums or flags to keep track if a process such
        // as obtaining a chat response from IBM Assistant was still being processed
        // or was finished processing but this was cumbersome.
    
        if (externalInputField != null)
        {
            if (externalTriggerType == InputFieldTrigger.onEndEdit)
            {
                externalInputField.onEndEdit.AddListener(delegate { AddTextToQueue(externalInputField.text); });
            }
            else
            {
                externalInputField.onValueChanged.AddListener(delegate { AddTextToQueue(externalInputField.text); });
            }
        }

        inputField = gameObject.AddComponent<InputField>();
        inputField.textComponent = gameObject.AddComponent<Text>();
        inputField.onValueChanged.AddListener(delegate { AddTextToQueue(inputField.text); });
    }

    private void Update()
    {
        // If no AudioClip is playing, convert the next text phrase to
        // audio audio if there is any left in the text queue.
        // The new audio clip is placed into the audio queue.
        if (textQueue.Count > 0 && audioStatus == ProcessingStatus.Idle)
        {
            Debug.Log("Run ProcessText");                
            Runnable.Run(ProcessText());
        }

        // If no AudioClip is playing, remove the next clip from the
        // queue and play it.
        if (audioQueue.Count > 0 && !outputAudioSource.isPlaying)
        {
            PlayClip(audioQueue.Dequeue());
        }

    }

    // Create the IBM text to speech service
    public IEnumerator CreateService()
    {
        //  Create credential and instantiate service
        tts_authenticator = new IamAuthenticator(apikey: settings.tts_apikey);

        //  Wait for tokendata
        while (!tts_authenticator.CanAuthenticate())
            yield return null;

        tts_service = new TextToSpeechService(tts_authenticator);
        if (!string.IsNullOrEmpty(settings.tts_serviceUrl))
        {
            tts_service.SetServiceUrl(settings.tts_serviceUrl);
        }
    }

    private IEnumerator ProcessText()
    {
        Debug.Log("ProcessText");

        string nextText = String.Empty;

        audioStatus = ProcessingStatus.Processing;

        if (outputAudioSource.isPlaying)
        {
            yield return null;
        }

        if (textQueue.Count < 1)
        {
            yield return null;
        }
        else
        {
            nextText = textQueue.Dequeue();
            Debug.Log(nextText);

            if (String.IsNullOrEmpty(nextText))
            {
                yield return null;
            }
        }

        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        tts_service.Synthesize(
            callback: (DetailedResponse<byte[]> response, IBMError error) =>
            {
                synthesizeResponse = response.Result;
                clip = WaveFile.ParseWAV("myClip", synthesizeResponse);

                //Place the new clip into the audio queue.
                audioQueue.Enqueue(clip);
            },
            text: nextText,
            voice: "en-" + voice,
            accept: "audio/wav"
        );

        while (synthesizeResponse == null)
            yield return null;

        // Set status to indicate text to speech processing task completed
        audioStatus = ProcessingStatus.Idle;

    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            outputAudioSource.spatialBlend = 0.0f;
            outputAudioSource.loop = false;
            outputAudioSource.clip = clip;
            if (oculusLipsyncAudioSource != null)
            {
                oculusLipsyncAudioSource.clip = clip;
                oculusLipsyncAudioSource.Play();
            }
            outputAudioSource.Play();
        }
    }

    // Add a text sample to the text queue to be converted into audio
    public void AddTextToQueue(string text)
    {
        Debug.Log("AddTextToQueue: " + text);
        if (!string.IsNullOrEmpty(text))
        {
            textQueue.Enqueue(text);
            inputField.text = string.Empty;
        }

    }

    // Return the status of the conversion to audio
    public ProcessingStatus Status()
    {
        return audioStatus;
    }

    // Check if the text to speech service is ready
    public bool ServiceReady()
    {
        return tts_service != null;
    }

    public bool IsFinished()
    {
        return !outputAudioSource.isPlaying && audioQueue.Count < 1 && textQueue.Count < 1;
    }
}
