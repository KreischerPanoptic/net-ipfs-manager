﻿using System;
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
using Ipfs.Manager.Tools.Results;
using Ipfs.Manager.Tools.Results.EntityResults;
using Ipfs.Manager.Tools.Queue;

namespace Ipfs.Manager.Services.Versions.DownloadService
{
    class BaseDownloadService : IDownloadService
    {
        private Manager _manager;
        private CancellationTokenSource _cancellation;
        private CancellationToken _token;

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

        private Tuple<List<IEntityResult>, List<IEntityResult>> GetEntitiesFromHypermediaResult(HypermediaResult hypermedia)
        {
            List<IEntityResult> corrupted = new List<IEntityResult>();
            List<IEntityResult> completed = new List<IEntityResult>();

            if (hypermedia.Status == EntityDownloadStatus.Completed)
            {
                completed.Add(hypermedia);
            }
            else
            {
                corrupted.Add(hypermedia);
            }

            foreach (var er in hypermedia.Results)
            {
                dynamic tuple;
                if (er is FileResult)
                {
                    tuple = GetEntitiesFromFileResult(er as FileResult);

                }
                else if (er is DirectoryResult)
                {
                    tuple = GetEntitiesFromDirectoryResult(er as DirectoryResult);
                }
                else
                {
                    tuple = GetEntitiesFromHypermediaResult(er as HypermediaResult);
                }
                completed.AddRange(tuple.Item1);
                corrupted.AddRange(tuple.Item2);
            }

            return new Tuple<List<IEntityResult>, List<IEntityResult>>(completed, corrupted);
        }

        private Tuple<List<IEntityResult>, List<IEntityResult>> GetEntitiesFromDirectoryResult(DirectoryResult directory)
        {
            List<IEntityResult> corrupted = new List<IEntityResult>();
            List<IEntityResult> completed = new List<IEntityResult>();

            if (directory.Status == EntityDownloadStatus.Completed)
            {
                completed.Add(directory);
            }
            else
            {
                corrupted.Add(directory);
            }

            foreach (var er in directory.Results)
            {
                dynamic tuple;
                if (er is FileResult)
                {
                    tuple = GetEntitiesFromFileResult(er as FileResult);

                }
                else
                {
                    tuple = GetEntitiesFromDirectoryResult(er as DirectoryResult);
                }
                completed.AddRange(tuple.Item1);
                corrupted.AddRange(tuple.Item2);
            }

            return new Tuple<List<IEntityResult>, List<IEntityResult>>(completed, corrupted);
        }

        private Tuple<List<IEntityResult>, List<IEntityResult>> GetEntitiesFromFileResult(FileResult file)
        {
            List<IEntityResult> corrupted = new List<IEntityResult>();
            List<IEntityResult> completed = new List<IEntityResult>();

            if (file.Status == EntityDownloadStatus.Completed)
            {
                completed.Add(file);
            }
            else
            {
                corrupted.Add(file);
            }

            foreach (var br in file.BlockResults)
            {
                var tuple = GetEntitiesFromBlockResult(br);
                completed.AddRange(tuple.Item1);
                corrupted.AddRange(tuple.Item2);
            }

            return new Tuple<List<IEntityResult>, List<IEntityResult>>(completed, corrupted);
        }

        private Tuple<List<IEntityResult>, List<IEntityResult>> GetEntitiesFromBlockResult(BlockResult block)
        {
            List<IEntityResult> corrupted = new List<IEntityResult>();
            List<IEntityResult> completed = new List<IEntityResult>();

            if (block.Status == EntityDownloadStatus.Completed)
            {
                completed.Add(block);
            }
            else
            {
                corrupted.Add(block);
            }

            return new Tuple<List<IEntityResult>, List<IEntityResult>>(completed, corrupted);
        }

        public Result DownloadHypermedia(Models.Hypermedia hypermedia)
        {
            Result result;
            _manager.FileSystemService.CreateFileSystemModel(hypermedia);
            var hypermediaResult = DownloadRawHypermedia(hypermedia);
            hypermedia = hypermediaResult.GetHypermedia();
            var tuple = GetEntitiesFromHypermediaResult(hypermediaResult);
            if (hypermediaResult.Status == EntityDownloadStatus.Completed)
            {
                if (_manager.FileSystemService.ClearTempFileSystem(hypermedia))
                {
                    result = new Result()
                    {
                        Status = ResultStatus.Completed,
                        Comment = "Download Successful. All temporary data cleared",
                        Hypermedia = hypermediaResult,
                        CompletedEntities = tuple.Item1,
                        CorruptedEntities = tuple.Item2
                    };
                    return result;
                }
                else
                {
                    result = new Result()
                    {
                        Status = ResultStatus.Error,
                        Comment = "Error encountered during temporary data cleaning",
                        Hypermedia = hypermediaResult,
                        CompletedEntities = tuple.Item1,
                        CorruptedEntities = tuple.Item2
                    };
                    return result;
                }
            }
            else
            {
                result = new Result()
                {
                    Status = ResultStatus.Error,
                    Comment = "Error encountered during download. Temporary data saved. Reloading possible",
                    Hypermedia = hypermediaResult,
                    CompletedEntities = tuple.Item1,
                    CorruptedEntities = tuple.Item2
                };
                return result;
            }
        }

        public Result DownloadHypermedia(Hypermedia.Hypermedia hypermedia, string downloadPath)
        {
            var converted = _manager.HypermediaService.ConvertToRealmModel
            (
                hypermedia,
                downloadPath,
                long.MinValue
            );

            return DownloadHypermedia(converted);
        }

        public Result DownloadHypermedia
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

        public async Task<Result> DownloadHypermediaAsync(Models.Hypermedia hypermedia)
        {
            Result result;
            _manager.FileSystemService.CreateFileSystemModel(hypermedia);
            var hypermediaResult = await DownloadRawHypermediaAsync(hypermedia);
            hypermedia = hypermediaResult.GetHypermedia();
            var tuple = GetEntitiesFromHypermediaResult(hypermediaResult);
            if (hypermediaResult.Status == EntityDownloadStatus.Completed)
            {
                if (_manager.FileSystemService.ClearTempFileSystem(hypermedia))
                {
                    result = new Result()
                    {
                        Status = ResultStatus.Completed,
                        Comment = "Download Successful. All temporary data cleared",
                        Hypermedia = hypermediaResult,
                        CompletedEntities = tuple.Item1,
                        CorruptedEntities = tuple.Item2
                    };
                    return result;
                }
                else
                {
                    result = new Result()
                    {
                        Status = ResultStatus.Error,
                        Comment = "Error encountered during temporary data cleaning",
                        Hypermedia = hypermediaResult,
                        CompletedEntities = tuple.Item1,
                        CorruptedEntities = tuple.Item2
                    };
                    return result;
                }
            }
            else
            {
                result = new Result()
                {
                    Status = ResultStatus.Error,
                    Comment = "Error encountered during download",
                    Hypermedia = hypermediaResult,
                    CompletedEntities = tuple.Item1,
                    CorruptedEntities = tuple.Item2
                };
                return result;
            }
        }

