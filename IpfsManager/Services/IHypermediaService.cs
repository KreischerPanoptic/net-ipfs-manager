using Ipfs.Manager.Tools.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IHypermediaService
    {
        Hypermedia.Hypermedia GetHypermediaFromIPFSLink(string path);

        Task<Hypermedia.Hypermedia> GetHypermediaFromIPFSLinkAsync(string path);

        string TranslateHypermediaToIPFSLink(Hypermedia.Hypermedia hypermedia);

        Task<string> TranslateHypermediaToIPFSLinkAsync(Hypermedia.Hypermedia hypermedia);

        Hypermedia.Hypermedia CreateHypermediaWithPath(Hypermedia.Hypermedia hypermedia);

        Task<Hypermedia.Hypermedia> CreateHypermediaWithPathAsync(Hypermedia.Hypermedia hypermedia);

        Hypermedia.Hypermedia CreateHypermediaWithPath(Hypermedia.Hypermedia hypermedia, BlockSizeOptions options, string key);

        Task<Hypermedia.Hypermedia> CreateHypermediaWithPathAsync(Hypermedia.Hypermedia hypermedia, BlockSizeOptions options, string key);

        /// <summary>
        /// Function to created hypermedia with linked inside hypermedias asynchronously.
        /// </summary>
        /// <param name="hypermediaChain">
        /// First element in list is outer hypermedia for second, second is inner in outer first and outer for third and so on...
        /// </param>
        /// <returns>Constructed hypermedia with inner hypermedias</returns>
        Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(List<Hypermedia.Hypermedia> hypermediaChain);
        /// <summary>
        /// Function to created hypermedia with linked inside hypermedias
        /// </summary>
        /// <param name="hypermediaChain">
        /// First element in list is outer hypermedia for second, second is inner in outer first and outer for third and so on...
        /// </param>
        /// <returns></returns>
        Hypermedia.Hypermedia ConstructWrappedHypermedia(List<Hypermedia.Hypermedia> hypermediaChain);

        Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(Hypermedia.Hypermedia outerHypermedia, Hypermedia.Hypermedia innerHypermedia);

        Hypermedia.Hypermedia ConstructWrappedHypermedia(Hypermedia.Hypermedia outerHypermedia, Hypermedia.Hypermedia innerHypermedia);

        Task<Models.Hypermedia> ConvertToRealmModelAsync(Hypermedia.Hypermedia hypermedia, string downloadPath, long queuePosition);

        Models.Hypermedia ConvertToRealmModel(Hypermedia.Hypermedia hypermedia, string downloadPath, long queuePosition);

        Task<Models.Hypermedia> ConvertToRealmModelAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            long queuePosition,
            Models.WrappingOptions wrappingOptions,
            Models.Priority priority,
            Models.Status status
        );

        Models.Hypermedia ConvertToRealmModel
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            long queuePosition,
            Models.WrappingOptions wrappingOptions,
            Models.Priority priority,
            Models.Status status
        );
    }
}
