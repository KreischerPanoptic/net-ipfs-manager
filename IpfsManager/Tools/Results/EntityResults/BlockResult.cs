using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results.EntityResults
{
    public class BlockResult : IEntityResult
    {
        private Models.Block _block;
        private EntityDownloadStatus _status;
        private string _comment;
        public EntityDownloadStatus Status { get => _status; }
        public string Path { get => _block.Path; }
        public string InternalPath { get => _block.InternalPath; }
        public string Comment { get => _comment; }

        public BlockResult(Models.Block block, EntityDownloadStatus status, string comment)
        {
            _block = block;
            _status = status;
            _comment = comment;
        }

        public Models.Block GetBlock()
        {
            return _block;
        }
    }
}
