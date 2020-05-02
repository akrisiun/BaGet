#!/usr/local/bin/pwsh

mkdir -f app/

dotnet restore

$p = $PWD
cd src\BaGet.UI\
yarn 
yarn build

cd $p

dotnet build   src/Baget
dotnet publish src/Baget -o $PWD/app

