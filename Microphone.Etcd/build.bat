del *.nupkg
nuget pack Microphone.Etcd.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.Etcd.0.1.4.0.nupkg
