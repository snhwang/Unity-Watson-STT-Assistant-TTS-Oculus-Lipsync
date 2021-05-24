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

using System;

namespace IBM.Cloud.SDK.Authentication.Iam
{
    public class IamToken
    {
        public string AccessToken { get; set; }
        public long ExpirationTimeInMillis { get; set; }

        /// <summary>
        /// This ctor is used to store a user-managed access token which will never expire.
        /// </summary>
        /// <param name="accessToken">the user-managed access token</param>
        public IamToken(string accessToken)
        {
            AccessToken = accessToken;
            ExpirationTimeInMillis = -1;
        }

        public IamToken(IamTokenResponse response)
        {
            AccessToken = response.AccessToken;

            float fractionOfTtl = 0.8f;
            long? timeToLive = response.ExpiresIn;
            long? expireTime = response.Expiration;
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            ExpirationTimeInMillis = (long)expireTime - ((long)timeToLive * (long)(1.0 - fractionOfTtl)) * 1000;
        }

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(AccessToken) && (ExpirationTimeInMillis < 0 || DateTimeOffset.Now.ToUnixTimeMilliseconds() <= ExpirationTimeInMillis);
        }
    }
}