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

namespace IBM.Cloud.SDK.Authentication.Iam
{
    /// <summary>
    /// This class implements support for the IAM authentication mechanism.
    /// </summary>
    public class IamAuthenticator : Authenticator
    {
        // Configuration properties for this authenticator.
        public string Apikey { get; private set; }
        public bool? DisableSslVerification { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        // This field holds an access token and its expiration time.
        private IamToken tokenData;

        private const string DefaultIamUrl = "https://iam.cloud.ibm.com/identity/token";
        private const string GrantType = "grant_type";
        private const string RequestGrantType = "urn:ibm:params:oauth:grant-type:apikey";
        private const string ApikeyConst = "apikey";
        private const string ResponseType = "response_type";
        private const string CloudIam = "cloud_iam";

        /// <summary>
        /// Constructs an IamAuthenticator with all properties.
        /// </summary>
        /// <param name="apikey">The apikey to be used when retrieving the access token</param>
        /// <param name="url">The URL representing the token server endpoint</param>
        /// <param name="clientId">The clientId to be used in token server interactions</param>
        /// <param name="clientSecret">The clientSecret to be used in token server interactions</param>
        /// <param name="disableSslVerification">A flag indicating whether SSL hostname verification should be disabled</param>
        /// <param name="headers">A set of user-supplied headers to be included in token server interactions</param>
        public IamAuthenticator(string apikey, string url = null, string clientId = null, string clientSecret = null, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Init(apikey, url, clientId, clientSecret, disableSslVerification, headers);
        }

        /// <summary>
        /// Construct an IamAuthenticator instance using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">A map containing the configuration properties</param>
        public IamAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameUrl, out string url);
            config.TryGetValue(PropNameApikey, out string apikey);
            config.TryGetValue(PropNameClientId, out string clientId);
            config.TryGetValue(PropNameClientSecret, out string clientSecret);
            config.TryGetValue(PropNameDisableSslVerification, out string disableSslVerficiationString);
            bool.TryParse(disableSslVerficiationString, out bool disableSslVerification);
            Init(apikey, url, clientId, clientSecret, disableSslVerification);
        }

        private void Init(string apikey, string url = null, string clientId = null, string clientSecret = null, bool? disableSslVerification = null, Dictionary<string, string> headers = null)
        {
            Apikey = apikey;

            if (string.IsNullOrEmpty(url))
            {
                url = DefaultIamUrl;
            }
            this.Url = url;
            if (!string.IsNullOrEmpty(clientId))
            {
                ClientId = clientId;
            }
            if (!string.IsNullOrEmpty(clientSecret))
            {
                ClientSecret = clientSecret;
            }
            if (disableSslVerification != null)
            {
                DisableSslVerification = disableSslVerification;
            }
            if (headers != null)
            {
                this.Headers = headers;
            }

            Validate();
            GetToken();
        }

        public override string AuthenticationType
        {
            get { return AuthTypeIam; }
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

        private void OnGetToken(DetailedResponse<IamTokenResponse> response, IBMError error)
        {
            if (error != null)
            {
                Log.Error("Credentials.OnRequestIamTokenResponse()", "Exception: {0}", error.ToString());
            }
            else {
                tokenData = new IamToken(response.Result);
            }
        }

        private void GetToken()
        {
            if (tokenData == null || !tokenData.IsTokenValid())
            {
                RequestToken(OnGetToken);
            }
        }

        #region Request Token
        /// <summary>
        /// Request an IAM token using an API key.
        /// </summary>
        /// <param name="callback">The request callback.</param>
        /// <returns></returns>
        bool RequestToken(Callback<IamTokenResponse> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("successCallback");

            RESTConnector connector = new RESTConnector();
            connector.URL = Url;
            if (connector == null)
                return false;

            RequestIamTokenRequest req = new RequestIamTokenRequest();
            req.Callback = callback;
            req.HttpMethod = UnityWebRequest.kHttpVerbGET;
            req.Headers.Add("Content-type", "application/x-www-form-urlencoded");
            // If both the clientId and secret were specified by the user, then use them.
            if (!string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret))
            {
                req.Headers.Add("Authorization", Utility.CreateAuthorization(ClientId, ClientSecret));
            }
            req.OnResponse = OnRequestIamTokenResponse;
            req.DisableSslVerification = DisableSslVerification;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms[GrantType] = new RESTConnector.Form(RequestGrantType);
            req.Forms[ApikeyConst] = new RESTConnector.Form(Apikey);
            req.Forms[ResponseType] = new RESTConnector.Form(CloudIam);

            return connector.Send(req);
        }

        private class RequestIamTokenRequest : RESTConnector.Request
        {
            public Callback<IamTokenResponse> Callback { get; set; }
        }

        private void OnRequestIamTokenResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<IamTokenResponse> response = new DetailedResponse<IamTokenResponse>();
            response.Result = new IamTokenResponse();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IamTokenResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("Credentials.OnRequestIamTokenResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestIamTokenRequest)req).Callback != null)
                ((RequestIamTokenRequest)req).Callback(response, resp.Error);
        }
        #endregion

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Apikey))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "apikey"));
            }

            if (Utility.HasBadFirstOrLastCharacter(Apikey))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "apikey"));
            }

            if (Utility.HasBadFirstOrLastCharacter(Url))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "url"));
            }

            if (!string.IsNullOrEmpty(ClientSecret) && string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret) && !string.IsNullOrEmpty(ClientId))
            {

                throw new ArgumentException("Client ID and Secret must BOTH be provided.");
            }
        }
    }
}