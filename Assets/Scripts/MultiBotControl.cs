using IBM.Cloud.SDK.Utilities;
using UnityEngine;
using System.Collections.Generic;

public class MultiBotControl : MonoBehaviour
{
    //private TextToSpeech tts; // IBM Watson Text to Speech gameobject
    //private SpeechToText stt; // IBM Watson Speech to Text gameobject
    //private SimpleBot chat; // IBM Watson Assistant

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private List<GameObject> botList = new List<GameObject>();

    [SerializeField]
    private int distanceThreshold = 10;

    [SerializeField]
    private int nearestBotIndex;

    private void Start()
    {
        //Runnable.Run(chat.Welcome());
    }

    void Update()
    {
        /* Wait for audio clip of the output speech to finish before listening and converting
         * new input speech
         */

        nearestBotIndex = GetNearestBotIndex();

        for (int i = 0; i < botList.Count; i++)
        {
            SpeechToText stt = botList[i].GetComponentInChildren<SpeechToText>();
            TextToSpeech tts = botList[i].GetComponentInChildren<TextToSpeech>();
            SimpleBot chat = botList[i].GetComponentInChildren<SimpleBot>();

            if (stt.ServiceReady())
            {
                if (nearestBotIndex != i || chat.GetStatus() == SimpleBot.ProcessingStatus.Processing || !tts.IsFinished())
                {
                    stt.Active = false;
                    stt.StopRecording();
                }
                else
                {
                    stt.Active = true;
                    stt.StartRecording();
                    Debug.Log("Bot " + i + " is active.");
                }
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

    public float BotDistance(GameObject bot)
    {
        return Vector3.Distance(bot.transform.position, player.transform.position);
    }

    public int GetNearestBotIndex()
    {
        float smallestDist = BotDistance(botList[0]);
        int smallestIndex = 0;
        GameObject nearestBot = null;

        for (int i = 1; i < botList.Count; i++)
        {
            float nextDist = BotDistance(botList[i]);
            if (nextDist < smallestDist) {
                smallestDist = nextDist;
                smallestIndex = i;
            }
        }
        if (smallestDist > distanceThreshold)
        {
            smallestIndex = -1;
        }
        Debug.Log("smallest index = " + smallestIndex);
        return smallestIndex;
    }
}
