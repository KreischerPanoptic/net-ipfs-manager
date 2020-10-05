using Ipfs.Manager.Tools.Results.EntityResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools.Results
{
    public class Result
    {
        public ResultStatus Status { get; set; }
        public string Comment { get; set; }
        public HypermediaResult Hypermedia { get; set; }
        public List<IEntityResult> CompletedEntities { get; set; }
        public List<IEntityResult> CorruptedEntities { get; set; }
    }
}
