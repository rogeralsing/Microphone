del *.nupkg
nuget pack Microphone.Core.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.Core.0.1.4.0.nupkg
