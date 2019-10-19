#!/usr/local/bin/pwsh
# pwsh push.ps1

$ls = Get-ChildItem *.nupkg
# foreach ($file in $ls) { write-Host $file; }

foreach ($file in $ls)
{
  write-Host $file;
  dotnet nuget push -s http://localhost:90/v3/index.json $file
}

# pwsh -c "Get-ChildItem *.snupkg"
foreach ($file in (Get-ChildItem *.snupkg))
{
  write-Host $file;
  dotnet nuget push -s http://localhost:90/v3/index.json $file
}

# dotnet nuget list -s http://localhost:90/v3/index.json
curl http://localhost:90/v3/index.json

