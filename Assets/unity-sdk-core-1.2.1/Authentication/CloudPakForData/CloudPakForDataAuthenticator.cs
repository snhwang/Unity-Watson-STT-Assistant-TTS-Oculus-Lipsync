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

using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Utility = IBM.Cloud.SDK.Utilities.Utility;

namespace IBM.Cloud.SDK.Authentication.Cp4d
{
    /// <summary>
    /// This class implements support for the CP4D authentication mechanism.
    /// </summary>
    public class CloudPakForDataAuthenticator : Authenticator
    {
        // This is the suffix we'll need to add to the user-supplied URL to retrieve an access token.
        private static string UrlSuffix = "/v1/preauth/validateAuth";

        // Configuration properties for this authenticator.
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool? DisableSslVerification { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        // This field holds an access token and its expiration time.
        private CloudPakForDataToken tokenData;

        /// <summary>
        /// Constructs a CloudPakForDataAuthenticator with all properties.
        /// </summary>
        /// <param name="url">The base URL associated with the token server. The path "/v1/preauth/validateAuth" will be appended to this value automatically.</param>
        /// <param name="username">The username to be used when retrieving the access token</param>
        /// <param name="password">The password to be used when retrieving the access token</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions</param>
        public CloudPakForDataAuthenticator(string url, string username, string password, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(url, username, password, disableSslVerification, headers);
        }

        /// <summary>
        /// Construct a CloudPakForDataAuthenticator instance using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the configuration properties</param>
        public CloudPakForDataAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUrl, out string url);
            config.TryGetValue(PropNameUsername, out string username);
            config.TryGetValue(PropNamePassword, out string password);
            config.TryGetValue(PropNameDisableSslVerification, out string disableSslVerficiationString);
            bool.TryParse(disableSslVerficiationString, out bool disableSslVerification);
            Init(url, username, password, disableSslVerification);
        }

        private void Init(string url, string username, string password, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Url = url;
            Username = username;
            Password = password;

            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }

            if (headers != null)
            {
                Headers = headers;
            }

            Validate();
            GetToken();
        }

        public override string AuthenticationType
        {
            get { return AuthTypeCp4d; }
        }

        /// <summary>
        /// Do we have TokenData?
        /// </summary>
        /// <returns></returns>
        public override bool CanAuthenticate()
        {
            if (tokenData != null)
            {
                return tokenData.AccessToken != null;
            }
            else
            {
                return false;
            }
        }

        public override void Authenticate(RESTConnector connector)
        {
            connector.WithAuthentication(tokenData.AccessToken);
        }

        public override void Authenticate(WSConnector connector)
        {
            connector.WithAuthentication(tokenData.AccessToken);
        }

        private void GetToken()
        {
            if (tokenData == null || !tokenData.IsTokenValid())
            {
                RequestToken(OnGetToken);
            }
        }

        private void OnGetToken(DetailedResponse<CloudPakForDataTokenResponse> response, IBMError error)
        {
            if (error != null)
            {
                Log.Error("Credentials.OnRequestCloudPakForDataTokenResponse()", "Exception: {0}", error.ToString());
            }
            else {
                tokenData = new CloudPakForDataToken(response.Result);
            }
        }

        private bool RequestToken(Callback<CloudPakForDataTokenResponse> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("successCallback");

            RESTConnector connector = new RESTConnector();
            connector.URL = Url;
            if (connector == null)
                return false;

            RequestCloudPakForDataTokenRequest req = new RequestCloudPakForDataTokenRequest();
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.Callback = callback;
            req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
            req.Headers.Add("Authorization", Utility.CreateAuthorization(Username, Password));
            req.OnResponse = OnRequestCloudPakForDataTokenResponse;
            req.DisableSslVerification = DisableSslVerification;
            return connector.Send(req);
        }

        private class RequestCloudPakForDataTokenRequest : RESTConnector.Request
        {
            public Callback<CloudPakForDataTokenResponse> Callback { get; set; }
        }

        private void OnRequestCloudPakForDataTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CloudPakForDataTokenResponse> response = new DetailedResponse<CloudPakForDataTokenResponse>();
            response.Result = new CloudPakForDataTokenResponse();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CloudPakForDataTokenResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("Credentials.OnRequestCloudPakForDataTokenResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }
            if (((RequestCloudPakForDataTokenRequest)req).Callback != null)
                ((RequestCloudPakForDataTokenRequest)req).Callback(response, resp.Error);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Url"));
            }

            if (string.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Username"));
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "Password"));
            }

            if (Utility.HasBadFirstOrLastCharacter(Url))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Url"));
            }

            if (Utility.HasBadFirstOrLastCharacter(Username))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Username"));
            }

            if (Utility.HasBadFirstOrLastCharacter(Password))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "Password"));
            }
        }
    }
}