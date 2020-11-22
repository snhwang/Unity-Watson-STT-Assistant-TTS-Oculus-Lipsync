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

using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.NoAuth;
using System;
using System.Collections.Generic;

namespace IBM.Cloud.SDK
{
    public class BaseService
    {
        #region Authenticator
        /// <summary>
        /// Gets and sets the authenticator of the service.
        public Authenticator Authenticator { get; set; }
        #endregion

        #region Connector
        private RESTConnector _connector;
        /// <summary>
        /// Gets and sets the Connector of the service.
        /// </summary>
        public RESTConnector Connector
        {
            get { return _connector; }
            set { _connector = value; }
        }
        #endregion

        protected string serviceUrl;
        public string ServiceId { get; set; }
        protected Dictionary<string, string> customRequestHeaders = new Dictionary<string, string>();
        public static string PropNameServiceUrl = "URL";
        public static string PropNameServiceDisableSslVerification = "DISABLE_SSL";
        private const string ErrorMessageNoAuthenticator = "Authentication information was not properly configured.";

        public BaseService(string versionDate, Authenticator authenticator, string serviceId) : this(authenticator, serviceId) { }

        public BaseService(Authenticator authenticator, string serviceId) {
            ServiceId = serviceId;

            Authenticator = authenticator ?? throw new ArgumentNullException(ErrorMessageNoAuthenticator);
            // Try to retrieve the service URL from either a credential file, environment, or VCAP_SERVICES.
            Dictionary<string, string> props = CredentialUtils.GetServiceProperties(serviceId);
            props.TryGetValue(PropNameServiceUrl, out string url);
            if (!string.IsNullOrEmpty(url))
            {
                SetServiceUrl(url);
            }

            Connector = new RESTConnector();
        }

        public void SetServiceUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("The serviceUrl must not be empty or null.");
            }
            if (Utility.HasBadFirstOrLastCharacter(url))
            {
                throw new ArgumentException("The serviceUrl property is invalid. Please remove any surrounding {{, }}, or \" characters.");
            }
            serviceUrl = url;
        }

        public string GetServiceUrl()
        {
            return serviceUrl;
        }

        /// <summary>
        /// Returns the authenticator for the service.
        /// </summary>
        public Authenticator GetAuthenticator()
        {
            return Authenticator;
        }

        public void WithHeader(string name, string value)
        {
            if (!customRequestHeaders.ContainsKey(name))
            {
                customRequestHeaders.Add(name, value);
            }
            else
            {
                customRequestHeaders[name] = value;
            }
        }

        public void WithHeaders(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                if (!customRequestHeaders.ContainsKey(kvp.Key))
                {
                    customRequestHeaders.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    customRequestHeaders[kvp.Key] = kvp.Value;
                }
            }
        }

        protected void ClearCustomRequestHeaders()
        {
            customRequestHeaders = new Dictionary<string, string>();
        }
    }
}
