del *.nupkg
nuget pack Microphone.Nancy.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.Nancy.0.1.1.0.nupkg
