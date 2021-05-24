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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using IBM.Cloud.SDK.Connection;

namespace IBM.Cloud.SDK.Tests
{
    public class CoreTests
    {
        [Test]
        public void GetErrorFromArray()
        {
            string json = "{\"errors\":[{\"code\":\"missing_field\",\"message\":\"The request path is not valid. Make sure that the endpoint is correct.\",\"more_info\":\"https://cloud.ibm.com/apidocs/visual-recognition-v4\",\"target\":{\"type\":\"field\",\"name\":\"URL path\"}}],\"trace\":\"4e1b7b85-4dba-4219-b46b-6cdd2e2c06fd\"}";
            RESTConnector restConnector = new RESTConnector();
            string errorMessage = restConnector.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "The request path is not valid. Make sure that the endpoint is correct.");
        }

        [Test]
        public void GetErrorFromError()
        {
            string json = "{\"code\":\"400\",\"error\":\"Error: Too many images in collection\"}";
            RESTConnector restConnector = new RESTConnector();
            string errorMessage = restConnector.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "Error: Too many images in collection");
        }

        [Test]
        public void GetErrorFromMessage()
        {
            string json = "{\"code\":\"string\",\"message\":\"string\"}";
            RESTConnector restConnector = new RESTConnector();
            string errorMessage = restConnector.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "string");
        }

        [Test]
        public void GetErrorFromErrorMessage()
        {
            string json = "{\"code\":\"string\",\"errorMessage\":\"string\"}";
            RESTConnector restConnector = new RESTConnector();
            string errorMessage = restConnector.GetErrorMessage(json);
            Assert.IsTrue(errorMessage == "string");
        }
    }
}
