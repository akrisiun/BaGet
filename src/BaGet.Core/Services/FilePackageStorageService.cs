using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaGet.Core.Entities;
using BaGet.Core.Extensions;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace BaGet.Core.Services
{
    public class FilePackageStorageService : IPackageStorageService
    {
        // See: https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/Stream.cs#L35
        private const int DefaultCopyBufferSize = 81920;

        private readonly string _storePath;

        public FilePackageStorageService(string storePath)
        {
            _storePath = storePath ?? throw new ArgumentNullException(nameof(storePath));
        }

        public async Task OverwritePackageStreamAsync(PackageArchiveReader package, Stream packageStream)
        {
            var identity = package.GetIdentity();
            var packagePath = Path.Combine(_storePath, PackagePath(identity));
            if (File.Exists(packagePath)) {
                File.Delete(packagePath);
            }
            await SavePackageStreamAsync(package, packageStream);
        }

        //public async Task SavePackageContentAsync(Package package,
        //    Stream packageStream,
        //    Stream nuspecStream,
        //    Stream readmeStream,
        //    CancellationToken cancellationToken)

        public async Task SavePackageStreamAsync(PackageArchiveReader package, Stream packageStream)
        {
            var identity = package.GetIdentity();
            var packagePath = Path.Combine(_storePath, PackagePath(identity));
            var nuspecPath = Path.Combine(_storePath, NuspecPath(identity));
            var readmePath = Path.Combine(_storePath, ReadmePath(identity));

            EnsurePathExists(identity);

            // TODO: Catch IOException and test if File.Exists. If false, rethrow exception.
            using (var fileStream = File.Open(packagePath, FileMode.CreateNew))
            {
                packageStream.Seek(0, SeekOrigin.Begin);

                await packageStream.CopyToAsync(fileStream);
            }

            using (var nuspec = package.GetNuspec())
            using (var fileStream = File.Open(nuspecPath, FileMode.OpenOrCreate))
            {
                await nuspec.CopyToAsync(fileStream);
            }

            // GetReadme
            using (Stream readme = await package.GetReadmeAsync(CancellationToken.None))
            using (var fileStream = File.Open(readmePath, FileMode.OpenOrCreate))
            {
                await readme.CopyToAsync(fileStream);
            }
        }

        public Task<Stream> GetPackageStreamAsync(string id, NuGetVersion version) => Task.FromResult(GetPackageStream(new PackageIdentity(id, version)));
        public Task<Stream> GetNuspecStreamAsync(string id, NuGetVersion version) => Task.FromResult(GetNuspecStream(new PackageIdentity(id, version)));
        public Task<Stream> GetReadmeStreamAsync(string id, NuGetVersion version) => Task.FromResult(GetReadmeStream(new PackageIdentity(id, version)));

        public Task<Stream> GetPackageStreamAsync(PackageIdentity package) => Task.FromResult(GetPackageStream(package));
        public Task<Stream> GetNuspecStreamAsync(PackageIdentity package) => Task.FromResult(GetNuspecStream(package));
        public Task<Stream> GetReadmeStreamAsync(PackageIdentity package) => Task.FromResult(GetReadmeStream(package));

        private Stream GetPackageStream(PackageIdentity package) => GetFileStream(PackagePath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString()));
        private Stream GetNuspecStream(PackageIdentity package) => GetFileStream(NuspecPath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString()));
        private Stream GetReadmeStream(PackageIdentity package) => GetFileStream(ReadmePath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString()));

        private Stream GetFileStream(string path)
        {
            path = Path.Combine(_storePath, path);

            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public string PackagePath(PackageIdentity package) => PackagePath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString());
        private string PackagePath(string lowercasedId, string lowercasedNormalizedVersion)
        {
            return Path.Combine(
                _storePath,
                lowercasedId,
                lowercasedNormalizedVersion,
                $"{lowercasedId}.{lowercasedNormalizedVersion}.nupkg");
        }

        public string NuspecPath(PackageIdentity package) => NuspecPath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString());
        private string NuspecPath(string lowercasedId, string lowercasedNormalizedVersion)
        {
            return Path.Combine(
                _storePath,
                lowercasedId,
                lowercasedNormalizedVersion,
                $"{lowercasedId}.nuspec");
        }

        public string ReadmePath(PackageIdentity package) => ReadmePath(package.Id.ToLowerInvariant(), package.Version.ToNormalizedString());
        private string ReadmePath(string lowercasedId, string lowercasedNormalizedVersion)
        {
            return Path.Combine(
                _storePath,
                lowercasedId,
                lowercasedNormalizedVersion,
                "readme");
        }

        public Task DeleteAsync(string id, NuGetVersion version) // PackageIdentity package)
        {
            throw new NotImplementedException();
        }

        private void EnsurePathExists(PackageIdentity package)
        {
            var id = package.Id.ToLowerInvariant();
            var version = package.Version.ToNormalizedString().ToLowerInvariant();
            var path = Path.Combine(_storePath, id, version);

            Directory.CreateDirectory(path);
        }
    }
}
