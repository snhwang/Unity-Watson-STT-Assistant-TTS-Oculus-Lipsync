using IBM.Cloud.SDK.Utilities;
using UnityEngine;

public class BotControl : MonoBehaviour
{
    [SerializeField]
    private TextToSpeech tts; // IBM Watson Text to Speech gameobject
    [SerializeField]
    private SpeechToText stt; // IBM Watson Speech to Text gameobject
    [SerializeField]
    private SimpleBot chat; // IBM Watson Assistant

    private void Start()
    {
        Runnable.Run(chat.Welcome());
    }
    void Update()
    {
        /* Wait for audio clip of the output speech to finish before listening and converting
         * new input speech
         */
        if (stt.ServiceReady())
        {
            if (chat.GetStatus() == SimpleBot.ProcessingStatus.Processing || !tts.IsFinished())
            {
                stt.Active = false;
                stt.StopRecording();
            }
            else
            {
                stt.Active = true;
                stt.StartRecording();
            }
        }

        /* I used to need to keep track of all the processing steps to known when to do
         * the next step, i.e. when to ask for a chat response and when to convert to audio.
         * This is now triggered by listeners in InputFields. I still need to keep track
         * of processing to know when to record or stop recording speech as above.
         * /
        /*
        if (stt.GetStatus() == SpeechToText.ProcessingStatus.Processed && chat.ServiceReady())
        {
            // GetResult obtains the result of the speech to text conversion and changes the speech input status to Idle.
            Runnable.Run(chat.ProcessChat(stt.GetResult()));
        }
        */
        /*
        if (chat.GetStatus() == SimpleBot.ProcessingStatus.Processed && tts.ServiceReady())
        {
            // GetResult obtains the chat response and adds it to the queue for conversion to speech audio.
            tts.AddTextToQueue(chat.GetResult());
        }
        */
    }
}
