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

using IBM.Cloud.SDK.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace IBM.Cloud.SDK.Utilities
{
    public class CredentialUtils
    {
        private static string defaultCredentialFileName = "ibm-credentials.env";
        private static string vcapServices = "VCAP_SERVICES";

        /// <summary>
        /// Returns true if the supplied value begins or ends with curly brackets or quotation marks. Returns false for null
        /// inputs.
        /// </summary>
        /// <param name="credentialValue">The credential value to check</param>
        /// <returns>true if the value starts or ends with these characters and is therefore invalid</returns>
        public static bool HasBadStartOrEndChar(string credentialValue)
        {
            return credentialValue != null
                && (credentialValue.StartsWith("{")
                || credentialValue.StartsWith("\"")
                || credentialValue.EndsWith("}")
                || credentialValue.EndsWith("\""));
        }

        /// <summary>
        /// Gets the <b>VCAP_SERVICES</b> environment variable and return it as a Dictionary.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, List<VcapCredential>> GetVcapServices()
        {
            string envServices = Environment.GetEnvironmentVariable(CredentialUtils.vcapServices);
            if (string.IsNullOrEmpty(envServices))
            {
                return null;
            }

            Dictionary<string, List<VcapCredential>> vcapServices = null;

            try
            {
                vcapServices = JsonConvert.DeserializeObject<Dictionary<string, List<VcapCredential>>>(envServices);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error parsing VCAP_SERVICES: {0}", e.Message));
            }

            return vcapServices;
        }

        /// <summary>
        /// A helper method to retrieve the appropriate 'credentials' JSON property value from the VCAP_SERVICES.
        /// </summary>
        /// <param name="vcapServices">Dictionary<string, List<VcapCredential>> representing the VCAP_SERVICES</param>
        /// <param name="serviceName">The name of the service whose credentials are sought</param>
        /// <returns>The first set of credentials that match the search criteria, service name and plan. May return null</returns>
        private static VcapCredential GetVcapCredentialsObject(Dictionary<string, List<VcapCredential>> vcapServices, string serviceName)
        {
            if (vcapServices == null || vcapServices.Count == 0)
            {
                return null;
            }

            foreach (KeyValuePair<string, List<VcapCredential>> kvp in vcapServices)
            {
                List<VcapCredential> item = kvp.Value;
                if (item != null && item.Count > 0)
                {
                    foreach (VcapCredential credential in item)
                    {
                        if (credential.Name == serviceName)
                        {
                            return credential;
                        }
                    }
                }
            }
            // try to find a service list with the specified key.
            if (vcapServices.TryGetValue(serviceName, out List<VcapCredential> credentials))
            {
                if (credentials == null || credentials.Count == 0)
                {
                    return null;
                }

                return credentials[0];
            }

            return null;
        }


        /// <summary>
        /// Creates a list of files to check for credentials. The file locations are:
        /// Location provided by user-specified IBM_CREDENTIALS_FILE environment variable
        /// System home directory (Unix)
        /// System home directory (Windows)
        /// Top-level directory of the project this code is being called in
        /// </summary>
        /// <returns>List of credential files to check</returns>
        private static List<string> GetFilesToCheck()
        {
            List<string> files = new List<string>();

            string userSpecifedPath = Environment.GetEnvironmentVariable("IBM_CREDENTIALS_FILE");
            string unixHomeDirectory = Environment.GetEnvironmentVariable("HOME");
            string windowsFirstHomeDirectory = Environment.GetEnvironmentVariable("HOMEDRIVE") + Environment.GetEnvironmentVariable("HOMEPATH");
            string windowsSecondHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            string projectDirectory = Directory.GetCurrentDirectory();

            if (!string.IsNullOrEmpty(userSpecifedPath) && File.Exists(userSpecifedPath))
            {
                files.Add(userSpecifedPath);
            }

            if (!string.IsNullOrEmpty(projectDirectory))
            {
                var fullPath = Path.GetFullPath(Path.Combine(projectDirectory, defaultCredentialFileName));
                if (File.Exists(fullPath))
                {
                    files.Add(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(unixHomeDirectory))
            {
                var fullPath = Path.GetFullPath(Path.Combine(unixHomeDirectory, defaultCredentialFileName));
                if (File.Exists(fullPath))
                {
                    files.Add(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(windowsFirstHomeDirectory))
            {
                var fullPath = Path.GetFullPath(Path.Combine(windowsFirstHomeDirectory, defaultCredentialFileName));
                if (File.Exists(fullPath))
                {
                    files.Add(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(windowsSecondHomeDirectory))
            {
                var fullPath = Path.GetFullPath(Path.Combine(windowsSecondHomeDirectory, defaultCredentialFileName));
                if (File.Exists(fullPath))
                {
                    files.Add(fullPath);
                }
            }

            return files;
        }

        /// <summary>
        /// Looks through the provided list of files to search for credentials, stopping at the first existing file.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>list of lines in the credential file, or null if no file is found</returns>
        private static List<string> GetFirstExistingFileContents(List<string> files)
        {
            List<string> credentialFileContents = null;
            try
            {
                foreach (string file in files)
                {
                    var contentsArray = File.ReadAllLines(file);
                    credentialFileContents = new List<string>(contentsArray);
                    break;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(string.Format("There was a problem trying to read the credential file: {0}", e.Message));
            }

            return credentialFileContents;
        }

        public static Dictionary<string, string> GetFileCredentialsAsMap(string serviceName)
        {
            List<string> files = GetFilesToCheck();
            List<string> contents = GetFirstExistingFileContents(files);
            if (contents != null && contents.Count > 0)
            {
                return ParseCredentials(serviceName, contents);
            }

            return new Dictionary<string, string>();
        }

        public static Dictionary<string, string> GetEnvCredentialsAsMap(string serviceName)
        {
            var environmentVariables = Environment.GetEnvironmentVariables();

            if (environmentVariables != null && environmentVariables.Count > 0)
            {
                Dictionary<string, string> props = new Dictionary<string, string>();
                serviceName = serviceName.ToUpper();
                foreach (DictionaryEntry de in environmentVariables)
                {
                    string key = de.Key.ToString();
                    string value = de.Value.ToString();

                    if (key.StartsWith(serviceName + "_"))
                    {
                        string credentialName = key.Substring(serviceName.Length + 1);
                        if (!string.IsNullOrEmpty(credentialName) && !string.IsNullOrEmpty(value))
                        {
                            props.Add(credentialName, value);
                        }
                    }
                }

                return props;
            }

            return null;
        }

        /// <summary>
        /// Returns a Map containing properties found within the VCAP_SERVICES environment variable that are associated
        /// with the specified cloud service.
        /// </summary>
        /// <param name="serviceName">The name of the cloud service whose properties should be retrieved</param>
        /// <returns>A Dictionary containing the properties</returns>
        public static Dictionary<string, string> GetVcapCredentialsAsMap(string serviceName)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            var vcapServices = GetVcapServices();
            if (vcapServices == null || vcapServices.Count == 0)
            {
                return props;
            }
            // Retrieve the vcap service entry for the specific key and name, then copy its values to the dictionary.
            VcapCredential serviceCredentials = GetVcapCredentialsObject(vcapServices, serviceName);

            if (serviceCredentials != null)
            {
                AddToDictionary(props, Authenticator.PropNameUsername, serviceCredentials.Credentials.Username);
                AddToDictionary(props, Authenticator.PropNamePassword, serviceCredentials.Credentials.Password);
                AddToDictionary(props, Authenticator.PropNameUrl, serviceCredentials.Credentials.Url);

                // For the IAM apikey, the "apikey" property has higher precedence than "iam_apikey".
                AddToDictionary(props, Authenticator.PropNameApikey, serviceCredentials.Credentials.IamApikey);
                AddToDictionary(props, Authenticator.PropNameApikey, serviceCredentials.Credentials.ApiKey);

                // Try to guess at the auth type based on the properties found.
                if (props.ContainsKey(Authenticator.PropNameApikey))
                {
                    AddToDictionary(props, Authenticator.PropNameAuthType, Authenticator.AuthTypeIam);
                }
                else if (props.ContainsKey(Authenticator.PropNameUsername) || props.ContainsKey(Authenticator.PropNamePassword))
                {
                    AddToDictionary(props, Authenticator.PropNameAuthType, Authenticator.AuthTypeBasic);
                }
            }

            return props;
        }

        /// <summary>
        /// Adds the specified key/value pair to the map if the value is not null or "".
        /// </summary>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        private static void AddToDictionary(Dictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// This function forms a wrapper around the "GetFileCredentialsAsMap", "GetEnvCredentialsAsMap", and
        /// "GetVcapCredentialsAsMap" methods and provides a convenient way to retrieve the configuration
        /// properties for the specified service from any of the three config sources.
        /// The properties are retrieved from one of the following sources (in precendence order):
        /// 1) Credential file
        /// 2) Environment variables
        /// 3) VCAP_SERVICES
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>A Map of properties associated with the service</returns>
        public static Dictionary<string, string> GetServiceProperties(string serviceName)
        {
            Dictionary<string, string> props = GetFileCredentialsAsMap(serviceName);

            if (props == null || props.Count == 0)
            {
                props = GetEnvCredentialsAsMap(serviceName);
            }

            if (props == null || props.Count == 0)
            {
                props = GetVcapCredentialsAsMap(serviceName);
            }

            return props;
        }

        protected static Dictionary<string, string> ParseCredentials(string serviceName, List<string> contents)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            serviceName = serviceName.ToUpper();

            // Within "contents", we're looking for lines of the form:
            //    <serviceName>_<credentialName>=<value>
            //    Example:  ASSISTANT_APIKEY=myapikey
            // Each such line will be parsed into <credentialName> and <value>,
            // and added to the result Map.
            foreach (string line in contents)
            {
                //  Skip commentslines and empty lines
                if (line.StartsWith("#") || string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }

                string[] stringSeparators = new string[] { "=" };
                List<string> lineTokens = new List<string>(line.Split(stringSeparators, 2, StringSplitOptions.None));
                if (lineTokens.Count != 2)
                {
                    continue;
                }

                string key = lineTokens[0];
                string value = lineTokens[1];

                if (key.StartsWith(serviceName + "_"))
                {
                    string credentialName = key.Substring(serviceName.Length + 1);
                    if (!string.IsNullOrEmpty(credentialName) && !string.IsNullOrEmpty(value))
                    {
                        props.Add(credentialName, value);
                    }
                }
            }

            return props;
        }
    }
}