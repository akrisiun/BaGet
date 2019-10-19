#!/usr/local/bin/pwsh
# pwsh pack.ps1

dotnet pack src/BaGet.Core -o $PWD
dotnet pack src/BaGet.Server -o $PWD
dotnet pack src/BaGet.Database.Sqlite -o $PWD

ls *.nupkg