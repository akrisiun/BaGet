#!/usr/local/bin/pwsh

mkdir -f app/

dotnet restore
dotnet build   src/Baget
dotnet publish src/Baget -o $PWD/app

