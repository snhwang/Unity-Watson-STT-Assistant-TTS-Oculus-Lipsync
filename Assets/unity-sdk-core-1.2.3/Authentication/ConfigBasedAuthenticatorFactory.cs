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

using IBM.Cloud.SDK.Authentication.BasicAuth;
using IBM.Cloud.SDK.Authentication.Bearer;
using IBM.Cloud.SDK.Authentication.Cp4d;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Authentication.NoAuth;
using IBM.Cloud.SDK.Utilities;
using System.Collections.Generic;
using System;

namespace IBM.Cloud.SDK.Authentication
{
    public class ConfigBasedAuthenticatorFactory
    {
        public static string ErrorMessageAuthTypeUnknown = "Unrecognized authentication type: {0}";

        public static Authenticator GetAuthenticator(string serviceName)
        {
            Authenticator authenticator = null;

            // Gather authentication-related properties from all the supported config sources:
            // - 1) Credential file
            // - 2) Environment variables
            // - 3) VCAP_SERVICES env variable
            Dictionary<string, string> authProps = new Dictionary<string, string>();

            // First check to see if this service has any properties defined in a credential file.
            authProps = CredentialUtils.GetFileCredentialsAsMap(serviceName);

            // If we didn't find any properties so far, then try the environment.
            if (authProps == null || authProps.Count == 0)
            {
                authProps = CredentialUtils.GetEnvCredentialsAsMap(serviceName);
            }

            // If we didn't find any properties so far, then try VCAP_SERVICES
            if (authProps == null || authProps.Count == 0)
            {
                authProps = CredentialUtils.GetVcapCredentialsAsMap(serviceName);
            }

            // Now create an authenticator from the map.
            if (authProps != null && authProps.Count > 0)
            {
                authenticator = CreateAuthenticator(authProps);
            }

            return authenticator;
        }

        /// <summary>
        /// Instantiates an Authenticator that reflects the properties contains in the specified Map.
        /// </summary>
        /// <param name="props">A Map containing configuration properties</param>
        /// <returns>An Authenticator instance</returns>
        private static Authenticator CreateAuthenticator(Dictionary<string, string> props)
        {
            Authenticator authenticator = null;

            // If auth type was not specified, we'll use "iam" as the default.
            props.TryGetValue(Authenticator.PropNameAuthType, out string authType);
            if (string.IsNullOrEmpty(authType))
            {
                authType = Authenticator.AuthTypeIam;
            }

            if (authType.Equals(Authenticator.AuthTypeNoAuth, StringComparison.InvariantCultureIgnoreCase))
            {
                authenticator = new NoAuthAuthenticator(props);
            }
            else if (authType.Equals(Authenticator.AuthTypeBasic, StringComparison.InvariantCultureIgnoreCase))
            {
                authenticator = new BasicAuthenticator(props);
            }
            else if (authType.Equals(Authenticator.AuthTypeIam, StringComparison.InvariantCultureIgnoreCase))
            {
                authenticator = new IamAuthenticator(props);
            }
            else if (authType.Equals(Authenticator.AuthTypeCp4d, StringComparison.InvariantCultureIgnoreCase))
            {
                authenticator = new CloudPakForDataAuthenticator(props);
            }
            else if (authType.Equals(Authenticator.AuthTypeBearer, StringComparison.InvariantCultureIgnoreCase))
            {
                authenticator = new BearerTokenAuthenticator(props);
            }

            return authenticator;
        }
    }
}