        public async Task<Result> DownloadHypermediaAsync(Hypermedia.Hypermedia hypermedia, string downloadPath)
        {
            var converted = await _manager.HypermediaService.ConvertToRealmModelAsync
            (
                hypermedia,
                downloadPath,
                long.MinValue
            );

            return await DownloadHypermediaAsync(converted);
        }

        public async Task<Result> DownloadHypermediaAsync
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

        //TODO: rewrite
        //public Result ReloadHypermedia(Result result)
        //{
        //    bool isCorruptionCleared = ClearCorruptedData(result);
        //    if(isCorruptionCleared)
        //    {
        //        List<Tuple<IEntityResult, IEntityResult>> fixedEntities = new List<Tuple<IEntityResult, IEntityResult>>();
        //        List<Tuple<IEntityResult, IEntityResult>> corruptedEntities = new List<Tuple<IEntityResult, IEntityResult>>();
        //        for(int i = 0; i < result.CorruptedEntities.Count; ++i)
        //        {
        //            if (result.CorruptedEntities[i] is BlockResult)
        //            {
        //                var block = (result.CorruptedEntities[i] as BlockResult).GetBlock();
        //                if (!System.IO.Directory.Exists(block.Parent.BlockStorePath))
        //                {
        //                    throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //                }
        //                else 
        //                {
        //                    if(System.IO.File.Exists(block.InternalPath))
        //                    {
        //                        try
        //                        {
        //                            System.IO.File.Delete(block.InternalPath);
        //                        }
        //                        catch
        //                        {
        //                            throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            System.IO.File.Create(block.InternalPath);
        //                        }
        //                        catch
        //                        {
        //                            throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //                        }
        //                    }

        //                    var blockResult = DownloadBlock(block);
        //                    if(blockResult.Status == EntityDownloadStatus.Completed)
        //                    {
        //                        fixedEntities.Add(new Tuple<IEntityResult, IEntityResult>(result.CorruptedEntities[i], blockResult));
        //                    }
        //                    else
        //                    {
        //                        corruptedEntities.Add(new Tuple<IEntityResult, IEntityResult>(result.CorruptedEntities[i], blockResult));
        //                    }
        //                }
        //            }
        //            else if (result.CorruptedEntities[i] is FileResult)
        //            {
        //                var file = (result.CorruptedEntities[i] as FileResult).GetFile();
        //                if (file.IsSingleBlock)
        //                {
        //                    if (System.IO.File.Exists(file.InternalPath))
        //                    {
        //                        try
        //                        {
        //                            System.IO.File.Delete(file.InternalPath);
        //                        }
        //                        catch
        //                        {
        //                            throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            System.IO.File.Create(file.InternalPath);
        //                        }
        //                        catch
        //                        {
        //                            throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //                        }
        //                    }

        //                    var fileResult = DownloadFile(file);
        //                    if (fileResult.Status == EntityDownloadStatus.Completed)
        //                    {
        //                        fixedEntities.Add(new Tuple<IEntityResult, IEntityResult>(result.CorruptedEntities[i], fileResult));
        //                    }
        //                    else
        //                    {
        //                        corruptedEntities.Add(new Tuple<IEntityResult, IEntityResult>(result.CorruptedEntities[i], fileResult));
        //                    }
        //                }
        //            }
        //        }

        //        foreach()
        //    }
        //    else
        //    {
        //        throw new ArgumentException($"Corruption of {nameof(result)} is beyond repair", nameof(result));
        //    }
        //}

        //public Task<Result> ReloadHypermediaAsync(Result result)
        //{

        //}

