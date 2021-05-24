#! /bin/sh
project="unity-sdk-travis"

echo "Attempting to install IBM Unity SDK Core into the test project..."
mkdir -p Travis/UnityTestProject/Assets/Watson/Core
git clone -b $TRAVIS_BRANCH https://github.com/IBM/unity-sdk-core.git Travis/UnityTestProject/Assets/Watson/Core

if [ $? = 0 ] ; then
  echo "IBM Unity SDK Core install SUCCEEDED! Exited with $?"

  echo "Attempting to create Travis/UnityTestProject/Assets/Scripts/Editor/"
  mkdir -p Travis/UnityTestProject/Assets/Scripts/Editor/
  if [ $? = 0 ] ; then
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ SUCCEEDED! Exited with $?"
  else
    echo "Creating Travis/UnityTestProject/Assets/Scripts/Editor/ FAILED! EXITED WITH $?"
  fi
else
  echo "WDC Unity SDK install FAILED! Exited with $?"
  exit 1
fi