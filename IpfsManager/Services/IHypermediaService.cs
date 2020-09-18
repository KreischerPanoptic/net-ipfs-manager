using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IHypermediaService
    {
        Hypermedia.Hypermedia GetOrTranslateHypermedia
        (
            string path,
            string name,
            string comment
        );

        Task<Hypermedia.Hypermedia> GetOrTranslateHypermediaAsync
        (
            string path,
            string name,
            string comment
        );

        Hypermedia.Hypermedia GetOrTranslateHypermedia
        (
            string path,
            string name,
            string comment,
            string extension
        );

        Task<Hypermedia.Hypermedia> GetOrTranslateHypermediaAsync
        (
            string path,
            string name,
            string comment,
            string extension
        );

        Hypermedia.Hypermedia GetHypermediaFromIPFSLink(string path);

        Task<Hypermedia.Hypermedia> GetHypermediaFromIPFSLinkAsync(string path);

        Hypermedia.Hypermedia TranslateRawIPFSToHypermedia
        (
           string path,
           string name,
           string comment
        );

        Task<Hypermedia.Hypermedia> TranslateRawIPFSToHypermediaAsync
        (
           string path,
           string name,
           string comment
        );

        Hypermedia.Hypermedia TranslateRawIPFSToHypermedia
        (
            string path,
            string name,
            string comment,
            string extension
        );

        Task<Hypermedia.Hypermedia> TranslateRawIPFSToHypermediaAsync
        (
            string path,
            string name,
            string comment,
            string extension
        );

        string[] TranslateHypermediaToIPFSLinks(Hypermedia.Hypermedia hypermedia);

        Task<string[]> TranslateHypermediaToIPFSLinksAsync(Hypermedia.Hypermedia hypermedia);

        string[] TranslateHypermediaToIPFSLinks(Hypermedia.Hypermedia hypermedia, bool isMetadataPreservationEnabled);

        Task<string[]> TranslateHypermediaToIPFSLinksAsync(Hypermedia.Hypermedia hypermedia, bool isMetadataPreservationEnabled);

        string TranslateHypermediaToIPFSLink(Hypermedia.Hypermedia hypermedia);

        Task<string> TranslateHypermediaToIPFSLinkAsync(Hypermedia.Hypermedia hypermedia);

        Hypermedia.Hypermedia CreateHypermediaWithPath(Hypermedia.Hypermedia hypermedia);

        Task<Hypermedia.Hypermedia> CreateHypermediaWithPathAsync(Hypermedia.Hypermedia hypermedia);

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

        Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(Hypermedia.Hypermedia outerHypermedia, string pathToInnerHypermedia);

        Hypermedia.Hypermedia ConstructWrappedHypermedia(Hypermedia.Hypermedia outerHypermedia, string pathToInnerHypermedia);

        Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(string pathToOuterHypermedia, string pathToInnerHypermedia);

        Hypermedia.Hypermedia ConstructWrappedHypermedia(string pathToOuterHypermedia, string pathToInnerHypermedia);

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
