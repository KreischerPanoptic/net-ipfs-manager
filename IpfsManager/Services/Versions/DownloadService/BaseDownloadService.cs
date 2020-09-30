using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Ipfs.Engine;
using Ipfs.CoreApi;
using Realms;
using System.Threading.Tasks;
using Ipfs.Manager.Tools.Options;
using Ipfs.Manager.Models;
using System.Threading;
using System.Diagnostics;

namespace Ipfs.Manager.Services.Versions.DownloadService
{
    class BaseDownloadService : IDownloadService
    {
        private Manager _manager;

        public BaseDownloadService(Manager manager)
        {
            _manager = manager;
        }

        public bool ChangeBlockPriority(string hypermediaPath, string blockPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeBlock(hypermediaPath, blockPath);
            Models.Block block = currentRealm.ResolveReference<Models.Block>(threadSafeReference);
            if (block != null)
            {
                currentRealm.Write(() =>
                {
                    block.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeBlockPriorityAsync(string hypermediaPath, string blockPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeBlockAsync(hypermediaPath, blockPath);
            Models.Block block = currentRealm.ResolveReference<Models.Block>(threadSafeReference);
            if (block != null)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    block.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public bool ChangeDirectoryPriority(string hypermediaPath, string directoryPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeDirectory(hypermediaPath, directoryPath);
            Models.Directory directory = currentRealm.ResolveReference<Models.Directory>(threadSafeReference);
            if (directory != null)
            {
                currentRealm.Write(() =>
                {
                    directory.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeDirectoryPriorityAsync(string hypermediaPath, string directoryPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeDirectoryAsync(hypermediaPath, directoryPath);
            Models.Directory directory = currentRealm.ResolveReference<Models.Directory>(threadSafeReference);
            if (directory != null)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    directory.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public bool ChangeFilePriority(string hypermediaPath, string filePath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeFile(hypermediaPath, filePath);
            Models.File file = currentRealm.ResolveReference<Models.File>(threadSafeReference);
            if (file != null)
            {
                currentRealm.Write(() =>
                {
                    file.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeFilePriorityAsync(string hypermediaPath, string filePath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeFileAsync(hypermediaPath, filePath);
            Models.File file = currentRealm.ResolveReference<Models.File>(threadSafeReference);
            if (file != null)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    file.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public bool ChangeHypermediaPriority(string path, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeHypermedia(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if (hypermedia != null)
            {
                currentRealm.Write(() =>
                {
                    hypermedia.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeHypermediaPriorityAsync(string path, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeHypermediaAsync(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if (hypermedia != null)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    hypermedia.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public bool ChangeInnerHypermediaPriority(string outerPath, string innerPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeInnerHypermedia(outerPath, innerPath);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if (hypermedia != null)
            {
                currentRealm.Write(() =>
                {
                    hypermedia.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeInnerHypermediaPriorityAsync(string outerPath, string innerPath, Priority priority)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeInnerHypermediaAsync(outerPath, innerPath);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if (hypermedia != null)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    hypermedia.Priority = priority;
                });
                return true;
            }
            return false;
        }

        public bool ChangeQueuePosition(string path, QueueDirectionOptions direction)
        {
            if(!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeHypermedia(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if(hypermedia != null)
            {
                long queue = hypermedia.QueuePosition;

                var threadSafeReferences = _manager.RealmService.ReadThreadSafeHypermedias();
                List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
                foreach (var t in threadSafeReferences)
                {
                    hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
                }

                hypermedias = hypermedias.OrderBy(x => x.QueuePosition).ToList();
                int index = hypermedias.IndexOf(hypermedia);
                long maxIndex = hypermedias.Last().QueuePosition;

                switch(direction)
                {
                    case QueueDirectionOptions.Up:
                        if(index > 1)
                        {
                            currentRealm.Write(() =>
                            {
                                hypermedias[index - 1].QueuePosition = queue;
                                hypermedia.QueuePosition = queue - 1;
                            });
                        }
                        break;
                    case QueueDirectionOptions.Down:
                        if (index < maxIndex)
                        {
                            currentRealm.Write(() =>
                            {
                                hypermedias[index + 1].QueuePosition = queue;
                                hypermedia.QueuePosition = queue + 1;
                            });
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeQueuePositionAsync(string path, QueueDirectionOptions direction)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeHypermediaAsync(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            if (hypermedia != null)
            {
                long queue = hypermedia.QueuePosition;

                var threadSafeReferences = await _manager.RealmService.ReadThreadSafeHypermediasAsync();
                List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
                foreach (var t in threadSafeReferences)
                {
                    hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
                }

                hypermedias = hypermedias.OrderBy(x => x.QueuePosition).ToList();
                int index = hypermedias.IndexOf(hypermedia);
                long maxIndex = hypermedias.Last().QueuePosition;

                switch (direction)
                {
                    case QueueDirectionOptions.Up:
                        if (index > 1)
                        {
                            await currentRealm.WriteAsync(tempRealm =>
                            {
                                hypermedias[index - 1].QueuePosition = queue;
                                hypermedia.QueuePosition = queue - 1;
                            });
                        }
                        break;
                    case QueueDirectionOptions.Down:
                        if (index < maxIndex)
                        {
                            await currentRealm.WriteAsync(tempRealm =>
                            {
                                hypermedias[index + 1].QueuePosition = queue;
                                hypermedia.QueuePosition = queue + 1;
                            });
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        public void CheckAllHypermedias()
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReferences = _manager.RealmService.ReadThreadSafeHypermedias();
            List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
            List<Tuple<Models.Hypermedia, Status>> prepared = new List<Tuple<Models.Hypermedia, Status>>();
            foreach(var t in threadSafeReferences)
            {
                hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
            }
            foreach (var h in hypermedias)
            {
                prepared.Add(new Tuple<Models.Hypermedia, Status>(h, h.Status));
                currentRealm.Write(() =>
                {
                    h.Status = Models.Status.Checking;
                });
            }
            foreach (var h in prepared)
            {
                var isFixed = _manager.FileSystemService.CheckAndFixFileSystemModel(h.Item1);
                if (!isFixed)
                {
                    currentRealm.Write(() =>
                    {
                        h.Item1.Status = Models.Status.Error;
                    });
                }
                else
                {
                    var updated = _manager.FileSystemService.UpdateStatus(h.Item1, h.Item2);
                    currentRealm.Write(() =>
                    {
                        h.Item1.Status = updated.Status;
                    });
                }
            }
        }

        public async Task CheckAllHypermediasAsync()
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReferences = await _manager.RealmService.ReadThreadSafeHypermediasAsync();
            List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
            List<Tuple<Models.Hypermedia, Status>> prepared = new List<Tuple<Models.Hypermedia, Status>>();
            foreach (var t in threadSafeReferences)
            {
                hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
            }
            foreach (var h in hypermedias)
            {
                prepared.Add(new Tuple<Models.Hypermedia, Status>(h, h.Status));
                await currentRealm.WriteAsync(tempRealm =>
                {
                    h.Status = Models.Status.Checking;
                });
            }
            foreach (var h in prepared)
            {
                var isFixed = _manager.FileSystemService.CheckAndFixFileSystemModel(h.Item1);
                if (!isFixed)
                {
                    await currentRealm.WriteAsync(tempRealm =>
                    {
                        h.Item1.Status = Models.Status.Error;
                    });
                }
                else
                {
                    var updated = await _manager.FileSystemService.UpdateStatusAsync(h.Item1, h.Item2);
                    await currentRealm.WriteAsync(tempRealm =>
                    {
                        h.Item1.Status = updated.Status;
                    });
                }
            }
        }

        public void CheckHypermedia(string path)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeHypermedia(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            Status origin = hypermedia.Status;

            currentRealm.Write(() =>
            {
                hypermedia.Status = Models.Status.Checking;
            });

            var isFixed = _manager.FileSystemService.CheckAndFixFileSystemModel(hypermedia);
            if (!isFixed)
            {
                currentRealm.Write(() =>
                {
                    hypermedia.Status = Models.Status.Error;
                });
            }
            else
            {
                var updated = _manager.FileSystemService.UpdateStatus(hypermedia, origin);
                currentRealm.Write(() =>
                {
                    hypermedia.Status = updated.Status;
                });
            }
        }

        public async Task CheckHypermediaAsync(string path)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = await _manager.RealmService.ReadThreadSafeHypermediaAsync(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);
            Status origin = hypermedia.Status;

            await currentRealm.WriteAsync(tempRealm =>
            {
                hypermedia.Status = Models.Status.Checking;
            });

            var isFixed = _manager.FileSystemService.CheckAndFixFileSystemModel(hypermedia);
            if (!isFixed)
            {
                await currentRealm.WriteAsync(tempRealm =>
                {
                    hypermedia.Status = Models.Status.Error;
                });
            }
            else
            {
                var updated = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, origin);
                await currentRealm.WriteAsync(tempRealm =>
                {
                    hypermedia.Status = updated.Status;
                });
            }
        }

        public bool DownloadHypermedia(Models.Hypermedia hypermedia)
        {
            _manager.FileSystemService.CreateFileSystemModel(hypermedia);
            hypermedia = DownloadRawHypermedia(hypermedia);
            if(hypermedia.Status == Status.Seeding)
            {
                return _manager.FileSystemService.ClearTempFileSystem(hypermedia);
            }
            else
            {
                try
                {
                    _manager.FileSystemService.DeleteHypermedia(hypermedia, true);
                }
                catch { };
                return false;
            }
        }

        public bool DownloadHypermedia(Hypermedia.Hypermedia hypermedia, string downloadPath)
        {
            var converted = _manager.HypermediaService.ConvertToRealmModel
            (
                hypermedia,
                downloadPath,
                long.MinValue
            );

            return DownloadHypermedia(converted);
        }

        public bool DownloadHypermedia
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            WrappingOptions wrappingOptions
        )
        {
            var converted = _manager.HypermediaService.ConvertToRealmModel
            (
                hypermedia,
                downloadPath,
                isAttributesPreservationEnabled,
                isContinuousDownloadingEnabled,
                long.MinValue,
                wrappingOptions,
                Priority.High,
                Status.Downloading
            );

            return DownloadHypermedia(converted);
        }

        public async Task<bool> DownloadHypermediaAsync(Models.Hypermedia hypermedia)
        {
            _manager.FileSystemService.CreateFileSystemModel(hypermedia);
            hypermedia = await DownloadRawHypermediaAsync(hypermedia);
            if (hypermedia.Status == Status.Seeding)
            {
                return _manager.FileSystemService.ClearTempFileSystem(hypermedia);
            }
            else
            {
                try
                {
                    _manager.FileSystemService.DeleteHypermedia(hypermedia, true);
                }
                catch { };
                return false;
            }
        }

        public async Task<bool> DownloadHypermediaAsync(Hypermedia.Hypermedia hypermedia, string downloadPath)
        {
            var converted = await _manager.HypermediaService.ConvertToRealmModelAsync
            (
                hypermedia,
                downloadPath,
                long.MinValue
            );

            return await DownloadHypermediaAsync(converted);
        }

        public async Task<bool> DownloadHypermediaAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            WrappingOptions wrappingOptions
        )
        {
            var converted = await _manager.HypermediaService.ConvertToRealmModelAsync
            (
                hypermedia,
                downloadPath,
                isAttributesPreservationEnabled,
                isContinuousDownloadingEnabled,
                long.MinValue,
                wrappingOptions,
                Priority.High,
                Status.Downloading
            );

            return await DownloadHypermediaAsync(converted);
        }

        private async Task<Models.Hypermedia> DownloadRawHypermediaAsync(Models.Hypermedia hypermedia)
        {
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                hypermedia.Files[i] = await DownloadFileAsync(hypermedia.Files[i]);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                hypermedia.Directories[i] = await DownloadDirectoryAsync(hypermedia.Directories[i]);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                hypermedia.Hypermedias[i] = await DownloadRawHypermediaAsync(hypermedia.Hypermedias[i]);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }
            return await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
        }

        private Models.Hypermedia DownloadRawHypermedia(Models.Hypermedia hypermedia)
        {
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                hypermedia.Files[i] = DownloadFile(hypermedia.Files[i]);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                hypermedia.Directories[i] = DownloadDirectory(hypermedia.Directories[i]);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                hypermedia.Hypermedias[i] = DownloadRawHypermedia(hypermedia.Hypermedias[i]);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }
            return _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
        }

        private async Task<Models.Directory> DownloadDirectoryAsync(Models.Directory directory)
        {
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                directory.Files[i] = await DownloadFileAsync(directory.Files[i]);
                directory = await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory);
            }
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                directory.Directories[i] = await DownloadDirectoryAsync(directory.Directories[i]);
                directory = await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory);
            }
            return await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory);
        }

        private Models.Directory DownloadDirectory(Models.Directory directory)
        {
            for(int i = 0; i < directory.Files.Count; ++i)
            {
                directory.Files[i] = DownloadFile(directory.Files[i]);
                directory = _manager.FileSystemService.UpdateDirectoryStatus(directory);
            }
            for(int i = 0; i < directory.Directories.Count; ++i)
            {
                directory.Directories[i] = DownloadDirectory(directory.Directories[i]);
                directory = _manager.FileSystemService.UpdateDirectoryStatus(directory);
            }
            return _manager.FileSystemService.UpdateDirectoryStatus(directory);
        }

        private async Task<Models.File> DownloadFileAsync(Models.File file)
        {
            if (file.IsSingleBlock)
            {
                Task<System.IO.Stream> fileStream = null;
                CancellationToken token;
                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: check if setting compression flag to false is working correctly within Ipfs.Engine;
                    fileStream = _manager.Engine().FileSystem.GetAsync(file.Path, false, token);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                        {
                            isTimeout = true;
                            if (fileStream.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (fileStream.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);

                using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                {
                    fileStream.Result.CopyTo(fs);
                }

                fileStream.Result.Close();
            }
            else
            {
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    file.Blocks[i] = await DownloadBlockAsync(file.Blocks[i]);
                }

                file = await _manager.FileSystemService.UpdateFileStatusAsync(file);
                if (file.Status == Status.ReadyForReconstruction)
                {
                    _manager.FileSystemService.ReconstructFile(file);
                }
                else
                {
                    throw new Exception("Unexpected behaviour");
                }
            }

            return await _manager.FileSystemService.UpdateFileStatusAsync(file);
        }

        private Models.File DownloadFile(Models.File file)
        {
            if(file.IsSingleBlock)
            {
                Task<System.IO.Stream> fileStream = null;
                CancellationToken token;
                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: check if setting compression flag to false is working correctly within Ipfs.Engine;
                    fileStream = _manager.Engine().FileSystem.GetAsync(file.Path, false, token);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                        {
                            isTimeout = true;
                            if (fileStream.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (fileStream.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);

                using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                {
                    fileStream.Result.CopyTo(fs);
                }

                fileStream.Result.Close();
            }
            else
            {
                for(int i = 0; i < file.Blocks.Count; ++i)
                {
                    file.Blocks[i] = DownloadBlock(file.Blocks[i]);
                }

                file = _manager.FileSystemService.UpdateFileStatus(file);
                if (file.Status == Status.ReadyForReconstruction)
                {
                    _manager.FileSystemService.ReconstructFile(file);
                }
                else
                {
                    throw new Exception("Unexpected behaviour");
                }
            }

            return _manager.FileSystemService.UpdateFileStatus(file);
        }

        private async Task<Models.Block> DownloadBlockAsync(Models.Block block)
        {
            Task<System.IO.Stream> blockStream = null;
            CancellationToken token;
            do
            {
                CancellationTokenSource source = new CancellationTokenSource();
                token = source.Token;
                //TODO: check if setting compression flag to false is working correctly within Ipfs.Engine;
                blockStream = _manager.Engine().FileSystem.GetAsync(block.Path, false, token);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                bool isTimeout = false, isTaskCompleted = false;

                while (!isTimeout && !isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                    {
                        isTimeout = true;
                        if (blockStream.Status != TaskStatus.RanToCompletion)
                        {
                            source.Cancel();
                            break;
                        }
                    }

                    if (blockStream.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            using (var fs = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
            {
                blockStream.Result.CopyTo(fs);
            }

            blockStream.Result.Close();

            return await _manager.FileSystemService.UpdateBlockStatusAsync(block);
        }

        private Models.Block DownloadBlock(Models.Block block)
        {
            Task<System.IO.Stream> blockStream = null;
            CancellationToken token;
            do
            {
                CancellationTokenSource source = new CancellationTokenSource();
                token = source.Token;
                //TODO: check if setting compression flag to false is working correctly within Ipfs.Engine;
                blockStream = _manager.Engine().FileSystem.GetAsync(block.Path, false, token);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                bool isTimeout = false, isTaskCompleted = false;

                while (!isTimeout && !isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                    {
                        isTimeout = true;
                        if (blockStream.Status != TaskStatus.RanToCompletion)
                        {
                            source.Cancel();
                            break;
                        }
                    }

                    if (blockStream.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            using(var fs = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
            {
                blockStream.Result.CopyTo(fs);
            }

            blockStream.Result.Close();

            return _manager.FileSystemService.UpdateBlockStatus(block);
        }

        public bool StartAllDownloads()
        {
            throw new NotImplementedException();
        }

        public bool StartDownloadService()
        {
            throw new NotImplementedException();
        }

        public bool StartHypermediaDownloading(string path)
        {
            throw new NotImplementedException();
        }

        public bool StopAllDownloads()
        {
            throw new NotImplementedException();
        }

        public bool StopDownloadService()
        {
            throw new NotImplementedException();
        }

        public bool StopHypermediaDownloading(string path)
        {
            throw new NotImplementedException();
        }
    }
}
