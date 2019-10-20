# pwsh ./iis7k

$p = "$PWD\dist"

start "http://localhost:7000/"

& "C:\Program Files\IIS Express\iisexpress.exe" /path:$p /port:7000 /trace:error
