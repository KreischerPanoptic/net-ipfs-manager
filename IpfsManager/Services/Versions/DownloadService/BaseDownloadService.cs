using System;
using System.Collections.Generic;
using System.Text;

using Ipfs.Engine;
using Ipfs.CoreApi;

namespace Ipfs.Manager.Services.Versions.DownloadService
{
    class BaseDownloadService
    {
        private Manager _manager;

        public BaseDownloadService(Manager manager)
        {
            _manager = manager;
        }
    }
}