        public bool ClearCorruptedData(Result result)
        {
            if(result.Status == ResultStatus.Completed)
            {
                return false;
            }
            if (result.CorruptedEntities.Count <= 0)
            {
                return false;
            }
            else
            {
                foreach (var e in result.CorruptedEntities)
                {
                    if (e is BlockResult)
                    {
                        try
                        {
                            System.IO.File.Delete(e.InternalPath);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else if(e is FileResult)
                    {
                        if((e as FileResult).GetFile().IsSingleBlock)
                        {
                            try
                            {
                                System.IO.File.Delete(e.InternalPath);
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool ClearAllData(Result result)
        {
            var hypermedia = result.Hypermedia.GetHypermedia();
            return _manager.FileSystemService.DeleteHypermedia(hypermedia, true);
        }

        public async Task<bool> ClearAllDataAsync(Result result)
        {
            var hypermedia = result.Hypermedia.GetHypermedia();
            return await _manager.FileSystemService.DeleteHypermediaAsync(hypermedia, true);
        }

        private async Task<HypermediaResult> DownloadRawHypermediaAsync(Models.Hypermedia hypermedia)
        {
            HypermediaResult result;
            List<IEntityResult> entityResults = new List<IEntityResult>();

            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                var hypermediaResult = await DownloadFileAsync(hypermedia.Files[i]);
                hypermedia.Files[i] = hypermediaResult.GetFile();
                entityResults.Add(hypermediaResult);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                var directoryResult = await DownloadDirectoryAsync(hypermedia.Directories[i]);
                hypermedia.Directories[i] = directoryResult.GetDirectory();
                entityResults.Add(directoryResult);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                var hypermediaResult = await DownloadRawHypermediaAsync(hypermedia.Hypermedias[i]);
                hypermedia.Hypermedias[i] = hypermediaResult.GetHypermedia();
                entityResults.Add(hypermediaResult);
                hypermedia = await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status);
            }

            bool isCompleted = true;
            foreach (var er in entityResults)
            {
                if (er.Status != EntityDownloadStatus.Completed)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                result = new HypermediaResult(await _manager.FileSystemService.UpdateStatusAsync(hypermedia, hypermedia.Status), EntityDownloadStatus.Completed, "Download Successful");
                result.Results = entityResults;
                return result;
            }
            else
            {
                result = new HypermediaResult(hypermedia, EntityDownloadStatus.UnknownError, "Error occured during entities download");
                result.Results = entityResults;
                return result;
            }
        }

        private HypermediaResult DownloadRawHypermedia(Models.Hypermedia hypermedia)
        {
            HypermediaResult result;
            List<IEntityResult> entityResults = new List<IEntityResult>();

            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                var hypermediaResult = DownloadFile(hypermedia.Files[i]);
                hypermedia.Files[i] = hypermediaResult.GetFile();
                entityResults.Add(hypermediaResult);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                var directoryResult = DownloadDirectory(hypermedia.Directories[i]);
                hypermedia.Directories[i] = directoryResult.GetDirectory();
                entityResults.Add(directoryResult);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                var hypermediaResult = DownloadRawHypermedia(hypermedia.Hypermedias[i]);
                hypermedia.Hypermedias[i] = hypermediaResult.GetHypermedia();
                entityResults.Add(hypermediaResult);
                hypermedia = _manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status);
            }

            bool isCompleted = true;
            foreach (var er in entityResults)
            {
                if (er.Status != EntityDownloadStatus.Completed)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                result = new HypermediaResult(_manager.FileSystemService.UpdateStatus(hypermedia, hypermedia.Status), EntityDownloadStatus.Completed, "Download Successful");
                result.Results = entityResults;
                return result;
            }
            else
            {
                result = new HypermediaResult(hypermedia, EntityDownloadStatus.UnknownError, "Error occured during entities download");
                result.Results = entityResults;
                return result;
            }
        }

        private async Task<DirectoryResult> DownloadDirectoryAsync(Models.Directory directory)
        {
            DirectoryResult result;
            List<IEntityResult> entityResults = new List<IEntityResult>();

            for (int i = 0; i < directory.Files.Count; ++i)
            {
                var fileResult = await DownloadFileAsync(directory.Files[i]);
                directory.Files[i] = fileResult.GetFile();
                entityResults.Add(fileResult);
                directory = await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory);
            }
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                var directoryResult = await DownloadDirectoryAsync(directory.Directories[i]);
                directory.Directories[i] = directoryResult.GetDirectory();
                entityResults.Add(directoryResult);
                directory = await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory);
            }

            bool isCompleted = true;
            foreach (var er in entityResults)
            {
                if (er.Status != EntityDownloadStatus.Completed)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                result = new DirectoryResult(await _manager.FileSystemService.UpdateDirectoryStatusAsync(directory), EntityDownloadStatus.Completed, "Download Successful");
                result.Results = entityResults;
                return result;
            }
            else
            {
                result = new DirectoryResult(directory, EntityDownloadStatus.UnknownError, "Error occured during entities download");
                result.Results = entityResults;
                return result;
            }
        }

        private DirectoryResult DownloadDirectory(Models.Directory directory)
        {
            DirectoryResult result;
            List<IEntityResult> entityResults = new List<IEntityResult>();

            for(int i = 0; i < directory.Files.Count; ++i)
            {
                var fileResult = DownloadFile(directory.Files[i]);
                directory.Files[i] = fileResult.GetFile();
                entityResults.Add(fileResult);
                directory = _manager.FileSystemService.UpdateDirectoryStatus(directory);
            }
            for(int i = 0; i < directory.Directories.Count; ++i)
            {
                var directoryResult = DownloadDirectory(directory.Directories[i]);
                directory.Directories[i] = directoryResult.GetDirectory();
                entityResults.Add(directoryResult);
                directory = _manager.FileSystemService.UpdateDirectoryStatus(directory);
            }

            bool isCompleted = true;
            foreach(var er in entityResults)
            {
                if(er.Status != EntityDownloadStatus.Completed)
                {
                    isCompleted = false;
                }
            }

            if(isCompleted)
            {
                result = new DirectoryResult(_manager.FileSystemService.UpdateDirectoryStatus(directory), EntityDownloadStatus.Completed, "Download Successful");
                result.Results = entityResults;
                return result;
            }
            else
            {
                result = new DirectoryResult(directory, EntityDownloadStatus.UnknownError, "Error occured during entities download");
                result.Results = entityResults;
                return result;
            }
        }

        private async Task<FileResult> DownloadFileAsync(Models.File file)
        {
            FileResult result;
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
                    bool isTaskCompleted = false;

                    while (!isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                        {
                            result = new FileResult(file, EntityDownloadStatus.TimeoutError, "Encountered timeout error, while trying to download file");
                            return result;
                        }

                        if (fileStream.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);

                try
                {
                    using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                    {
                        fileStream.Result.CopyTo(fs);
                    }
                    fileStream.Result.Close();
                }
                catch (Exception e)
                {
                    result = new FileResult(file, EntityDownloadStatus.FileSystemError, $"Encountered filesystem error: {e.Message}");
                    return result;
                }

                result = new FileResult(_manager.FileSystemService.UpdateFileStatus(file), EntityDownloadStatus.Completed, "Download Successful");
                return result;
            }
            else
            {
                List<BlockResult> blockResults = new List<BlockResult>();
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    var br = await DownloadBlockAsync(file.Blocks[i]);
                    blockResults.Add(br);
                    file.Blocks[i] = br.GetBlock();
                }

                bool isCompleted = true;
                foreach (var br in blockResults)
                {
                    if (br.Status != EntityDownloadStatus.Completed)
                    {
                        isCompleted = false;
                    }
                }

                if (isCompleted)
                {
                    file = await _manager.FileSystemService.UpdateFileStatusAsync(file);
                    if (file.Status == Status.ReadyForReconstruction)
                    {
                        if (_manager.FileSystemService.ReconstructFile(file))
                        {
                            result = new FileResult(await _manager.FileSystemService.UpdateFileStatusAsync(file), EntityDownloadStatus.Completed, "Download Successful");
                            result.BlockResults = blockResults;
                            return result;
                        }
                        else
                        {
                            result = new FileResult(file, EntityDownloadStatus.FileSystemError, "Unknown error during reconstruction proccess");
                            result.BlockResults = blockResults;
                            return result;
                        }
                    }
                    else
                    {
                        result = new FileResult(file, EntityDownloadStatus.UnknownError, "Unknown error during reconstruction preparations");
                        result.BlockResults = blockResults;
                        return result;
                    }
                }
                else
                {
                    result = new FileResult(file, EntityDownloadStatus.UnknownError, "Error occured during blocks download");
                    result.BlockResults = blockResults;
                    return result;
                }
            }
        }

