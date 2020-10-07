using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Queue
{
    public enum DownloadItemStatus
    {
        Launched,
        Prepared,
        Waiting,
        Ignored
    }
}
