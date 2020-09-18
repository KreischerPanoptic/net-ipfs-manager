using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Manager.Services
{
    public interface IUploadService
    {
        //TODO rewrite
        Task<string> UploadFileInIPFSModeAsync(string path);
        string UploadFileInIPFSMode(string path);
        Task<string> UploadFileInIPFSModeAsync(string path, bool isWrappedWithFolder, bool isRecursiveAdd);
        string UploadFileInIPFSMode(string path, bool isWrappedWithFolder, bool isRecursiveAdd);
    }
}
