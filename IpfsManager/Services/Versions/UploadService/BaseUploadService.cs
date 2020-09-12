using System;
using System.Collections.Generic;
using System.Text;

using Ipfs.Engine;
using Ipfs.CoreApi;

namespace Ipfs.Manager.Services.Versions.UploadService
{
    class BaseUploadService
    {
        private Manager _manager;

        public BaseUploadService(Manager manager)
        {
            _manager = manager;
        }


    }
}
