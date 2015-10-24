del *.nupkg
nuget pack Microphone.Nancy.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget push Microphone.Nancy.0.1.4.0.nupkg
