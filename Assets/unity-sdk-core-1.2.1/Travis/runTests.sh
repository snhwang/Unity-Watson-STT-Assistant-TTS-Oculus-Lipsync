#! /bin/sh

set -e

if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
  echo '$TRAVIS_PULL_REQUEST is false, running tests'

echo "Attempting to run IBM Unity SDK Core Tests..."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -runEditorTests \
  -logFile $(pwd)/integrationTests.log \
  -projectPath $(pwd)/Travis/UnityTestProject \
  -testPlatform playmode \
  -testResults $(pwd)/test-results.xml \
  -editorTestsResultFile $(pwd)/test-results.xml \
  -quit

if [ $? = 0 ] ; then
  echo "Tests COMPLETED! Exited with $?"
  echo 'Test logs'
  cat $(pwd)/integrationTests.log
  cat $(pwd)/test-results.xml
  exit 0
else
  echo "Tests FAILED! Exited with $?"
  echo 'Test logs'
  cat $(pwd)/integrationTests.log
  cat $(pwd)/test-results.xml
  exit 1
fi
else
  echo '$TRAVIS_PULL_REQUEST is not false ($TRAVIS_PULL_REQUEST), skipping tests'
fi
