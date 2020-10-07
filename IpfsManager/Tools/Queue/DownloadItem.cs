using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace Ipfs.Manager.Tools.Queue
{
    public class DownloadItem
    {
        public ThreadSafeReference.Object<Models.Hypermedia> Item { get; set; }
        public DownloadItemStatus Status { get; set; }
    }
}