        private FileResult DownloadFile(Models.File file)
        {
            FileResult result;
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
                    bool isTaskCompleted = false;

                    while (!isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                        {
                            result = new FileResult(file, EntityDownloadStatus.TimeoutError, "Encountered timeout error, while trying to download file");
                            return result;
                        }

                        if (fileStream.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);

                try
                {
                    using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                    {
                        fileStream.Result.CopyTo(fs);
                    }
                    fileStream.Result.Close();
                }
                catch (Exception e)
                {
                    result = new FileResult(file, EntityDownloadStatus.FileSystemError, $"Encountered filesystem error: {e.Message}");
                    return result;
                }

                result = new FileResult(_manager.FileSystemService.UpdateFileStatus(file), EntityDownloadStatus.Completed, "Download Successful");
                return result;
            }
            else
            {
                List<BlockResult> blockResults = new List<BlockResult>();
                for(int i = 0; i < file.Blocks.Count; ++i)
                {
                    var br = DownloadBlock(file.Blocks[i]);
                    blockResults.Add(br);
                    file.Blocks[i] = br.GetBlock();
                }

                bool isCompleted = true;
                foreach(var br in blockResults)
                {
                    if(br.Status != EntityDownloadStatus.Completed)
                    {
                        isCompleted = false;
                    }
                }

                if (isCompleted)
                {
                    file = _manager.FileSystemService.UpdateFileStatus(file);
                    if (file.Status == Status.ReadyForReconstruction)
                    {
                        if (_manager.FileSystemService.ReconstructFile(file))
                        {
                            result = new FileResult(_manager.FileSystemService.UpdateFileStatus(file), EntityDownloadStatus.Completed, "Download Successful");
                            result.BlockResults = blockResults;
                            return result;
                        }
                        else
                        {
                            result = new FileResult(file, EntityDownloadStatus.FileSystemError, "Unknown error during reconstruction proccess");
                            result.BlockResults = blockResults;
                            return result;
                        }
                    }
                    else
                    {
                        result = new FileResult(file, EntityDownloadStatus.UnknownError, "Unknown error during reconstruction preparations");
                        result.BlockResults = blockResults;
                        return result;
                    }
                }
                else
                {
                    result = new FileResult(file, EntityDownloadStatus.UnknownError, "Error occured during blocks download");
                    result.BlockResults = blockResults;
                    return result;
                }
            }
        }

