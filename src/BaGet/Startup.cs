using System;
using BaGet.Configurations;
using BaGet.Core.Entities;
using BaGet.Extensions;
using BaGet.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaGet
{
    public class Startup
    {
        static Startup() {
            Args = Environment.GetCommandLineArgs();
            ServerUrls = Environment.GetEnvironmentVariable("server_urls");
            // "server.urls": "http://localhost:5000/"
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }
        // --server.urls=http://localhost:5000/
        public static string[] Args {get; set;}
        public static string ServerUrls {get; set;}

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureBaGet(Configuration, httpServices: true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();

                // Run migrations automatically in development mode.
                var scopeFactory = app.ApplicationServices
                    .GetRequiredService<IServiceScopeFactory>();

                using (var scope = scopeFactory.CreateScope())
                {
                    scope.ServiceProvider
                        .GetRequiredService<IContext>()
                        .Database
                        .Migrate();
                }
            }

            app.UseForwardedHeaders();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors(ConfigureCorsOptions.CorsPolicy);

            app.UseMvc(routes =>
            {
                routes
                    .MapServiceIndexRoutes()
                    .MapPackagePublishRoutes()
                    .MapSearchRoutes()
                    .MapRegistrationRoutes()
                    .MapPackageContentRoutes();
            });

            // --server.urls=http://localhost:5001/
            if (string.IsNullOrWhiteSpace(ServerUrls)) {
                ServerUrls = "http://localhost:8050/";
            }

            Console.WriteLine($"{ServerUrls}v3/index.json");
            if ((Args?.Length ?? 0) > 0)
                Console.WriteLine($"Args {String.Join(" ", Args)}");
            Console.WriteLine($"BaGet {env.EnvironmentName} / {env.ToString()} : index here");
        }
    }
}
