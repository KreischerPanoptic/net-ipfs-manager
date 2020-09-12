using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public enum Status
    {
        Connecting,
        Stopped,
        ReadyForDownload,
        Downloading,
        ReadyForReconstruction,
        Seeding,
        Completed,
        Checking,
        Error
    }
}
