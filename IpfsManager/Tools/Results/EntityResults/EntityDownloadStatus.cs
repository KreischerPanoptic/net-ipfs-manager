using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results.EntityResults
{
    public enum EntityDownloadStatus
    {
        Completed,
        UnknownError,
        FileSystemError,
        TimeoutError,
        EntitySearchError
    }
}