        private async Task<BlockResult> DownloadBlockAsync(Models.Block block)
        {
            BlockResult result;
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
                bool isTaskCompleted = false;

                while (!isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                    {
                        result = new BlockResult(block, EntityDownloadStatus.TimeoutError, "Encountered timeout error, while trying to download block");
                        return result;
                    }

                    if (blockStream.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            try
            {
                using (var fs = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
                {
                    blockStream.Result.CopyTo(fs);
                }
                blockStream.Result.Close();
            }
            catch (Exception e)
            {
                result = new BlockResult(block, EntityDownloadStatus.FileSystemError, $"Encountered filesystem error: {e.Message}");
                return result;
            }

            result = new BlockResult(await _manager.FileSystemService.UpdateBlockStatusAsync(block), EntityDownloadStatus.Completed, "Download Successful");
            return result;
        }

        private BlockResult DownloadBlock(Models.Block block)
        {
            BlockResult result;
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
                bool isTaskCompleted = false;

                while (!isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                    {
                        result = new BlockResult(block, EntityDownloadStatus.TimeoutError, "Encountered timeout error, while trying to download block");
                        return result;
                    }

                    if (blockStream.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            try
            {
                using (var fs = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
                {
                    blockStream.Result.CopyTo(fs);
                }
                blockStream.Result.Close();
            }
            catch (Exception e)
            {
                result = new BlockResult(block, EntityDownloadStatus.FileSystemError, $"Encountered filesystem error: {e.Message}");
                return result;
            }

            result = new BlockResult(_manager.FileSystemService.UpdateBlockStatus(block), EntityDownloadStatus.Completed, "Download Successful");
            return result;
        }

        public bool StartAllDownloads()
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReferences = _manager.RealmService.ReadThreadSafeHypermedias();
            List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
            foreach (var t in threadSafeReferences)
            {
                hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
            }

            bool isAllStarted = true;
            foreach (var h in hypermedias)
            {
                if (h != null)
                {
                    Status status = Status.ReadyForDownload;
                    if (h.Status == Status.Completed)
                    {
                        status = Status.Seeding;
                    }
                    currentRealm.Write(() =>
                    {
                        h.Status = status;
                    });
                    isAllStarted = true;
                }
                else
                {
                    isAllStarted = false;
                }
            }
            return isAllStarted;
        }

        public bool StartDownloadService()
        {
            _cancellation = new CancellationTokenSource();
            _token = _cancellation.Token;

            Thread downloadsThread = new Thread(() => ProcessDownloads(_token))
            {
                IsBackground = true
            };

            try
            {
                downloadsThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ProcessDownloads(CancellationToken token)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());

            bool isFirstRun = true;
            DownloadQueue queue = new DownloadQueue { Items = new Dictionary<int, DownloadItem>(), Status = DownloadQueueStatus.Created };
            DownloadQueue prev = new DownloadQueue { Items = new Dictionary<int, DownloadItem>(), Status = DownloadQueueStatus.Created };
            while (!token.IsCancellationRequested)
            {
                //TODO: check if deep copy is needed
                prev = queue;
                queue.Items.Clear();
                var threadSafeReferences = _manager.RealmService.ReadThreadSafeHypermedias();
                List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
                foreach (var t in threadSafeReferences)
                {
                    hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
                }

                //TODO: check if order is correct
                hypermedias = hypermedias.OrderBy(h => h.Priority).ToList();
                hypermedias = hypermedias.OrderBy(h => h.QueuePosition).ToList();

                if (hypermedias.Count > 0)
                {
                    for (int i = 0; i < hypermedias.Count; ++i)
                    {
                        DownloadItemStatus status = DownloadItemStatus.Ignored;
                        if (i < 3) //TODO: check to number in environment service
                        {
                            status = DownloadItemStatus.Prepared;
                        }
                        else
                        {
                            status = DownloadItemStatus.Waiting;
                        }

                        if(hypermedias[i].Priority ==  Priority.Ignore)
                        {
                            status = DownloadItemStatus.Ignored;
                        }

                        if 
                        (
                               hypermedias[i].Status == Status.Completed
                            || hypermedias[i].Status == Status.Error
                            || hypermedias[i].Status == Status.Seeding
                            || hypermedias[i].Status == Status.Stopped
                        )
                        {
                            status = DownloadItemStatus.Ignored;
                        }

                        queue.Items.Add(i, new DownloadItem
                        {
                            Item = ThreadSafeReference.Create<Models.Hypermedia>(hypermedias[i]),
                            Status = status
                        });
                    }
                    queue.Status = DownloadQueueStatus.Prepared;
                }
                else
                {
                    continue;
                }

                if (!isFirstRun)
                {
                    //TODO: add checking of queue;
                }
                else
                {
                    foreach (var item in queue.Items)
                    {
                        if (item.Value.Status == DownloadItemStatus.Prepared)
                        {
                            //TODO: add cancellation token;
                            Thread downloadThread = new Thread(() => DownloadHypermedia(item.Value.Item))
                            {
                                IsBackground = true
                            };
                            downloadThread.Start();
                            item.Value.Status = DownloadItemStatus.Launched;
                        }
                    }

                    queue.Status = DownloadQueueStatus.Processing;
                    isFirstRun = false;
                }
            }
            //TODO: add cleaning of queue;
        }

        private void DownloadHypermedia(ThreadSafeReference.Object<Models.Hypermedia> hypermediaReference)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(hypermediaReference);
            //TODO: check if deep copy is needed
            Models.Hypermedia newHypermedia = hypermedia;

            int fileCount = hypermedia.Files.Count;
            int completedFilesCount = 0;
            int directoryCount = hypermedia.Directories.Count;
            int completedDirectoriesCount = 0;
            int hypermediaCount = hypermedia.Hypermedias.Count;
            int completedHypermediasCount = 0;
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                newHypermedia = hypermedia;
                var fileReference = ThreadSafeReference.Create<Models.File>(newHypermedia.Files[i]);
                DownloadFile(fileReference);
                newHypermedia = _manager.FileSystemService.UpdateStatus(newHypermedia, newHypermedia.Status);
                Models.File file = currentRealm.ResolveReference<Models.File>(fileReference);
                if (file.Status != Status.Error)
                {
                    ++completedFilesCount;
                }

                currentRealm.Write(() =>
                {
                    hypermedia.Files.Clear();
                    foreach (var f in newHypermedia.Files)
                    {
                        hypermedia.Files.Add(f);
                    }
                    hypermedia.Encoding = newHypermedia.Encoding;
                    hypermedia.Directories.Clear();
                    foreach (var d in newHypermedia.Directories)
                    {
                        hypermedia.Directories.Add(d);
                    }
                    hypermedia.Hash = newHypermedia.Hash;
                    hypermedia.Hypermedias.Clear();
                    foreach (var h in newHypermedia.Hypermedias)
                    {
                        hypermedia.Hypermedias.Add(h);
                    }
                    hypermedia.Index = newHypermedia.Index;
                    hypermedia.InternalPath = newHypermedia.InternalPath;
                    hypermedia.Name = newHypermedia.Name;
                    hypermedia.Parent = newHypermedia.Parent;
                    hypermedia.Path = newHypermedia.Path;
                    hypermedia.Priority = newHypermedia.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = hypermedia.Progress;
                    progress.Report((completedFilesCount + completedDirectoriesCount + completedHypermediasCount) / (fileCount + directoryCount + hypermediaCount) * 100);
                    //
                    hypermedia.Size = newHypermedia.Size;
                    hypermedia.Status = newHypermedia.Status;
                });
            }
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                newHypermedia = hypermedia;
                var directoryReference = ThreadSafeReference.Create<Models.Directory>(newHypermedia.Directories[i]);
                DownloadDirectory(directoryReference);
                newHypermedia = _manager.FileSystemService.UpdateStatus(newHypermedia, newHypermedia.Status);
                Models.Directory directory = currentRealm.ResolveReference<Models.Directory>(directoryReference);
                if (directory.Status != Status.Error)
                {
                    ++completedDirectoriesCount;
                }

                currentRealm.Write(() =>
                {
                    hypermedia.Files.Clear();
                    foreach (var f in newHypermedia.Files)
                    {
                        hypermedia.Files.Add(f);
                    }
                    hypermedia.Encoding = newHypermedia.Encoding;
                    hypermedia.Directories.Clear();
                    foreach (var d in newHypermedia.Directories)
                    {
                        hypermedia.Directories.Add(d);
                    }
                    hypermedia.Hash = newHypermedia.Hash;
                    hypermedia.Hypermedias.Clear();
                    foreach (var h in newHypermedia.Hypermedias)
                    {
                        hypermedia.Hypermedias.Add(h);
                    }
                    hypermedia.Index = newHypermedia.Index;
                    hypermedia.InternalPath = newHypermedia.InternalPath;
                    hypermedia.Name = newHypermedia.Name;
                    hypermedia.Parent = newHypermedia.Parent;
                    hypermedia.Path = newHypermedia.Path;
                    hypermedia.Priority = newHypermedia.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = hypermedia.Progress;
                    progress.Report((completedFilesCount + completedDirectoriesCount + completedHypermediasCount) / (fileCount + directoryCount + hypermediaCount) * 100);
                    //
                    hypermedia.Size = newHypermedia.Size;
                    hypermedia.Status = newHypermedia.Status;
                });
            }
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                newHypermedia = hypermedia;
                var internalHypermediaReference = ThreadSafeReference.Create<Models.Hypermedia>(newHypermedia.Hypermedias[i]);
                DownloadHypermedia(internalHypermediaReference);
                newHypermedia = _manager.FileSystemService.UpdateStatus(newHypermedia, newHypermedia.Status);
                Models.Hypermedia intrnalHypermedia = currentRealm.ResolveReference<Models.Hypermedia>(internalHypermediaReference);
                if (intrnalHypermedia.Status != Status.Error)
                {
                    ++completedHypermediasCount;
                }

                currentRealm.Write(() =>
                {
                    hypermedia.Files.Clear();
                    foreach (var f in newHypermedia.Files)
                    {
                        hypermedia.Files.Add(f);
                    }
                    hypermedia.Encoding = newHypermedia.Encoding;
                    hypermedia.Directories.Clear();
                    foreach (var d in newHypermedia.Directories)
                    {
                        hypermedia.Directories.Add(d);
                    }
                    hypermedia.Hash = newHypermedia.Hash;
                    hypermedia.Hypermedias.Clear();
                    foreach (var h in newHypermedia.Hypermedias)
                    {
                        hypermedia.Hypermedias.Add(h);
                    }
                    hypermedia.Index = newHypermedia.Index;
                    hypermedia.InternalPath = newHypermedia.InternalPath;
                    hypermedia.Name = newHypermedia.Name;
                    hypermedia.Parent = newHypermedia.Parent;
                    hypermedia.Path = newHypermedia.Path;
                    hypermedia.Priority = newHypermedia.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = hypermedia.Progress;
                    progress.Report((completedFilesCount + completedDirectoriesCount + completedHypermediasCount) / (fileCount + directoryCount + hypermediaCount) * 100);
                    //
                    hypermedia.Size = newHypermedia.Size;
                    hypermedia.Status = newHypermedia.Status;
                });
            }

            bool isCompleted = true;
            if (newHypermedia.Status == Status.Error)
            {
                isCompleted = false;
            }
            foreach (var f in newHypermedia.Files)
            {
                if (f.Status == Status.Error)
                {
                    isCompleted = false;
                }
            }
            foreach (var d in newHypermedia.Directories)
            {
                if (d.Status == Status.Error)
                {
                    isCompleted = false;
                }
            }
            foreach (var h in newHypermedia.Hypermedias)
            {
                if (h.Status == Status.Error)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                newHypermedia = _manager.FileSystemService.UpdateStatus(newHypermedia, newHypermedia.Status);
                currentRealm.Write(() =>
                {
                    hypermedia.Completed = DateTimeOffset.Now;
                    hypermedia.Files.Clear();
                    foreach (var f in newHypermedia.Files)
                    {
                        hypermedia.Files.Add(f);
                    }
                    hypermedia.Encoding = newHypermedia.Encoding;
                    hypermedia.Directories.Clear();
                    foreach (var d in newHypermedia.Directories)
                    {
                        hypermedia.Directories.Add(d);
                    }
                    hypermedia.Hash = newHypermedia.Hash;
                    hypermedia.Hypermedias.Clear();
                    foreach(var h in newHypermedia.Hypermedias)
                    {
                        hypermedia.Hypermedias.Add(h);
                    }
                    hypermedia.Index = newHypermedia.Index;
                    hypermedia.InternalPath = newHypermedia.InternalPath;
                    hypermedia.Name = newHypermedia.Name;
                    hypermedia.Parent = newHypermedia.Parent;
                    hypermedia.Path = newHypermedia.Path;
                    hypermedia.Priority = newHypermedia.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = hypermedia.Progress;
                    progress.Report(100);
                    //
                    hypermedia.Size = newHypermedia.Size;
                    hypermedia.Status = newHypermedia.Status;
                });
            }
            else
            {
                newHypermedia = _manager.FileSystemService.UpdateStatus(newHypermedia, newHypermedia.Status);
                currentRealm.Write(() =>
                {
                    hypermedia.Completed = DateTimeOffset.Now;
                    hypermedia.Files.Clear();
                    foreach (var f in newHypermedia.Files)
                    {
                        hypermedia.Files.Add(f);
                    }
                    hypermedia.Encoding = newHypermedia.Encoding;
                    hypermedia.Directories.Clear();
                    foreach (var d in newHypermedia.Directories)
                    {
                        hypermedia.Directories.Add(d);
                    }
                    hypermedia.Hash = newHypermedia.Hash;
                    hypermedia.Hypermedias.Clear();
                    foreach (var h in newHypermedia.Hypermedias)
                    {
                        hypermedia.Hypermedias.Add(h);
                    }
                    hypermedia.Index = newHypermedia.Index;
                    hypermedia.InternalPath = newHypermedia.InternalPath;
                    hypermedia.Name = newHypermedia.Name;
                    hypermedia.Parent = newHypermedia.Parent;
                    hypermedia.Path = newHypermedia.Path;
                    hypermedia.Priority = newHypermedia.Priority;
                    hypermedia.Size = newHypermedia.Size;
                    hypermedia.Status = Status.Error;
                });
            }
        }

        private void DownloadDirectory(ThreadSafeReference.Object<Models.Directory> directoryReference)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            Models.Directory directory = currentRealm.ResolveReference<Models.Directory>(directoryReference);
            //TODO: check if deep copy is needed
            Models.Directory newDirectory = directory;

            int fileCount = directory.Files.Count;
            int completedFilesCount = 0;
            int directoryCount = directory.Directories.Count;
            int completedDirectoriesCount = 0;
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                newDirectory = directory;
                var fileReference = ThreadSafeReference.Create<Models.File>(newDirectory.Files[i]);
                DownloadFile(fileReference);
                newDirectory = _manager.FileSystemService.UpdateDirectoryStatus(newDirectory);
                Models.File file = currentRealm.ResolveReference<Models.File>(fileReference);
                if (file.Status != Status.Error)
                {
                    ++completedFilesCount;
                }
                //TODO: check if it won't break for loop
                currentRealm.Write(() =>
                {
                    directory.Attributes = newDirectory.Attributes;
                    directory.Files.Clear();
                    foreach (var f in newDirectory.Files)
                    {
                        directory.Files.Add(f);
                    }
                    directory.Directories.Clear();
                    foreach (var d in newDirectory.Directories)
                    {
                        directory.Directories.Add(d);
                    }
                    directory.Hash = newDirectory.Hash;
                    directory.Index = newDirectory.Index;
                    directory.InternalPath = newDirectory.InternalPath;
                    directory.LastModifiedDateTime = newDirectory.LastModifiedDateTime;
                    directory.Name = newDirectory.Name;
                    directory.Parent = newDirectory.Parent;
                    directory.Path = newDirectory.Path;
                    directory.Priority = newDirectory.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = directory.Progress;
                    progress.Report((completedFilesCount + completedDirectoriesCount) / (fileCount + directoryCount) * 100);
                    //
                    directory.Size = newDirectory.Size;
                    directory.Status = newDirectory.Status;
                });
            }
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                newDirectory = directory;
                var internalDirectoryReference = ThreadSafeReference.Create<Models.Directory>(newDirectory.Directories[i]);
                DownloadDirectory(internalDirectoryReference);
                newDirectory = _manager.FileSystemService.UpdateDirectoryStatus(newDirectory);
                Models.Directory internalDirectory = currentRealm.ResolveReference<Models.Directory>(internalDirectoryReference);
                if (internalDirectory.Status != Status.Error)
                {
                    ++completedDirectoriesCount;
                }
                //TODO: check if it won't break for loop
                currentRealm.Write(() =>
                {
                    directory.Attributes = newDirectory.Attributes;
                    directory.Files.Clear();
                    foreach (var f in newDirectory.Files)
                    {
                        directory.Files.Add(f);
                    }
                    directory.Directories.Clear();
                    foreach (var d in newDirectory.Directories)
                    {
                        directory.Directories.Add(d);
                    }
                    directory.Hash = newDirectory.Hash;
                    directory.Index = newDirectory.Index;
                    directory.InternalPath = newDirectory.InternalPath;
                    directory.LastModifiedDateTime = newDirectory.LastModifiedDateTime;
                    directory.Name = newDirectory.Name;
                    directory.Parent = newDirectory.Parent;
                    directory.Path = newDirectory.Path;
                    directory.Priority = newDirectory.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = directory.Progress;
                    progress.Report((completedFilesCount + completedDirectoriesCount) / (fileCount + directoryCount) * 100);
                    //
                    directory.Size = newDirectory.Size;
                    directory.Status = newDirectory.Status;
                });
            }

