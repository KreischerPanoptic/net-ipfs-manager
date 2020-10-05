using Ipfs.Manager.Tools.Results.EntityResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results
{
    public interface IEntityResult
    {
        EntityDownloadStatus Status { get; }
        string Path { get; }
        string InternalPath { get; }
        string Comment { get; }
    }
}
