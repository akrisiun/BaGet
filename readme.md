# BaGet :baguette_bread:

pwsh push package one line:
```
foreach ($f in (Get-ChildItem *.nupkg)) { dotnet nuget push -s http://localhost:90/v3/index.json $f; }
# -k $key; }
```

custom build for docker :90 port [http://localhost:90](http://localhost:90)  
```
./dock-b.sh
./dock-run.sh

# OR
docker run -d -p 90:5000   --name baget90 -v ~/Packages:/app/packages baget
# :5005
docker run -d -p 5005:5000 --name baget5005 baget
```

[http://localhost:5005](http://localhost:5005)

## Nuget.config sample

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="~/.nuget/packages" />
  </config>
  <packageSources>
    <clear />
    <add key="local90"   value="http://localhost:90/nuget/nuget/" />   
    <add key="dotnet-core" value="https://dotnetfeed.blob.core.windows.net/dotnet-core/index.json" />
    <add key="NuGet-org" value="https://api.nuget.org/v3/index.json" />
    <!--
    -->
  </packageSources>
</configuration>
```

## Oficial info

A lightweight [NuGet](https://docs.microsoft.com/en-us/nuget/what-is-nuget) and [Symbol](https://docs.microsoft.com/en-us/windows/desktop/debug/symbol-servers-and-symbol-stores) server.

<p align="center">
  <img width="100%" src="https://user-images.githubusercontent.com/737941/50140219-d8409700-0258-11e9-94c9-dad24d2b48bb.png">
</p>

## Getting Started

1. Install [.NET Core SDK](https://www.microsoft.com/net/download)
2. Download and extract [BaGet's latest release](https://github.com/loic-sharma/BaGet/releases)
3. Start the service with `dotnet BaGet.dll`
4. Browse `http://localhost:5000/` in your browser

For more information, please refer to [our documentation](https://loic-sharma.github.io/BaGet/).

## Features

* Cross-platform
* [Dockerized](https://loic-sharma.github.io/BaGet/#running-baget-on-docker)
* [Cloud ready](https://loic-sharma.github.io/BaGet/cloud/azure/)
* [Supports read-through caching](https://loic-sharma.github.io/BaGet/configuration/#enabling-read-through-caching)
* Can index the entirety of nuget.org. See [this documentation](https://loic-sharma.github.io/BaGet/tools/mirroring/)
* Coming soon: Supports [private feeds](https://loic-sharma.github.io/BaGet/private-feeds/)
* And more!

Stay tuned, more features are planned!

## Develop

1. Install [.NET Core SDK](https://www.microsoft.com/net/download) and [Node.js](https://nodejs.org/)
2. Run `git clone https://github.com/loic-sharma/BaGet.git`
3. Navigate to `.\BaGet\src\BaGet.UI`
4. Install the frontend's dependencies with `npm install`
5. Navigate to `..\BaGet`
6. Start the service with `dotnet run`
7. Open the URL `http://localhost:5000/v3/index.json` in your browser
