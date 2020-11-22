/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IBM.Cloud.SDK.Authentication.Cp4d
{
    /// <summary>
    /// This class models a response received from the CloudPakForData "get token" API.
    /// </summary>
    public class CloudPakForDataTokenResponse
    {
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }
        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Permissions { get; set; }
        [JsonProperty("sub", NullValueHandling = NullValueHandling.Ignore)]
        public string Sub { get; set; }
        [JsonProperty("iss", NullValueHandling = NullValueHandling.Ignore)]
        public string Iss { get; set; }
        [JsonProperty("aud", NullValueHandling = NullValueHandling.Ignore)]
        public string Aud { get; set; }
        [JsonProperty("uid", NullValueHandling = NullValueHandling.Ignore)]
        public string Uid { get; set; }
        [JsonProperty("accessToken", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessToken { get; set; }
        [JsonProperty("_messageCode_", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageCode { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}