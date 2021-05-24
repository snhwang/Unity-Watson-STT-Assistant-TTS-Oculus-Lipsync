# 3D Chatbot with IBM Watson Speech-To-Text, Assistant, and Text-To-Speech with Oculus Lipsync on Unity

Scott Hwang

LinkedIn: https://www.linkedin.com/in/snhwang

Email: snhwang@alum.mit.edu


05/23/2021
Updated to unity-sdk-core-1.2.3
Updated to Unity 2019.4.26.f1

I also modified TextToSpeech.cs and SpeechToText.cs to update and increase the number of spoken languages to choose from. I included every language that was listed in the Watson API docs. The languages in TextToSpeech which include V3 in their names are higher quality. It seems that IBM removed some of the lower quality voices. That's actually too bad, since they sounded mostly OK and would process faster. You just need to select the Model for the language in Speech To Text (Script) section of the SpeechToText prefabs and the Voice to be used for speaking in the Text To Speech (Script) section of the TextToSpeech prefabs. I have not tested them all.


05/16/2021
Fixed Simplebot.cs so that the Text field targetText is set to the response text from the chat bot. So, in your Unity scene, you can add a Text GameObject to your canvas and set it to targetText in the Inspector so that the text response appears on your canvas. These a new Unity scene in the project "OculusLipsyncChat with output text" which shows an example of this. It is other identical to the other Unity scene "OculusLipsyncChat."

04/27/2021
Fixed error resulting from not checking that all the Watson services are ready.

04/22/2021

Made it work with the provided robot head from Oculus. It's a different type of model than the female avatar. The mouth is just a texture, so you need to use the script OVRLipSyncContextTextureFlip.cs instead of OVRLipSyncContextMorphTarget.cs. There are new scenes labeled with "robot" so you know which ones have the robot head.

There also a scene OculusLipsyncChatMulti which has 2 chatbots. The player is the main camera. You can move around using the arrow keys. The chatbot that is closest to the player is the one that will respond. Each of chatbots can different Watson Settings. In the Resoucres folder there is WatsonSettings and WatsonSettings1. Both need to have the credentials filled in to make the multi-chatbot scene work.

I update to Unity 2019.4.24f1. The Watson Unity sdk is unity-sdk-5.0.2 and the Watson Unity core sdk is unity-sdk-core-1.2.2.


12/6/2020

Fixed the speech-to-text scene.

11/22/2020

This project is based on the project at https://github.com/snhwang/Unity-Watson-STT-Assistant-TTS . There is only minimal difference in the Text-To-Speech code compared to that original repo. However, I've create this separate repo since it may develop in a different direction using other Oculus components. For now, only Oculus Lipsync is included ( https://developer.oculus.com/downloads/package/oculus-lipsync-unity/ ). The idea is similar to that of the orgininal repo which was intended to be used with SALSA lipsync to animate a 3D character's mouth during speaking. Oculus Lipsync is used for this project and is included here since it is free to download. Watson is still used for converting speech to text, generating a chat response with Assistant, and converting the chat response into audio speech. 

I am working on a YouTube video explaining it's setup, but basically you just need to obtain the credentials for the IBM Watson Services and paste them into the project. In the Unity Project, The WatsonSettings file can be found under Assets->Resources->WatsonSettings. Select the WatsonSettings file and fill in the field in the inspector.

The following video shows how to get the credentials for the Text-To-Speech portion for the original project. The process is the same:

https://www.youtube.com/watch?v=Yrtgig6qdhU

The README at the original repo also has some instructions on finding the credentials.

## Implementation

This project was implemented with Unity 2019.4.3f1. For Watson, unity-sdk-4.8.0 ( https://github.com/watson-developer-cloud/unity-sdk/releases ) and unity-sdk-core-1.2.1 ( https://github.com/IBM/unity-sdk-core/releases )were used. Oculus lipsync uses ovr_lipsync_unity_20.0.0 ( https://developer.oculus.com/downloads/package/oculus-lipsync-unity/ ).

The way I did this is probably wrong or at least not optimal. I tried to send the output from Watson text-to-speech to Oculus Lipsync. However, it would either not do anything, or the lips would move but there would be no sound, or there was sound but no lipsyncing. The only way I could get it to work was to have 2 separate AudioSources: one for the Oculus lipsyncing and one for audio.  If somebody can figure out a better way to do this, I would be grateful.

