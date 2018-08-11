﻿using System.IO;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace BaGet.Core.Services
{
    public interface IPackageStorageService
    {
        Task SavePackageStreamAsync(PackageArchiveReader package, Stream packageStream);
        Task OverwritePackageStreamAsync(PackageArchiveReader package, Stream packageStream);

        Task<Stream> GetPackageStreamAsync(PackageIdentity package);
        Task<Stream> GetNuspecStreamAsync(PackageIdentity package);
        Task<Stream> GetReadmeStreamAsync(PackageIdentity package);

        Task DeleteAsync(PackageIdentity package);
    }
}
