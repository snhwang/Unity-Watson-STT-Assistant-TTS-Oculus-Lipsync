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
using System;
using System.Collections.Generic;
using Utility = IBM.Cloud.SDK.Utilities.Utility;

namespace IBM.Cloud.SDK.Authentication.Bearer
{
    /// <summary>
    /// This class implements support for Bearer Token Authentication. The main purpose of this authenticator is to construct the
    /// Authorization header and then add it to each outgoing REST API request.
    /// </summary>
    public class BearerTokenAuthenticator : Authenticator
    {
        /// <summary>
        /// The access token configured for this authenticator
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// Construct a BearerTokenAuthenticator instance with the specified access token.
        /// The token value will be used to construct an Authorization header that will be included
        /// in outgoing REST API requests.
        /// </summary>
        /// <param name="bearerToken">The access token value</param>
        public BearerTokenAuthenticator(string bearerToken)
        {
            Init(bearerToken);
        }

        /// <summary>
        /// Construct a BearerTokenAuthenticator using properties retrieved from the specified Map.
        /// </summary>
        /// <param name="config">Config a map containing the access token value</param>
        public BearerTokenAuthenticator(Dictionary<string, string> config)
        {
            config.TryGetValue(PropNameBearerToken, out string bearerToken);
            Init(bearerToken);
        }

        private void Init(string bearerToken)
        {
            BearerToken = bearerToken;

            Validate();
        }

        /// <summary>
        /// Do we have BearerToken?
        /// </summary>
        /// <returns></returns>
        public override bool CanAuthenticate()
        {
            return BearerToken != null;
        }

        public override string AuthenticationType
        {
            get { return AuthTypeBearer; }
        }

        /// <summary>
        /// This method is called to authenticate an outgoing REST API request.
        /// Here, we'll just set the Authorization header to provide the necessary authentication info.
        /// </summary>
        /// <param name="connector"></param>
        public override void Authenticate(RESTConnector connector)
        {
            connector.WithAuthentication(BearerToken);
        }

        /// <summary>
        /// This method is called to authenticate an outgoing REST API request.
        /// Here, we'll just set the Authorization header to provide the necessary authentication info.
        /// </summary>
        /// <param name="connector"></param>
        public override void Authenticate(WSConnector connector)
        {
            connector.WithAuthentication(BearerToken);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(BearerToken))
            {
                throw new ArgumentNullException(string.Format(ErrorMessagePropMissing, "BearerToken"));
            }

            if (Utility.HasBadFirstOrLastCharacter(BearerToken))
            {
                throw new ArgumentException(string.Format(ErrorMessagePropInvalid, "BearerToken"));
            }
        }
    }
}