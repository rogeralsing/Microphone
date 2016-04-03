msbuild microphonecore.sln /p:Configuration=Debug

cd Microphone.Core
rd bin /s /q
dotnet pack
cd ..

cd Microphone.Etcd
rd bin /s /q
dotnet pack
cd ..

cd Microphone.AspNet
rd bin /s /q
dotnet pack
cd ..

cd Microphone.Nancy
rd bin /s /q
dotnet pack
cd ..

cd Asp5Service\
docker build -t rogeralsing/aspnetmicrophone .
