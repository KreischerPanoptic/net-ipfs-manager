using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results.EntityResults
{
    public class FileResult : IEntityResult
    {
        private Models.File _file;
        private EntityDownloadStatus _status;
        private string _comment;
        public EntityDownloadStatus Status { get => _status; }
        public string Path { get => _file.Path; }
        public string InternalPath { get => _file.InternalPath; }
        public string Comment { get => _comment; }
        public List<BlockResult> BlockResults { get; set; }

        public FileResult(Models.File file, EntityDownloadStatus status, string comment)
        {
            _file = file;
            _status = status;
            _comment = comment;
        }

        public Models.File GetFile()
        {
            return _file;
        }
    }
}
