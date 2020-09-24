using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Services
{
    public interface IRealmService
    {
        void Initialize(string path, ulong version);
        bool IsRealmInitialized();
        Realms.RealmConfiguration GetRealmConfiguration();
        bool CreateHypermedia(Models.Hypermedia hypermedia);
        bool DeleteHypermedia(Models.Hypermedia hypermedia);
        bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested);
        Realms.ThreadSafeReference ReadThreadSafeHypermedia(string path);
        List<Realms.ThreadSafeReference> ReadThreadSafeHypermedias();
    }
}
