using Ipfs.Manager.Tools.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IUploadService
    {
        Task<Hypermedia.Hypermedia> UploadFileAsync(string path, string name);

        Hypermedia.Hypermedia UploadFile(string path, string name);

        Task<Hypermedia.Hypermedia> UploadFileAsync
        (
            string path,
            string name,
            string comment, 
            BlockSizeOptions options
        );

        Hypermedia.Hypermedia UploadFile
        (
            string path,
            string name,
            string comment,
            BlockSizeOptions options
        );

        Task<Hypermedia.Hypermedia> UploadDirectoryAsync(string path, string name);

        Hypermedia.Hypermedia UploadDirectory(string path, string name);

        Task<Hypermedia.Hypermedia> UploadDirectoryAsync
        (
            string path,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        );

        Hypermedia.Hypermedia UploadDirectory
        (
            string path,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        );

        Task<Hypermedia.Hypermedia> UploadEntitiesAsync(string[] paths, string name);

        Hypermedia.Hypermedia UploadEntities(string[] paths, string name);

        Task<Hypermedia.Hypermedia> UploadEntitiesAsync
        (
            string[] paths,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        );

        Hypermedia.Hypermedia UploadEntities
        (
            string[] paths,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        );
    }
}
