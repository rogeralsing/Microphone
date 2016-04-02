cd Microphone.Core
dotnet restore
dotnet pack
cd ..

cd Microphone.Etcd
dotnet restore
dotnet pack
cd ..

cd Microphone.AspNet
dotnet restore
dotnet pack
cd ..

cd Microphone.Nancy
dotnet restore
dotnet pack
cd ..