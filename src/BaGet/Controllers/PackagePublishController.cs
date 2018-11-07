using System;
using System.IO;
using System.Threading.Tasks;
using BaGet.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace BaGet.Controllers
{
    public class PackagePublishController : Controller
    {
        public const string ApiKeyHeader = "X-NuGet-ApiKey";

        private readonly IAuthenticationService _authentication;
        private readonly IIndexingService _indexer;
        private readonly IPackageService _packages;
        private readonly ILogger<PackagePublishController> _logger;

        public PackagePublishController(
            IAuthenticationService authentication,
            IIndexingService indexer,
            IPackageService packages,
            ILogger<PackagePublishController> logger)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _indexer = indexer ?? throw new ArgumentNullException(nameof(indexer));
            _packages = packages ?? throw new ArgumentNullException(nameof(packages));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // See: https://docs.microsoft.com/en-us/nuget/api/package-publish-resource#push-a-package
        public async Task Upload(IFormFile package)
        {
            if (package == null)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            if (!await _authentication.AuthenticateAsync(ApiKey))
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            Stream uploadStream = null;
            MemoryStream sourceStream = null;
            try
            {
                sourceStream = new MemoryStream();
                {
                    // await dataSource.LoadIntoStream(sourceStream);
                    uploadStream = package.OpenReadStream();
                    uploadStream.CopyTo(sourceStream);
                    sourceStream.Position = 0;

                    Tuple<string, IndexingResult> r = await _indexer.IndexAsync(sourceStream); // uploadStream);
                    var result = r.Item2;
                    var version = r.Item1;

                    switch (result)
                    {
                        case IndexingResult.InvalidPackage:
                            HttpContext.Response.StatusCode = 400;
                            break;

                        case IndexingResult.PackageAlreadyExists:

                            Console.WriteLine($"Delete {package.Name} v{version}");
                            await Delete(package.Name, version);
                            // retry:
                            Console.WriteLine($"Put Retry  {package.Name} v{version}");

                            sourceStream.Dispose();
                            sourceStream = null;
                            using (var sourceStream2 = new MemoryStream())
                            using (var uploadStream2 = package.OpenReadStream())
                            {
                                uploadStream2.CopyTo(sourceStream2);
                                sourceStream2.Position = 0;
                                Tuple<string, IndexingResult> r2 =
                                      await (_indexer as IndexingService).IndexAsync(sourceStream2, true);

                                if (r2.Item2 == IndexingResult.PackageAlreadyExists) {
                                   HttpContext.Response.StatusCode = 409;
                                } else if(r2.Item2 == IndexingResult.Success) {
                                   HttpContext.Response.StatusCode = 201;
                                }
                            }
                            break;

                        case IndexingResult.Success:
                            HttpContext.Response.StatusCode = 201;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown during package upload");

                HttpContext.Response.StatusCode = 500;
            }
            finally {
                sourceStream?.Dispose();
                uploadStream?.Dispose();
                uploadStream = null;
            }
        }

        public async Task<IActionResult> Delete(string id, string version)
        {
            if (!NuGetVersion.TryParse(version, out var nugetVersion))
            {
                return NotFound();
            }

            if (!await _authentication.AuthenticateAsync(ApiKey))
            {
                return Unauthorized();
            }

            if (await _packages.UnlistPackageAsync(id, nugetVersion))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Relist(string id, string version)
        {
            if (!NuGetVersion.TryParse(version, out var nugetVersion))
            {
                return NotFound();
            }

            if (!await _authentication.AuthenticateAsync(ApiKey))
            {
                return Unauthorized();
            }

            if (await _packages.RelistPackageAsync(id, nugetVersion))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private string ApiKey => Request.Headers[ApiKeyHeader];
    }
}
