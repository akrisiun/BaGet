#!/usr/local/bin/pwsh

if ($PsVersionTable.Platform -eq "Unix") {
   mkdir app/
} else {
mkdir -f app/
}

dotnet restore

$p = $PWD
Set-Location src\BaGet.UI\
yarn 
yarn build

Set-Location $p

dotnet build   src/Baget
dotnet publish src/Baget -o $PWD/app

