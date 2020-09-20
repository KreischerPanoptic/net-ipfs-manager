using Realms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public class Directory : RealmObject, IEntity
    {
        [PrimaryKey]
        [Indexed]
        public string Path { get; set; }
        [Indexed]
        public string Name { get; set; }
        public int? AttributesRaw { get; set; }
        [Ignored]
        public System.IO.FileAttributes? Attributes
        {
            get
            {
                if (AttributesRaw is null)
                {
                    return null;
                }
                else
                {
                    return (System.IO.FileAttributes)AttributesRaw;
                }
            }
            set
            {
                if (value is null)
                {
                    AttributesRaw = null;
                }
                else
                {
                    AttributesRaw = (int)value;
                }
            }
        }
        public Progress<double> Progress { get; }
        public double ProgressRaw { get; set; }
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
        public DateTimeOffset? LastModifiedDateTime { get; set; }
        public IList<Directory> Directories { get; }
        public IList<File> Files { get; }
        [Indexed]
        public long Size { get; set; }
        [Indexed]
        public string Hash { get; set; }
        public Directory ParentDirectoryRaw { get; set; }
        public Hypermedia ParentHypermediaRaw { get; set; }
        [Ignored]
        public IEntity Parent
        {
            get
            {
                if (!(ParentDirectoryRaw is null))
                {
                    return ParentDirectoryRaw;
                }
                else if (!(ParentHypermediaRaw is null))
                {
                    return ParentHypermediaRaw;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value is null)
                {
                    ParentDirectoryRaw = null;
                    ParentHypermediaRaw = null;
                }
                else if (value is Directory)
                {
                    ParentDirectoryRaw = value as Directory;
                    ParentHypermediaRaw = null;
                }
                else if (value is Hypermedia)
                {
                    ParentHypermediaRaw = value as Hypermedia;
                    ParentDirectoryRaw = null;
                }
                else
                {
                    ParentDirectoryRaw = null;
                    ParentHypermediaRaw = null;
                }
            }
        }
        public long Index { get; set; }

        public Directory()
        {
            Progress = new Progress<double>(ChangeProgressRaw);
        }

        private void ChangeProgressRaw(double percent)
        {
            ProgressRaw = percent;
        }
    }
}
