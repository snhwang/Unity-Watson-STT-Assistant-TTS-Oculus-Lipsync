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

using JWT;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System;

namespace IBM.Cloud.SDK.Authentication.Cp4d
{
    /// <summary>
    /// This class holds relevant info re: an CP4D access token for use by the CP4DAuthenticator class.
    /// </summary>
    public class CloudPakForDataToken
    {
        public string AccessToken { get; set; }
        public long ExpirationTimeInMillis { get; set; }

        /// <summary>
        /// This ctor is used to store a user-managed access token which will never expire.
        /// </summary>
        /// <param name="accessToken">the user-managed access token</param>
        public CloudPakForDataToken(string accessToken)
        {
            AccessToken = accessToken;
            ExpirationTimeInMillis = -1;
        }

        public CloudPakForDataToken(CloudPakForDataTokenResponse response)
        {
            AccessToken = response.AccessToken;
            long? iat = null;
            long? exp = null;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var decodedResponse = decoder.Decode(AccessToken);

                if (!string.IsNullOrEmpty(decodedResponse))
                {
                    var o = JObject.Parse(decodedResponse);
                    exp = (long)o["exp"];
                    iat = (long)o["iat"];

                    double fractonOfTtl = 0.8d;
                    long timeToLive = (long)exp - (long)iat;
                    ExpirationTimeInMillis = Convert.ToInt64(exp - (timeToLive * (1.0d - fractonOfTtl)));
                }
                else
                {
                    throw new Exception("Access token recieved is not a valid JWT");
                }
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }

            if (iat != null && exp != null)
            {
                long ttl = (long)exp - (long)iat;
                ExpirationTimeInMillis = ((long)iat + (long)(0.8 * ttl)) * 1000;
            }
            else
            {
                throw new Exception("Properties 'iat' and 'exp' MUST be present within the encoded access token");
            }
        }

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(AccessToken) && (ExpirationTimeInMillis < 0 || DateTimeOffset.Now.ToUnixTimeMilliseconds() <= ExpirationTimeInMillis);
        }
    }
}