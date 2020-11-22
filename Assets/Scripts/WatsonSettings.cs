/* Create a scriptable object stored was WatsonSettings.asset to store IBM Watson credentials.
 * This appears as a menu item in the Unity Editor under Assets->Create. You can create more than
 * one set of credential settings.
 * 
 * Unity has a tutorial about scriptable objects:
 * https://learn.unity.com/tutorial/introduction-to-scriptable-objects#
 */

using UnityEditor;
using UnityEngine;

public class WatsonSettings : ScriptableObject
{
    [Space(10)]
    [Header("IBM Watson Assistant")]
    [Tooltip("The IAM apikey.")]
    public string Assistant_apikey; // The apikey for IBM Watson Assistant
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
    public string serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    public string versionDate;
    [Tooltip("The assistantId to run the example.")]
    public string assistantId;

    [Space(10)]
    [Header("IBM Watson Text to Speech")]
    public string tts_apikey; // API key for IBM Watson text to speech
    public string tts_serviceUrl; // Service URL for IBM Watson text to speech

    [Space(10)]
    [Header("IBM Watson Speech to Text")]
    [Tooltip("The IAM apikey.")]
    public string stt_apikey;
    [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/speech-to-text/api\"")]
    public string stt_serviceUrl;

}
