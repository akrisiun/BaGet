
$dir = $PWD
dotnet build src/BaGet -c Debug -o /Users/andriusk/Beta/dot.source/BaGet/app

# cd
Set-Location $dir/src/BaGet.UI/
yarn install
yarn run build-app

Set-Location $dir

Set-Location $dir/app/
dotnet BaGet.dll

Set-Location $dir
