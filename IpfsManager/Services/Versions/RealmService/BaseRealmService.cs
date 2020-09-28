using Ipfs.Manager.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

            if(hypermedia.Parent != null)
            {
                return false;
            }

            hypermedia.QueuePosition = index;
            hypermedia = SetAddedTime(hypermedia);
            _realm.Add<Models.Hypermedia>(hypermedia);

            return true;
        }

        private Models.Hypermedia SetAddedTime(Models.Hypermedia hypermedia)
        {
            hypermedia.Added = DateTimeOffset.Now;
            for(int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                hypermedia.Hypermedias[i] = SetAddedTime(hypermedia.Hypermedias[i]);
            }
            return hypermedia;
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

        public ThreadSafeReference.Object<Models.Hypermedia> ReadThreadSafeHypermedia(string path)
        {
            var result = _realm.Find<Models.Hypermedia>(path);
            return result is null ? null : ThreadSafeReference.Create<Models.Hypermedia>(result);
        }

        public List<ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeHypermedias()
        {
            var results = _realm.All<Models.Hypermedia>();
            var threads = new List<ThreadSafeReference.Object<Models.Hypermedia>>();
            foreach(var r in results)
            {
                threads.Add(r is null ? null : ThreadSafeReference.Create<Models.Hypermedia>(r));
            }
            return threads;
        }

        public async Task InitializeAsync(string path, ulong version)
        {
            RealmConfiguration configuration = new RealmConfiguration(path)
            {
                IsDynamic = true,
                SchemaVersion = version
            };
            _realm = await Realm.GetInstanceAsync(configuration);
            _isInitialized = true;
        }

        public async Task<bool> CreateHypermediaAsync(Models.Hypermedia hypermedia)
        {
            return await Task.Run(() =>
            {
                return CreateHypermedia(hypermedia);
            });
        }

        public async Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia)
        {
            return await Task.Run(() =>
            {
                return DeleteHypermedia(hypermedia);
            });
        }

        public async Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia, bool isFileDeletionRequested)
        {
            return await Task.Run(() =>
            {
                return DeleteHypermedia(hypermedia, isFileDeletionRequested);
            });
        }

        public async Task<List<ThreadSafeReference.Object<Models.Hypermedia>>> ReadThreadSafeHypermediasAsync()
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeHypermedias();
            });
        }

        public async Task<ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeHypermediaAsync(string path)
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeHypermedia(path);
            });
        }

        public ThreadSafeReference.Object<Models.Hypermedia> ReadThreadSafeInnerHypermedia(string outerPath, string innerPath)
        {
            var outerHypermedia = _realm.Find<Models.Hypermedia>(outerPath);
            Models.Hypermedia result = SearchForHypermedia(outerHypermedia, innerPath);
            return result is null ? null : ThreadSafeReference.Create<Models.Hypermedia>(result);
        }

        public async Task<ThreadSafeReference.Object<Models.Hypermedia>> ReadThreadSafeInnerHypermediaAsync(string outerPath, string innerPath)
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeInnerHypermedia(outerPath, innerPath);
            });
        }

        private Models.Hypermedia SearchForHypermedia(Models.Hypermedia hypermedia, string path)
        {
            foreach(var h in hypermedia.Hypermedias)
            {
                if(h.Path == path)
                {
                    return h;
                }
            }

            foreach(var h in hypermedia.Hypermedias)
            {
                return SearchForHypermedia(h, path);
            }

            return null;
        }

        public ThreadSafeReference.Object<Directory> ReadThreadSafeDirectory(string hypermediaPath, string directoryPath)
        {
            var outerHypermedia = _realm.Find<Models.Hypermedia>(hypermediaPath);
            Models.Directory result = SearchForDirectory(outerHypermedia, directoryPath);
            return result is null ? null : ThreadSafeReference.Create<Models.Directory>(result);
        }

        public async Task<ThreadSafeReference.Object<Directory>> ReadThreadSafeDirectoryAsync(string hypermediaPath, string directoryPath)
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeDirectory(hypermediaPath, directoryPath);
            });
        }

        private Models.Directory SearchForDirectory(Models.IEntity entity, string path)
        {
            if(entity is Models.Hypermedia)
            {
                foreach (var d in (entity as Models.Hypermedia).Directories)
                {
                    if (d.Path == path)
                    {
                        return d;
                    }
                }

                foreach (var d in (entity as Models.Hypermedia).Directories)
                {
                    return SearchForDirectory(d, path);
                }

                foreach (var h in (entity as Models.Hypermedia).Hypermedias)
                {
                    return SearchForDirectory(h, path);
                }
            }
            else if(entity is Models.Directory)
            {
                foreach (var d in (entity as Models.Directory).Directories)
                {
                    if (d.Path == path)
                    {
                        return d;
                    }
                }

                foreach (var d in (entity as Models.Directory).Directories)
                {
                    return SearchForDirectory(d, path);
                }
            }

            return null;
        }

        public ThreadSafeReference.Object<File> ReadThreadSafeFile(string hypermediaPath, string filePath)
        {
            var outerHypermedia = _realm.Find<Models.Hypermedia>(hypermediaPath);
            Models.File result = SearchForFile(outerHypermedia, filePath);
            return result is null ? null : ThreadSafeReference.Create<Models.File>(result);
        }

        public async Task<ThreadSafeReference.Object<File>> ReadThreadSafeFileAsync(string hypermediaPath, string filePath)
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeFile(hypermediaPath, filePath);
            });
        }

        private Models.File SearchForFile(Models.IEntity entity, string path)
        {
            if (entity is Models.Hypermedia)
            {
                foreach (var f in (entity as Models.Hypermedia).Files)
                {
                    if (f.Path == path)
                    {
                        return f;
                    }
                }

                foreach (var d in (entity as Models.Hypermedia).Directories)
                {
                    return SearchForFile(d, path);
                }

                foreach (var h in (entity as Models.Hypermedia).Hypermedias)
                {
                    return SearchForFile(h, path);
                }
            }
            else if (entity is Models.Directory)
            {
                foreach (var f in (entity as Models.Directory).Files)
                {
                    if (f.Path == path)
                    {
                        return f;
                    }
                }

                foreach (var d in (entity as Models.Directory).Directories)
                {
                    return SearchForFile(d, path);
                }
            }

            return null;
        }

        public ThreadSafeReference.Object<Block> ReadThreadSafeBlock(string hypermediaPath, string blockPath)
        {
            var outerHypermedia = _realm.Find<Models.Hypermedia>(hypermediaPath);
            Models.Block result = SearchForBlock(outerHypermedia, blockPath);
            return result is null ? null : ThreadSafeReference.Create<Models.Block>(result);
        }

        public async Task<ThreadSafeReference.Object<Block>> ReadThreadSafeBlockAsync(string hypermediaPath, string blockPath)
        {
            return await Task.Run(() =>
            {
                return ReadThreadSafeBlock(hypermediaPath, blockPath);
            });
        }

        private Models.Block SearchForBlock(Models.IEntity entity, string path)
        {
            if (entity is Models.Hypermedia)
            {
                foreach (var f in (entity as Models.Hypermedia).Files)
                {
                    return SearchForBlock(f, path);
                }

                foreach (var d in (entity as Models.Hypermedia).Directories)
                {
                    return SearchForBlock(d, path);
                }

                foreach (var h in (entity as Models.Hypermedia).Hypermedias)
                {
                    return SearchForBlock(h, path);
                }
            }
            else if (entity is Models.Directory)
            {
                foreach (var f in (entity as Models.Directory).Files)
                {
                    return SearchForBlock(f, path);
                }

                foreach (var d in (entity as Models.Directory).Directories)
                {
                    return SearchForBlock(d, path);
                }
            }
            else if (entity is Models.File)
            {
                if(!(entity as Models.File).IsSingleBlock)
                {
                    foreach(var b in (entity as Models.File).Blocks)
                    {
                        if(b.Path == path)
                        {
                            return b;
                        }
                    }
                }
            }

            return null;
        }
    }
}
