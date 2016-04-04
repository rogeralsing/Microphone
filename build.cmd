@echo off
cls
".nuget\NuGet.exe" update -self
".nuget\NuGet.exe" install FAKE -ConfigFile .nuget\Nuget.Config -OutputDirectory packages -ExcludeVersion -Version 4.16.1
"packages\FAKE\tools\Fake.exe" build.fsx %*