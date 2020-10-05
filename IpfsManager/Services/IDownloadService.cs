using Ipfs.Manager.Tools.Options;
using Ipfs.Manager.Tools.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IDownloadService
    {
        Result DownloadHypermedia(Models.Hypermedia hypermedia);

        Task<Result> DownloadHypermediaAsync(Models.Hypermedia hypermedia);

        Result DownloadHypermedia(Hypermedia.Hypermedia hypermedia, string downloadPath);

        Task<Result> DownloadHypermediaAsync(Hypermedia.Hypermedia hypermedia,string downloadPath);

        Result DownloadHypermedia
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            Models.WrappingOptions wrappingOptions
        );

        Task<Result> DownloadHypermediaAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            Models.WrappingOptions wrappingOptions
        );

        /*Result ReloadHypermedia(Result result);

        Task<Result> ReloadHypermediaAsync(Result result);*/

        bool ClearCorruptedData(Result result);

        bool ClearAllData(Result result);

        Task<bool> ClearAllDataAsync(Result result);

        bool StartDownloadService();

        bool StopDownloadService();

        bool StopHypermediaDownloading(string path);

        bool StartHypermediaDownloading(string path);

        bool StopAllDownloads();

        bool StartAllDownloads();

        void CheckHypermedia(string path);

        Task CheckHypermediaAsync(string path);

        void CheckAllHypermedias();

        Task CheckAllHypermediasAsync();

        bool ChangeQueuePosition(string path, QueueDirectionOptions direction);

        Task<bool> ChangeQueuePositionAsync(string path, QueueDirectionOptions direction);

        bool ChangeHypermediaPriority(string path, Models.Priority priority);

        Task<bool> ChangeHypermediaPriorityAsync(string path, Models.Priority priority);

        bool ChangeInnerHypermediaPriority(string outerPath, string innerPath, Models.Priority priority);

        Task<bool> ChangeInnerHypermediaPriorityAsync(string outerPath, string innerPath, Models.Priority priority);

        bool ChangeDirectoryPriority(string hypermediaPath, string directoryPath, Models.Priority priority);

        Task<bool> ChangeDirectoryPriorityAsync(string hypermediaPath, string directoryPath, Models.Priority priority);

        bool ChangeFilePriority(string hypermediaPath, string filePath, Models.Priority priority);

        Task<bool> ChangeFilePriorityAsync(string hypermediaPath, string filePath, Models.Priority priority);

        bool ChangeBlockPriority(string hypermediaPath, string blockPath, Models.Priority priority);

        Task<bool> ChangeBlockPriorityAsync(string hypermediaPath, string blockPath, Models.Priority priority);
    }
}
