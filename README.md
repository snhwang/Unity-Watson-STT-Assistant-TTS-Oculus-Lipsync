# 3D Chatbot with IBM Watson Speech-To-Text, Assistant, and Text-To-Speech with Oculus Lipsync on Unity

Scott Hwang

LinkedIn: https://www.linkedin.com/in/snhwang

11/22/2020

This project is based on the project at https://github.com/snhwang/Unity-Watson-STT-Assistant-TTS . There is only minimal difference in the Text-To-Speech code compared to that original repo. However, I've create this separate repo since it may develop in a different direction using other Oculus components. For now, only Oculus Lipsync is included ( https://developer.oculus.com/downloads/package/oculus-lipsync-unity/ ). The idea is similar to that of the orgininal repo which was intended to be used with SALSA lipsync to animate a 3D character's mouth during speaking. Oculus Lipsync is used for this project and is included here since it is free to download. Watson is still used for converting speech to text, generating a chat response with Assistant, and converting the chat response into audio speech. 

I am working on a YouTube video explaining it's setup, but basically you just need to obtain the credentials for the IBM Watson Services and paste them into the project. In the Unity Project, The WatsonSettings file can be found under Assets->Resources->WatsonSettings. Select the WatsonSettings file and fill in the field in the inspector.

The following video shows how to get the credentials for the Text-To-Speech portion for the original project. The process is the same:

https://www.youtube.com/watch?v=Yrtgig6qdhU

The README at the original repo also has some instructions on finding the credentials.

