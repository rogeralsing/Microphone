del *.nupkg
nuget pack Microphone.WebApi.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.WebApi.0.1.3.0.nupkg
