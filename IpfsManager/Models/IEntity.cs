using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public interface IEntity
    {
        string Path { get; set; }
        string Name { get; set; }
        long Size { get; set; }
        string Hash { get; }
        IEntity Parent { get; set; }
    }
}
