# BaGet

This is the project that implements [NuGet service APIs](https://docs.microsoft.com/en-us/nuget/api/overview).
Most of the core logic is contained within the `BaGet.Core` project.

## IIS problems:

HTTP Error 502.5 - Process Failure   
Common causes of this issue:   
The application process failed to start  
The application process started but then stopped  
The application process started but failed to listen on the configured port
