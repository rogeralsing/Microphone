del *.nupkg
nuget pack Microphone.WebApi.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.WebApi.0.1.5.0.nupkg
