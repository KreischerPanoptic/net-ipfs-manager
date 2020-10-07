using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Queue
{
    public class DownloadQueue
    {
        public Dictionary<int, DownloadItem> Items { get; set; }
        public DownloadQueueStatus Status { get; set; }
    }
}
