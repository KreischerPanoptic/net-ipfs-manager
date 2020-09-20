using Realms;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Models
{
    public class Hypermedia : RealmObject, IEntity, IHypemedia
    {
        [PrimaryKey]
        [Indexed]
        public string Path { get; set; }
        [Indexed]
        public string Name { get; set; }
        public string Comment { get; set; }
        public string EncodingRaw { get; set; } = Encoding.UTF8.WebName;
        [Ignored]
        public Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.GetEncoding(EncodingRaw);
            }
            set
            {
                EncodingRaw = value is null ? System.Text.Encoding.UTF8.WebName : value.WebName;
            }
        }
        public string CreatedBy { get; set; }
        public string CreatorPeer { get; set; }
        public bool IsAttributesPreservationEnabled { get; set; }
        public bool IsContinuousDownloadingEnabled { get; set; }
        public IList<Tag> Tags { get; }
        public Tag MainTag { get; set; }
        [Ignored]
        public Progress<double> Progress { get; }
        public double ProgressRaw { get; set; }
        public string InternalPath { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Added { get; set; }
        public DateTimeOffset? Completed { get; set; }
        public IList<Hypermedia> Hypermedias { get; }
        public IList<Directory> Directories { get; }
        public IList<File> Files { get; }
        public string Topic { get; set; }
        public string DefaultSubscriptionMessage { get; set; }
        public string DefaultSeedingMessage { get; set; }
        [Indexed]
        public long QueuePosition { get; set; }
        public int WrappingOptionsRaw { get; set; } = 0;
        [Ignored]
        public WrappingOptions WrappingOptions
        {
            get => (WrappingOptions)WrappingOptionsRaw;
            set => WrappingOptionsRaw = (int)value;
        }
        public int PriorityRaw { get; set; } = 1;
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
        public long Size { get; set; }
        [Indexed]
        public string Hash { get; set; }
        public Hypermedia ParentRaw { get; set; }
        [Ignored]
        public IEntity Parent
        {
            get => ParentRaw;
            set
            {
                if(value is Hypermedia)
                {
                    ParentRaw = value as Hypermedia;
                }
                else
                {
                    ParentRaw = null;
                }
            }
        }
        public string Version { get; set; }
        public long Index { get; set; }

        public Hypermedia()
        {
            Progress = new Progress<double>(ChangeProgressRaw);
        }

        private void ChangeProgressRaw(double percent)
        {
            ProgressRaw = percent;
        }
    }
}
