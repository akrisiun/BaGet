using System;
using BaGet.Core;
using BaGet.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaGet
{
    public class Program
    {
		// Default
		const string hostUrl_Default = "http://localhost:5000";
		
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "baget",
                Description = "A light-weight NuGet service",
            };
			
			var allargs = string.Join(" ", args);
			Console.WriteLine($"args: {allargs}");
			// use this to allow command line parameters in the config
			var configuration = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();
			
			// --hosturl
			var hostUrl = configuration["hosturl"] as string;
			if (string.IsNullOrEmpty(hostUrl))
				hostUrl = hostUrl_Default;
			else 
				Console.WriteLine($"--hosturl {hostUrl ?? "-"}");
			
            app.HelpOption(inherited: true);
			
			if (allargs.Contains("hosturl")) {
				/*  TODO: read McMaster docs */

				app.Command("hosturl", url => {
				
					var arg1 = app.Argument("hosturl", "<host>");
					var hostUrl2 = arg1.Value;
					HostUrl = hostUrl2 ?? hostUrl;
					if (args.Length > 1 && HostUrl == hostUrl_Default && args[1].Contains("://"))
						HostUrl = args[1];
					else
						Console.WriteLine($"1 hosturl {HostUrl ?? "-"}");
					
					url.Command("hosturl", cmd => {

						Console.WriteLine($"> cmd:    {HostUrl}");
						var args2 = new string[] {};
						cmd.OnExecute(() =>
							CreateWebHostBuilder(args2).Build().Run());
				
						cmd.Execute(args2);
						app = null;
					});
				});
			}
			
			if (app == null)
				return;

            app.Command("import", import =>
            {
                import.Command("downloads", downloads =>
                {
                    downloads.OnExecute(async () =>
                    {
                        var provider = CreateHostBuilder(args).Build().Services;

                        await provider
                            .GetRequiredService<DownloadsImporter>()
                            .ImportAsync();
                    });
                });
            });

			if (HostUrl == null) {

				// var arg1 = app.Argument("hosturl", "hosturl argument.");
				// var hosturl2 = arg1.Value;
				// Console.WriteLine($"--hosturl (2): {hosturl2 ?? HostUrl}");
				HostUrl = hostUrl ?? hostUrl_Default;
				
				app.OnExecute(() =>
				{
					args = new string[] { "" };
					Console.WriteLine($"default hosturl: {HostUrl}");
					CreateWebHostBuilder(args).Build().Run();
				});
			}

            app.Execute(args);
        }
		
		// [Argument(1)]
	    // [Option(Description = "--hosturl <Host>")]
		public static string HostUrl { get; set; }

		[Option(Description = "--dev")]
		public static string Dev { get; set; }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
				.UseUrls(HostUrl)   // <!-- bind: 0.0.0.0 
                .UseKestrel(options =>
                {
                    // Remove the upload limit from Kestrel. If needed, an upload limit can
                    // be enforced by a reverse proxy server, like IIS.
                    options.Limits.MaxRequestBodySize = null;
                })
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var root = Environment.GetEnvironmentVariable("BAGET_CONFIG_ROOT");
                    if (!string.IsNullOrEmpty(root))
                        config.SetBasePath(root);
                });

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureBaGetConfiguration(args)
                .ConfigureBaGetServices()
                .ConfigureBaGetLogging();
        }
    }
}
