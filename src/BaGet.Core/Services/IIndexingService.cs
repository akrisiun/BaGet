using System;
using System.IO;
using System.Threading.Tasks;

namespace BaGet.Core.Services
{
    public enum IndexingResult
    {
        InvalidPackage,
        PackageAlreadyExists,
        Success,
    }

    public interface IIndexingService
    {
        Task<Tuple<string, IndexingResult>> IndexAsync(Stream stream);
    }
}