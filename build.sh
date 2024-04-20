#!/bin/bash

rm -r bin/Release/net8.0/
rm -r build/
mkdir build/

dotnet publish --no-dependencies --self-contained true --os linux
zip -r build/linux-x64.zip bin/Release/net8.0/linux-x64/publish/

dotnet publish --no-dependencies --self-contained true --os osx
zip -r build/osx-x64.zip bin/Release/net8.0/osx-x64/publish/

dotnet publish --no-dependencies --self-contained true --os win
zip -r build/win-x64.zip bin/Release/net8.0/win-x64/publish/