/*
 * Copyright 2019 Scott Hwang. All Rights Reserved.
 * This code was modified from ExampleAssistantV2.cs 
 * in unity-sdk-4.0.0. This continueds to be licensed 
 * under the Apache License, Version 2.0 as noted below.
 * 
 * I added the Watson text-to-speech service and
 * a flag to communicate with SpeechInput.cs.
 * 
 * I also incorporated the use of the chatbot to execute
 * commands as demonstrated by:
 * 
 * https://www.youtube.com/watch?v=OsbV1xqX0hQ
 * https://github.com/IBM/vr-speech-sandbox-cardboard
 * https://developer.ibm.com/patterns/create-a-virtual-reality-speech-sandbox/
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
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using IBM.Watson.TextToSpeech.V1;

using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.Examples
{
    public class CommandBot: SimpleBot
    {
        //private bool messageTested = false;

        protected override void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
        {
            Debug.Log("New OnMessage");

            textResponse = response.Result.Output.Generic[0].Text.ToString();
            messageTested = true;

            if (response != null && response.Result.Output.Intents.Count != 0)
            {
                string intent = response.Result.Output.Intents[0].Intent;
                Debug.Log("Intent: " + intent);
                string currentMat = null;
                string currentScale = null;
                string direction = null;
                if (intent == "move")
                {
                    foreach (RuntimeEntity entity in response.Result.Output.Entities)
                    {
                        Debug.Log("entityType: " + entity.Entity + " , value: " + entity.Value);
                        direction = entity.Value;
                        //gameManager.MoveObject(direction);
                    }
                }

                if (intent == "create")
                {
                    bool createdObject = false;
                    
                    GameObject myObject = null;
                    Renderer rend = null;
                    string ObjectType = null;
                    Color ObjectColor = Color.red;

                    foreach (RuntimeEntity entity in response.Result.Output.Entities)
                    {
                        Debug.Log("entityType: " + entity.Entity + " , value: " + entity.Value);
                        if (entity.Entity == "object")
                        {
                            //gameManager.CreateObject(entity.Value, currentMat, currentScale);
                            createdObject = true;
                            currentMat = null;
                            currentScale = null;
                            ObjectType = entity.Value;

                        }
                        if (entity.Entity == "material")
                        {
                            currentMat = entity.Value;

                            if (currentMat == "black")
                            {
                                ObjectColor = Color.black;
                            }
                            else if (currentMat == "blue")
                            {
                                ObjectColor = Color.blue;
                            }
                            else if (currentMat == "cyan")
                            {
                                ObjectColor = Color.cyan;
                            }
                            else if (currentMat == "gray")
                            {
                                ObjectColor = Color.gray;
                            }
                            else if (currentMat == "green")
                            {
                                ObjectColor = Color.green;
                            }
                            else if (currentMat == "magenta")
                            {
                                ObjectColor = Color.magenta;
                            }
                            else if (currentMat == "red")
                            {
                                ObjectColor = Color.red;
                            }
                            else if (currentMat == "white")
                            {
                                ObjectColor = Color.white;
                            }
                            else if (currentMat == "yellow")
                            {
                                ObjectColor = Color.yellow;
                            }


                        }
                        if (entity.Entity == "scale")
                        {
                            currentScale = entity.Value;
                        }

                        if (ObjectType != null)
                        {
                            if (ObjectType == "cube")
                            {
                                myObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                myObject.transform.position = new Vector3(0f, 1f, 0f);
                                rend = myObject.GetComponent<Renderer>();
                                rend.material.color = Color.red;
                            }
                            else if (ObjectType == "ball")
                            {
                                myObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                myObject.transform.position = new Vector3(0f, 1f, 0f);
                                rend = myObject.GetComponent<Renderer>();
                                rend.material.color = Color.red;
                            }
                            rend = myObject.GetComponent<Renderer>();
                            rend.material.color = ObjectColor;
                        }
                        
                    }

                    if (!createdObject)
                    {
                        //gameManager.PlayError(sorryClip);
                    }

                }
                else if (intent == "destroy")
                {
                    //gameManager.DestroyAtPointer();
                }
                else if (intent == "help")
                {

                }
            }
            else
            {
                Debug.Log("Failed to invoke OnMessage();");
            }
        }

    }

}
