using Realms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public class Block : RealmObject
    {
        [PrimaryKey]
        [Indexed]
        public string Key { get; set; }
        [Indexed]
        public long Size { get; set; }
        [Indexed]
        public string InternalPath { get; set; }
        public int PriorityRaw { get; set; }
        [Ignored]
        public Priority Priority
        {
            get => (Priority)PriorityRaw;
            set => PriorityRaw = (int)value;
        }
        public int StatusRaw { get; set; }
        [Ignored]
        public Status Status
        {
            get => (Status)StatusRaw;
            set => StatusRaw = (int)value;
        }
        [Indexed]
        public string Hash { get; set; }
        public File Parent { get; set; }
    }
}
