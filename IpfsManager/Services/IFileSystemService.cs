using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IFileSystemService
    {
        void SaveHypermedia(Hypermedia.Hypermedia hypermedia, string path);

        void SaveHypermedia(Hypermedia.Hypermedia hypermedia, string path, string name);

        Task SaveHypermediaAsync(Hypermedia.Hypermedia hypermedia, string path);

        Task SaveHypermediaAsync(Hypermedia.Hypermedia hypermedia, string path, string name);

        Hypermedia.Hypermedia OpenHypermedia(string path);

        Task<Hypermedia.Hypermedia> OpenHypermediaAsync(string path);

        bool ReconstructFile(Models.File file);

        Task<bool> ReconstructFileAsync(Models.File file);

        bool CreateFileSystemModel(Models.Hypermedia hypermedia);

        bool CheckAndFixFileSystemModel(Models.Hypermedia hypermedia);

        Models.Hypermedia UpdateStatus(Models.Hypermedia hypermedia);

        Task<Models.Hypermedia> UpdateStatusAsync(Models.Hypermedia hypermedia);

        bool ClearTempFileSystem(Models.Hypermedia hypermedia);

        bool IsBlockValid(Models.Block block);

        Task<bool> IsBlockValidAsync(Models.Block block);

        bool IsFileValid(Models.File file);

        Task<bool> IsFileValidAsync(Models.File file);

        bool IsDirectoryValid(Models.Directory directory);

        Task<bool> IsDirectoryValidAsync(Models.Directory directory);

        bool IsHypermediaValid(Models.Hypermedia hypermedia);

        Task<bool> IsHypermediaValidAsync(Models.Hypermedia hypermedia);

        bool DeleteHypermedia(Models.Hypermedia hypermedia);

        bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested);
    }
}
