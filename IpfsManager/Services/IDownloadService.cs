using Ipfs.Manager.Tools.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IDownloadService
    {
        bool DownloadHypermedia(Models.Hypermedia hypermedia);

        Task<bool> DownloadHypermediaAsync(Models.Hypermedia hypermedia);

        bool DownloadHypermedia(Hypermedia.Hypermedia hypermedia, string downloadPath);

        Task<bool> DownloadHypermediaAsync(Hypermedia.Hypermedia hypermedia,string downloadPath);

        bool DownloadHypermedia
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            Models.WrappingOptions wrappingOptions
        );

        Task<bool> DownloadHypermediaAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            Models.WrappingOptions wrappingOptions
        );

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
