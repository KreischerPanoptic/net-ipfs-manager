using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IRealmService
    {
        void Initialize(string path, ulong version);

        Task InitializeAsync(string path, ulong version);

        bool IsRealmInitialized();

        Realms.RealmConfiguration GetRealmConfiguration();

        bool CreateHypermedia(Models.Hypermedia hypermedia);

        Task<bool> CreateHypermediaAsync(Models.Hypermedia hypermedia);

        bool DeleteHypermedia(Models.Hypermedia hypermedia);

        Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia);

        bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested);

        Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia, bool isFileDeletionRequested);

        List<Realms.ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeHypermedias();

        Task<List<Realms.ThreadSafeReference.Object<Models.Hypermedia>>> ReadThreadSafeHypermediasAsync();

        Realms.ThreadSafeReference.Object<Models.Hypermedia> ReadThreadSafeHypermedia(string path);

        Task<Realms.ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeHypermediaAsync(string path);

        Realms.ThreadSafeReference.Object<Models.Hypermedia> ReadThreadSafeInnerHypermedia(string outerPath, string innerPath);

        Task<Realms.ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeInnerHypermediaAsync(string outerPath, string innerPath);

        Realms.ThreadSafeReference.Object<Models.Directory> ReadThreadSafeDirectory(string hypermediaPath, string directoryPath);

        Task<Realms.ThreadSafeReference.Object<Models.Directory>> ReadThreadSafeDirectoryAsync(string hypermediaPath, string directoryPath);

        Realms.ThreadSafeReference.Object<Models.File> ReadThreadSafeFile(string hypermediaPath, string filePath);

        Task<Realms.ThreadSafeReference.Object<Models.File>> ReadThreadSafeFileAsync(string hypermediaPath, string filePath);

        Realms.ThreadSafeReference.Object<Models.Block> ReadThreadSafeBlock(string hypermediaPath, string blockPath);

        Task<Realms.ThreadSafeReference.Object<Models.Block>> ReadThreadSafeBlockAsync(string hypermediaPath, string blockPath);
    }
}