            bool isCompleted = true;
            if (newDirectory.Status == Status.Error)
            {
                isCompleted = false;
            }
            foreach (var f in newDirectory.Files)
            {
                if (f.Status == Status.Error)
                {
                    isCompleted = false;
                }
            }
            foreach (var d in newDirectory.Directories)
            {
                if (d.Status == Status.Error)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                newDirectory = _manager.FileSystemService.UpdateDirectoryStatus(newDirectory);
                currentRealm.Write(() =>
                {
                    directory.Attributes = newDirectory.Attributes;
                    directory.Files.Clear();
                    foreach (var f in newDirectory.Files)
                    {
                        directory.Files.Add(f);
                    }
                    directory.Directories.Clear();
                    foreach (var d in newDirectory.Directories)
                    {
                        directory.Directories.Add(d);
                    }
                    directory.Hash = newDirectory.Hash;
                    directory.Index = newDirectory.Index;
                    directory.InternalPath = newDirectory.InternalPath;
                    directory.LastModifiedDateTime = newDirectory.LastModifiedDateTime;
                    directory.Name = newDirectory.Name;
                    directory.Parent = newDirectory.Parent;
                    directory.Path = newDirectory.Path;
                    directory.Priority = newDirectory.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = directory.Progress;
                    progress.Report(100);
                    //
                    directory.Size = newDirectory.Size;
                    directory.Status = newDirectory.Status;
                });
            }
            else
            {
                currentRealm.Write(() =>
                {
                    directory.Attributes = newDirectory.Attributes;
                    directory.Files.Clear();
                    foreach (var f in newDirectory.Files)
                    {
                        directory.Files.Add(f);
                    }
                    directory.Directories.Clear();
                    foreach (var d in newDirectory.Directories)
                    {
                        directory.Directories.Add(d);
                    }
                    directory.Hash = newDirectory.Hash;
                    directory.Index = newDirectory.Index;
                    directory.InternalPath = newDirectory.InternalPath;
                    directory.LastModifiedDateTime = newDirectory.LastModifiedDateTime;
                    directory.Name = newDirectory.Name;
                    directory.Parent = newDirectory.Parent;
                    directory.Path = newDirectory.Path;
                    directory.Priority = newDirectory.Priority;
                    directory.Size = newDirectory.Size;
                    directory.Status = Status.Error;
                });
            }
        }

        private void DownloadFile(ThreadSafeReference.Object<Models.File> fileReference)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            Models.File file = currentRealm.ResolveReference<Models.File>(fileReference);
            //TODO: check if deep copy is needed
            Models.File newFile = file;

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
                    bool isTaskCompleted = false;

                    while (!isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                        {
                            currentRealm.Write(() =>
                            {
                                file.Status = Status.Error;
                            });
                        }

                        if (fileStream.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);

                try
                {
                    using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                    {
                        fileStream.Result.CopyTo(fs);
                    }
                    fileStream.Result.Close();
                }
                catch (Exception e)
                {
                    currentRealm.Write(() =>
                    {
                        file.Status = Status.Error;
                    });
                }

                newFile = _manager.FileSystemService.UpdateFileStatus(file);
                currentRealm.Write(() =>
                {
                    file.Attributes = newFile.Attributes;
                    file.Blocks.Clear();
                    foreach(var b in newFile.Blocks)
                    {
                        file.Blocks.Add(b);
                    }
                    file.BlockStorePath = newFile.BlockStorePath;
                    file.Extension = newFile.Extension;
                    file.Hash = newFile.Hash;
                    file.Index = newFile.Index;
                    file.InternalPath = newFile.InternalPath;
                    file.IsSingleBlock = newFile.IsSingleBlock;
                    file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                    file.Name = newFile.Name;
                    file.Parent = newFile.Parent;
                    file.Path = newFile.Path;
                    file.Priority = newFile.Priority;
                    //TODO: check if such progress reporting works within Realm DB
                    IProgress<Double> progress = file.Progress;
                    progress.Report(newFile.Status != Status.Error ? 100 : 0);
                    //
                    file.Size = newFile.Size;
                    file.Status = newFile.Status;
                });
            }
            else
            {
                int blockCount = file.Blocks.Count;
                int completedBlocksCount = 0;
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    newFile = file;
                    var blockReference = ThreadSafeReference.Create<Models.Block>(newFile.Blocks[i]);
                    DownloadBlock(blockReference);
                    newFile = _manager.FileSystemService.UpdateFileStatus(newFile);
                    Models.Block block = currentRealm.ResolveReference<Models.Block>(blockReference);
                    if(block.Status != Status.Error)
                    {
                        ++completedBlocksCount;
                    }
                    //TODO: check if it won't break for loop
                    currentRealm.Write(() =>
                    {
                        file.Attributes = newFile.Attributes;
                        file.Blocks.Clear();
                        foreach (var b in newFile.Blocks)
                        {
                            file.Blocks.Add(b);
                        }
                        file.BlockStorePath = newFile.BlockStorePath;
                        file.Extension = newFile.Extension;
                        file.Hash = newFile.Hash;
                        file.Index = newFile.Index;
                        file.InternalPath = newFile.InternalPath;
                        file.IsSingleBlock = newFile.IsSingleBlock;
                        file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                        file.Name = newFile.Name;
                        file.Parent = newFile.Parent;
                        file.Path = newFile.Path;
                        file.Priority = newFile.Priority;
                        //TODO: check if such progress reporting works within Realm DB
                        IProgress<Double> progress = file.Progress;
                        progress.Report(completedBlocksCount / blockCount * 100);
                        //
                        file.Size = newFile.Size;
                        file.Status = newFile.Status;
                    });
                }

                bool isCompleted = true;
                if(newFile.Status == Status.Error)
                {
                    isCompleted = false;
                }
                foreach(var b in newFile.Blocks)
                {
                    if(b.Status == Status.Error)
                    {
                        isCompleted = false;
                    }
                }

                if (isCompleted)
                {
                    newFile = _manager.FileSystemService.UpdateFileStatus(newFile);
                    if (newFile.Status == Status.ReadyForReconstruction)
                    {
                        if (_manager.FileSystemService.ReconstructFile(newFile))
                        {
                            newFile = _manager.FileSystemService.UpdateFileStatus(newFile);
                            currentRealm.Write(() =>
                            {
                                file.Attributes = newFile.Attributes;
                                file.Blocks.Clear();
                                foreach (var b in newFile.Blocks)
                                {
                                    file.Blocks.Add(b);
                                }
                                file.BlockStorePath = newFile.BlockStorePath;
                                file.Extension = newFile.Extension;
                                file.Hash = newFile.Hash;
                                file.Index = newFile.Index;
                                file.InternalPath = newFile.InternalPath;
                                file.IsSingleBlock = newFile.IsSingleBlock;
                                file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                                file.Name = newFile.Name;
                                file.Parent = newFile.Parent;
                                file.Path = newFile.Path;
                                file.Priority = newFile.Priority;
                                //TODO: check if such progress reporting works within Realm DB
                                IProgress<Double> progress = file.Progress;
                                progress.Report(100);
                                //
                                file.Size = newFile.Size;
                                file.Status = newFile.Status;
                            });
                        }
                        else
                        {
                            currentRealm.Write(() =>
                            {
                                file.Attributes = newFile.Attributes;
                                file.Blocks.Clear();
                                foreach (var b in newFile.Blocks)
                                {
                                    file.Blocks.Add(b);
                                }
                                file.BlockStorePath = newFile.BlockStorePath;
                                file.Extension = newFile.Extension;
                                file.Hash = newFile.Hash;
                                file.Index = newFile.Index;
                                file.InternalPath = newFile.InternalPath;
                                file.IsSingleBlock = newFile.IsSingleBlock;
                                file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                                file.Name = newFile.Name;
                                file.Parent = newFile.Parent;
                                file.Path = newFile.Path;
                                file.Priority = newFile.Priority;
                                file.Size = newFile.Size;
                                file.Status = Status.Error;
                            });
                        }
                    }
                    else
                    {
                        currentRealm.Write(() =>
                        {
                            file.Attributes = newFile.Attributes;
                            file.Blocks.Clear();
                            foreach (var b in newFile.Blocks)
                            {
                                file.Blocks.Add(b);
                            }
                            file.BlockStorePath = newFile.BlockStorePath;
                            file.Extension = newFile.Extension;
                            file.Hash = newFile.Hash;
                            file.Index = newFile.Index;
                            file.InternalPath = newFile.InternalPath;
                            file.IsSingleBlock = newFile.IsSingleBlock;
                            file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                            file.Name = newFile.Name;
                            file.Parent = newFile.Parent;
                            file.Path = newFile.Path;
                            file.Priority = newFile.Priority;
                            file.Size = newFile.Size;
                            file.Status = Status.Error;
                        });
                    }
                }
                else
                {
                    currentRealm.Write(() =>
                    {
                        file.Attributes = newFile.Attributes;
                        file.Blocks.Clear();
                        foreach (var b in newFile.Blocks)
                        {
                            file.Blocks.Add(b);
                        }
                        file.BlockStorePath = newFile.BlockStorePath;
                        file.Extension = newFile.Extension;
                        file.Hash = newFile.Hash;
                        file.Index = newFile.Index;
                        file.InternalPath = newFile.InternalPath;
                        file.IsSingleBlock = newFile.IsSingleBlock;
                        file.LastModifiedDateTime = newFile.LastModifiedDateTime;
                        file.Name = newFile.Name;
                        file.Parent = newFile.Parent;
                        file.Path = newFile.Path;
                        file.Priority = newFile.Priority;
                        file.Size = newFile.Size;
                        file.Status = Status.Error;
                    });
                }
            }
        }

        private void DownloadBlock(ThreadSafeReference.Object<Models.Block> blockReference)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            Models.Block block = currentRealm.ResolveReference<Models.Block>(blockReference);
            //TODO: check if deep copy is needed
            Models.Block newBlock = block;

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
                bool isTaskCompleted = false;

                while (!isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(120))
                    {
                        currentRealm.Write(() =>
                        {
                            block.Status = Status.Error;
                        });
                    }

                    if (blockStream.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            try
            {
                using (var fs = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
                {
                    blockStream.Result.CopyTo(fs);
                }
                blockStream.Result.Close();
            }
            catch (Exception e)
            {
                currentRealm.Write(() =>
                {
                    block.Status = Status.Error;
                });
            }

            newBlock = _manager.FileSystemService.UpdateBlockStatus(block);
            currentRealm.Write(() =>
            {
                block.Hash = newBlock.Hash;
                block.Index = newBlock.Index;
                block.InternalPath = newBlock.InternalPath;
                block.Parent = newBlock.Parent;
                block.Path = newBlock.Path;
                block.Priority = newBlock.Priority;
                block.Size = newBlock.Size;
                block.Status = newBlock.Status;
            });
        }

        public bool StartHypermediaDownloading(string path)
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
                Status status = Status.ReadyForDownload;
                if (hypermedia.Status == Status.Completed)
                {
                    status = Status.Seeding;
                }
                currentRealm.Write(() =>
                {
                    hypermedia.Status = status;
                });
                return true;
            }
            return false;
        }

        public bool StopAllDownloads()
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReferences = _manager.RealmService.ReadThreadSafeHypermedias();
            List<Models.Hypermedia> hypermedias = new List<Models.Hypermedia>();
            foreach (var t in threadSafeReferences)
            {
                hypermedias.Add(currentRealm.ResolveReference<Models.Hypermedia>(t));
            }

            bool isAllStopped = true;
            foreach (var h in hypermedias)
            {
                if (h != null)
                {
                    Status status = Status.Stopped;
                    if (h.Status == Status.Seeding)
                    {
                        status = Status.Completed;
                    }
                    currentRealm.Write(() =>
                    {
                        h.Status = status;
                    });
                    isAllStopped = true;
                }
                else
                {
                    isAllStopped = false;
                }
            }
            return isAllStopped;
        }

        public bool StopDownloadService()
        {
            try
            {
                _cancellation.Cancel();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StopHypermediaDownloading(string path)
        {
            if (!_manager.RealmService.IsRealmInitialized())
            {
                throw new NullReferenceException("Realm is not initialized.");
            }
            Realm currentRealm = Realm.GetInstance(_manager.RealmService.GetRealmConfiguration());
            var threadSafeReference = _manager.RealmService.ReadThreadSafeHypermedia(path);
            Models.Hypermedia hypermedia = currentRealm.ResolveReference<Models.Hypermedia>(threadSafeReference);

            if(hypermedia != null)
            {
                Status status = Status.Stopped;
                if(hypermedia.Status == Status.Seeding)
                {
                    status = Status.Completed;
                }
                currentRealm.Write(() =>
                {
                    hypermedia.Status = status;
                });
                return true;
            }
            return false;
        }
    }
}
