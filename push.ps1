# params
param(
    # nupkg file
    [Parameter(Mandatory=$true)]
    [string]$nupkg
    , [string]$v1, [string]$v, [string]$port
)

$port = "5555"
# Debug:
# $port = "5001"

# [CmdletBinding(SupportsShouldProcess=$true)]
function push([string] $nupkg) 
{
    Write-host "dotnet nuget push -s http://localhost:$port/v3/index.json -k 12345 $nupkg"
    dotnet nuget push -s http://localhost:$port/v3/index.json -k 12345 $nupkg
}

function delete([string] $nupkg1, [string]$ver) { 
# ="1.0.0") {
    Write-Host "dotnet nuget delete $nupkg1 $ver -s http://localhost:$port/v3/index.json -k 12345"
    dotnet nuget delete $nupkg1 $ver -s http://localhost:$port/v3/index.json -k 12345
}

#debug
Write-Host "$nupkg $v1 $v"

if ($nupkg -eq 'delete') {
  delete($v1, $v)
}
else {
  push($nupkg)
}
