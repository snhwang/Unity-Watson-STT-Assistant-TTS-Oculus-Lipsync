## [1.2.3](https://github.com/IBM/unity-sdk-core/compare/v1.2.2...v1.2.3) (2021-05-12)


### Bug Fixes

* remove package-lock.json ([#42](https://github.com/IBM/unity-sdk-core/issues/42)) ([ea1230f](https://github.com/IBM/unity-sdk-core/commit/ea1230fef03532ee012d30115a6f77ed9077fef2))

## [1.2.2](https://github.com/IBM/unity-sdk-core/compare/v1.2.1...v1.2.2) (2021-02-12)


### Bug Fixes

* **build:** main migration ([5daafd1](https://github.com/IBM/unity-sdk-core/commit/5daafd11c18abb40149ed2a3ba40d784aed6891f))
* **build:** main migration automation ([e2ef3d8](https://github.com/IBM/unity-sdk-core/commit/e2ef3d8df786805dfd090b2048551edd928a3aa2))
* **build:** main migration automation ([82e29a1](https://github.com/IBM/unity-sdk-core/commit/82e29a10c9b2ebcce1e240835780412a0d823cb1))

## [1.2.1](https://github.com/IBM/unity-sdk-core/compare/v1.2.0...v1.2.1) (2020-06-16)


### Bug Fixes

* allow '=' character in environment config values ([89db5eb](https://github.com/IBM/unity-sdk-core/commit/89db5eb00b51467fd80a4bc72d50576bf222307c))
* assign an empty dictionary to RESTConnector's resp.headers if null ([81c3ebe](https://github.com/IBM/unity-sdk-core/commit/81c3ebe7460c62161214001e44309796f94ee7d4))

# [1.2.0](https://github.com/IBM/unity-sdk-core/compare/v1.1.1...v1.2.0) (2019-12-11)


### Features

* **IBMService:** enhance vcap parsing ([262fc55](https://github.com/IBM/unity-sdk-core/commit/262fc55a7fcbedc46a6e8afb3098c54fc6ea0355))

## [1.1.1](https://github.com/IBM/unity-sdk-core/compare/v1.1.0...v1.1.1) (2019-12-04)


### Bug Fixes

* do not throw error before setting defaulr url ([cb0f308](https://github.com/IBM/unity-sdk-core/commit/cb0f3085660e60ac130f3899a4e35aceb7c200dd))

# [1.1.0](https://github.com/IBM/unity-sdk-core/compare/v1.0.0...v1.1.0) (2019-11-27)


### Features

* add support for json sub types ([d1f3809](https://github.com/IBM/unity-sdk-core/commit/d1f38099d03fa02aab9113b655844ca9c363dc5a))
* make connectors settable to use them in unit tests ([6a42a27](https://github.com/IBM/unity-sdk-core/commit/6a42a2724adf33610aba309d28945c82fc9b1223))

# [1.0.0](https://github.com/IBM/unity-sdk-core/compare/v0.3.0...v1.0.0) (2019-10-04)


### Bug Fixes

* fix name for CP4DTokenResponse ([0f71b32](https://github.com/IBM/unity-sdk-core/commit/0f71b32))


### Features

* **auth:** add new authenticator methods ([6716d05](https://github.com/IBM/unity-sdk-core/commit/6716d05))
* **Authentication:** make auth type case insesitive ([4f468e6](https://github.com/IBM/unity-sdk-core/commit/4f468e6))
* **base:** add support for authenticator in base service ([349bc1b](https://github.com/IBM/unity-sdk-core/commit/349bc1b))
* **connectors:** update connectors to work with new design ([9fd52c8](https://github.com/IBM/unity-sdk-core/commit/9fd52c8))
* **dynamicModel:** add dynamic model and meta files ([2b471aa](https://github.com/IBM/unity-sdk-core/commit/2b471aa))
* **setServiceUrl:** use setServiceUrl instead of setServiceEndpoint ([2c33bf8](https://github.com/IBM/unity-sdk-core/commit/2c33bf8))
* add model for file with metadata ([6b536d1](https://github.com/IBM/unity-sdk-core/commit/6b536d1))
* **websocket-dll:** update websocket dll file to import from IBMCloud namespace ([d9b0bc9](https://github.com/IBM/unity-sdk-core/commit/d9b0bc9))


### BREAKING CHANGES

* **setServiceUrl:** Use SetServiceUrl to set the service endpoint rather than SetEndpoint

# [0.2.0](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.2.0) (2019-06-29)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))
* **BaseService:** Fix misnamed properties ([13dd25c](https://github.com/IBM/unity-sdk-core/commit/13dd25c))
* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))


### Features

* **FullSerializer:** Removed FullSerializer, refactored scripts to use Json.net ([3c2550b](https://github.com/IBM/unity-sdk-core/commit/3c2550b))
* **icp4d:** refactor iam token manager and add icp4d support ([c536b63](https://github.com/IBM/unity-sdk-core/commit/c536b63))
* **Json.NET:** Replace Json.NET library with Json.NET for Unity library ([646c1a4](https://github.com/IBM/unity-sdk-core/commit/646c1a4)), closes [#566](https://github.com/IBM/unity-sdk-core/issues/566)

# [0.2.0](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.2.0) (2019-05-10)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))
* **BaseService:** Fix misnamed properties ([13dd25c](https://github.com/IBM/unity-sdk-core/commit/13dd25c))
* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))


### Features

* **FullSerializer:** Removed FullSerializer, refactored scripts to use Json.net ([3c2550b](https://github.com/IBM/unity-sdk-core/commit/3c2550b))
* **Json.NET:** Replace Json.NET library with Json.NET for Unity library ([646c1a4](https://github.com/IBM/unity-sdk-core/commit/646c1a4)), closes [#566](https://github.com/IBM/unity-sdk-core/issues/566)

# [0.2.0](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.2.0) (2019-05-07)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))
* **BaseService:** Fix misnamed properties ([13dd25c](https://github.com/IBM/unity-sdk-core/commit/13dd25c))
* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))


### Features

* **FullSerializer:** Removed FullSerializer, refactored scripts to use Json.net ([3c2550b](https://github.com/IBM/unity-sdk-core/commit/3c2550b))

# [0.2.0](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.2.0) (2019-04-09)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))
* **BaseService:** Fix misnamed properties ([13dd25c](https://github.com/IBM/unity-sdk-core/commit/13dd25c))
* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))


### Features

* **FullSerializer:** Removed FullSerializer, refactored scripts to use Json.net ([3c2550b](https://github.com/IBM/unity-sdk-core/commit/3c2550b))

## [0.1.1](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.1.1) (2019-04-09)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))
* **BaseService:** Fix misnamed properties ([13dd25c](https://github.com/IBM/unity-sdk-core/commit/13dd25c))
* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))

## [0.1.1](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.1.1) (2019-04-09)


### Bug Fixes

* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))
* Look for iam_apikey in credential file for IAM auth ([d5f31d1](https://github.com/IBM/unity-sdk-core/commit/d5f31d1))

## [0.1.1](https://github.com/IBM/unity-sdk-core/compare/v0.1.0...v0.1.1) (2019-04-08)


### Bug Fixes

* **Error message parsing:** Added `errorMessage` to error message parsing for errors from the IAM se ([df91bb6](https://github.com/IBM/unity-sdk-core/commit/df91bb6))

# 1.0.0 (2019-03-28)


### Features

* Added missing datatypess and widgets class for websockets ([9af8e95](https://github.com/IBM/unity-sdk-core/commit/9af8e95))
* Added us culture to minijson ([d13fc23](https://github.com/IBM/unity-sdk-core/commit/d13fc23))
* Correctly parse error message ([85ab306](https://github.com/IBM/unity-sdk-core/commit/85ab306))
* Removed customData ([f3addbc](https://github.com/IBM/unity-sdk-core/commit/f3addbc))
* Removed customData from response, added response json to response object ([4d3675d](https://github.com/IBM/unity-sdk-core/commit/4d3675d))
* **CI:** Added files for CI and semantic release ([44fd1db](https://github.com/IBM/unity-sdk-core/commit/44fd1db))
