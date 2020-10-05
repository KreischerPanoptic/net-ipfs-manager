using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results.EntityResults
{
    public class HypermediaResult : IEntityResult
    {
        private Models.Hypermedia _hypermedia;
        private EntityDownloadStatus _status;
        private string _comment;
        public EntityDownloadStatus Status { get => _status; }
        public string Path { get => _hypermedia.Path; }
        public string InternalPath { get => _hypermedia.InternalPath; }
        public string Comment { get => _comment; }
        public List<IEntityResult> Results { get; set; }

        public HypermediaResult(Models.Hypermedia hypermedia, EntityDownloadStatus status, string comment)
        {
            _hypermedia = hypermedia;
            _status = status;
            _comment = comment;
        }

        public Models.Hypermedia GetHypermedia()
        {
            return _hypermedia;
        }
    }
}
