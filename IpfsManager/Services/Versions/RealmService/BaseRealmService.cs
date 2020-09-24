using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Services.Versions.RealmService
{
    class BaseRealmService : IRealmService
    {
        private Manager _manager;
        private Realm _realm;
        private bool _isInitialized = false;

        public BaseRealmService(Manager manager)
        {
            _manager = manager;
        }

        public bool CreateHypermedia(Models.Hypermedia hypermedia)
        {
            if (_realm is null)
            {
                throw new NullReferenceException("Realm is not initialized.");
            }

            var entities = _realm.All<Models.Hypermedia>();
            foreach(var e in entities)
            {
                if(e.Path == hypermedia.Path || e.Hash == hypermedia.Hash)
                {
                    return false;
                }
            }

            long index = long.MinValue;
            foreach(var e in entities)
            {
                if(e.QueuePosition > index)
                {
                    index = e.QueuePosition;
                }
            }

            if(index == long.MinValue)
            {
                index = 1;
            }
            else
            {
                ++index;
            }

            hypermedia.QueuePosition = index;
            _realm.Add<Models.Hypermedia>(hypermedia);

            return true;
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia)
        {
            return DeleteHypermedia(hypermedia, false);
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested)
        {
            bool isDeletionSuccessful = _manager.FileSystemService.DeleteHypermedia(hypermedia, isFileDeletionRequested);
            if (isDeletionSuccessful)
            {
                _realm.Remove(hypermedia);
            }
            return isDeletionSuccessful;
        }

        public RealmConfiguration GetRealmConfiguration()
        {
            if(_realm is null || !_isInitialized)
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            return _realm.Config as RealmConfiguration;
        }

        public void Initialize(string path, ulong version)
        {
            RealmConfiguration configuration = new RealmConfiguration(path)
            {
                IsDynamic = true,
                SchemaVersion = version
            };
            _realm = Realm.GetInstance(configuration);
            _isInitialized = true;
        }

        public bool IsRealmInitialized()
        {
            return _isInitialized;
        }

        public ThreadSafeReference ReadThreadSafeHypermedia(string path)
        {
            var result = _realm.Find<Models.Hypermedia>(path);
            return result is null ? null : ThreadSafeReference.Create<Models.Hypermedia>(result);
        }

        public List<ThreadSafeReference> ReadThreadSafeHypermedias()
        {
            var results = _realm.All<Models.Hypermedia>();
            var threads = new List<ThreadSafeReference>();
            foreach(var r in results)
            {
                threads.Add(r is null ? null : ThreadSafeReference.Create<Models.Hypermedia>(r));
            }
            return threads;
        }
    }
}
