using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results.EntityResults
{
    public class DirectoryResult : IEntityResult
    {
        private Models.Directory _directory;
        private EntityDownloadStatus _status;
        private string _comment;
        public EntityDownloadStatus Status { get => _status; }
        public string Path { get => _directory.Path; }
        public string InternalPath { get => _directory.InternalPath; }
        public string Comment { get => _comment; }
        public List<IEntityResult> Results { get; set; }

        public DirectoryResult(Models.Directory directory, EntityDownloadStatus status, string comment)
        {
            _directory = directory;
            _status = status;
            _comment = comment;
        }

        public Models.Directory GetDirectory()
        {
            return _directory;
        }
    }
}
