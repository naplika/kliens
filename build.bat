@echo off
rmdir /s /q "bin\Release\net8.0\"
del /q build\*
rmdir /s /q build
mkdir build

dotnet publish --no-dependencies --self-contained true --os linux
tar -cf build\linux-x64.zip -c bin\Release\net8.0\linux-x64\publish .

dotnet publish --no-dependencies --self-contained true --os osx
tar -cf build\osx-x64.zip -c bin\Release\net8.0\osx-x64\publish .

dotnet publish --no-dependencies --self-contained true --os win
tar -cf build\win-x64.zip -c bin\Release\net8.0\win-x64\publish .