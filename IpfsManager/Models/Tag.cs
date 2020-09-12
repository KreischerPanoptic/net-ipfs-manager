using Realms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public class Tag : RealmObject
    {
        public string Title { get; set; }
        public IList<Hypermedia> Hypermedias { get; }
    }
}
