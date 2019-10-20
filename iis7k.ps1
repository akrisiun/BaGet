# pwsh ./iis7k

# IIS :5000  (or :7000)
# netsh advfirewall firewall add rule name="Open Port 5000" dir=in action=allow protocol=TCP localport=5000
# netsh advfirewall firewall add rule name="Open Port 7000" dir=in action=allow protocol=TCP localport=7000

$p = "$PWD\app"

start "http://localhost:7000/"

& "C:\Program Files\IIS Express\iisexpress.exe" /path:$p /port:7000 /trace:error
