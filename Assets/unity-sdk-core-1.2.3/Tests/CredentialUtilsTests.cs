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

using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Utilities;
using NUnit.Framework;
using IBM.Cloud.SDK.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IBM.Cloud.SDK.Tests
{
    public class CredentialUtilsTests
    {
        [Test]
        public void TestGetVcapCredentialsAsMap()
        {
            var apikey = "bogus-apikey";
            var service1_apikey = "V4HXmoUtMjohnsnow=KotN";
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = apikey
                }
            };

            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = service1_apikey
                }
            };

            vcapCredential2.Name = "equals_sign_test";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });
            tempVcapCredential.Add("equals_sign_test", new List<VcapCredential>() { vcapCredential2 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == apikey);

            vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("equals_sign_test");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey2);
            Assert.IsTrue(extractedKey2 == service1_apikey);
        }

        [Test]
        public void TestGetVcapCredentialsAsMapFromInnerEntry()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey"
                }
            };
            vcapCredential.Name = "assistant1";

            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey2"
                }
            };
            vcapCredential2.Name = "assistant2";

            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "fakeapikey3"
                }
            };
            vcapCredential3.Name = "assistant3";
            //map to a single key
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential, vcapCredential2, vcapCredential3 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant2");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "fakeapikey2");
        }

        [Test]
        public void TestGetVcapCredentialsAsMapInnerEntryMultKeys()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("someService", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map creds to second service
            tempVcapCredential.Add("someOtherService", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            //should match with inner entry with name "assistantV1Copy"
            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistantV1Copy");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikeyCopy");
        }

        [Test]
        public void TestGetVcapCredentialsAsMapDuplicateName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map creds to second service
            tempVcapCredential.Add("assistantV1", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            //should match with inner entry with name "assistantV1"
            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistantV1");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [Test]
        public void TestGetVcapCredentialsAsMapNoMatchingName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            var vcapCredential2 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikeyCopy"
                }
            };
            vcapCredential2.Name = "assistantV1Copy";
            //map to creds to first service
            tempVcapCredential.Add("no_matching_name", new List<VcapCredential>() { vcapCredential, vcapCredential2 });

            //create credential entries for second service entry
            var vcapCredential3 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikey"
                }
            };
            vcapCredential3.Name = "assistantV2";

            var vcapCredential4 = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV2apikeyCopy"
                }
            };
            vcapCredential4.Name = "assistantV2Copy";

            //map to second service
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential3, vcapCredential4 });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("no_matching_name");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [Test]
        public void TestGetVcapCredentialsAsMapMissingNameField()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("assistant");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            vcapCredentaialsAsMap.TryGetValue(
                Authenticator.PropNameApikey,
                out string extractedKey);
            Assert.IsTrue(extractedKey == "assistantV1apikey");
        }

        [Test]
        public void TestGetVcapCredentialsAsMapEntryNotFound()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("fake_entry");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [Test]
        public void TestGetVcapCredentialsAsMapVcapNotSet()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("fake_entry");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [Test]
        public void TestGetVcapCredentialsAsMapEmptySvcName()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = "assistantV1apikey"
                }
            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [Test]
        public void TestGetVcapCredentialsAsMapNoCreds()
        {
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            //create credential entries for first service entry
            var vcapCredential = new VcapCredential()
            {

            };
            vcapCredential.Name = "assistantV1";
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var vcapCredentaialsAsMap = CredentialUtils.GetVcapCredentialsAsMap("no-creds");
            Assert.IsNotNull(vcapCredentaialsAsMap);
            Assert.IsTrue(vcapCredentaialsAsMap.Count == 0);
        }

        [Test]
        public void TestGetServiceProperties()
        {
            var apikey = "bogus-apikey";
            var tempVcapCredential = new Dictionary<string, List<VcapCredential>>();
            var vcapCredential = new VcapCredential()
            {
                Credentials = new Credential()
                {
                    ApiKey = apikey
                }
            };
            tempVcapCredential.Add("assistant", new List<VcapCredential>() { vcapCredential });

            var vcapString = JsonConvert.SerializeObject(tempVcapCredential);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", vcapString);
            Assert.IsNotNull(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            var serviceProperties = CredentialUtils.GetServiceProperties("assistant");

            Assert.IsNotNull(serviceProperties);
        }

        [Test]
        public void TestGetFileCredentialsAsMapService1()
        {
            // store and clear user set env variable
            string ibmCredFile = Environment.GetEnvironmentVariable("IBM_CREDENTIALS_FILE");
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", "");

            //  create .env file in current directory
            string[] linesWorking = { "SERVICE_1_AUTH_TYPE=iam",
                                      "SERVICE_1_APIKEY=V4HXmoUtMjohnsnow=KotN",
                                      "SERVICE_1_CLIENT_ID=somefake========id",
                                      "SERVICE_1_CLIENT_SECRET===my-client-secret==",
                                      "SERVICE_1_AUTH_URL=https://iamhost/iam/api=",
                                      "SERVICE_1_AUTH_DISABLE_SSL=" };
            var directoryPath = Directory.GetCurrentDirectory();
            var credsFile = Path.Combine(directoryPath, "ibm-credentials.env");

            using (StreamWriter outputFile = new StreamWriter(credsFile))
            {
                foreach (string line in linesWorking)
                {
                    outputFile.WriteLine(line);
                }
            }

            //  get props
            Dictionary<string, string> propsWorking = CredentialUtils.GetFileCredentialsAsMap("service_1");
            Assert.IsNotNull(propsWorking);
            Assert.AreEqual(propsWorking["AUTH_TYPE"], "iam");
            Assert.AreEqual(propsWorking["APIKEY"], "V4HXmoUtMjohnsnow=KotN");
            Assert.AreEqual(propsWorking["CLIENT_ID"], "somefake========id");
            Assert.AreEqual(propsWorking["CLIENT_SECRET"], "==my-client-secret==");
            Assert.AreEqual(propsWorking["AUTH_URL"], "https://iamhost/iam/api=");
            Assert.IsFalse(propsWorking.ContainsKey("DISABLE_SSL"));
            //  delete created env files
            if (File.Exists(credsFile))
            {
                File.Delete(credsFile);
            }
            //  reset env variable
            Environment.SetEnvironmentVariable("IBM_CREDENTIALS_FILE", ibmCredFile);
        }

        [Test]
        public void TestGetEnvCredentialsAsMapService1()
        {
            var apikey = "V4HXmoUtMjohnsnow=KotN";
            var authType = "iam";
            var clientId = "somefake========id";
            var clientIdSecret = "==my-client-secret==";
            var authUrl = "https://iamhost/iam/api=";

            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameApikey,
                apikey);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameAuthType,
                authType);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameClientId,
                clientId);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameClientSecret,
                clientIdSecret);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameUrl,
                authUrl);
            // get props
            Dictionary<string, string> props = CredentialUtils.GetEnvCredentialsAsMap("service_1");
            Assert.IsNotNull(props);
            Assert.AreEqual(props["AUTH_TYPE"], authType);
            Assert.AreEqual(props["APIKEY"], apikey);
            Assert.AreEqual(props["CLIENT_ID"], clientId);
            Assert.AreEqual(props["CLIENT_SECRET"], clientIdSecret);
            Assert.AreEqual(props["AUTH_URL"], authUrl);

            //  delete created env files
            Environment.SetEnvironmentVariable(
               "SERVICE_1_" + Authenticator.PropNameApikey,
               null);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameAuthType,
                null);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameClientId,
                null);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameClientSecret,
                null);
            Environment.SetEnvironmentVariable(
                "SERVICE_1_" + Authenticator.PropNameUrl,
                null);
        }
    }
}
