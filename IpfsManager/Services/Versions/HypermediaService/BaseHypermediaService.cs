using System;
using System.Collections.Generic;
using System.Text;

using Ipfs.Hypermedia;
using Ipfs.Hypermedia.Tools;
using Ipfs.Hypermedia.Extensions;
using Ipfs.Engine;
using Ipfs.CoreApi;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace Ipfs.Manager.Services.Versions.HypermediaService
{
    class BaseHypermediaService : IHypermediaService
    {
        private Manager _manager;

        public BaseHypermediaService(Manager manager)
        {
            _manager = manager;
        }

        public Hypermedia.Hypermedia GetOrTranslateHypermedia
        (
            string path,
            string name,
            string comment
        )
        {
            return GetOrTranslateHypermediaAsync(path, name, comment).Result;
        }

        public async Task<Hypermedia.Hypermedia> GetOrTranslateHypermediaAsync
        (
            string path,
            string name,
            string comment
        )
        {
            return await GetOrTranslateHypermediaAsync(path, name, comment, string.Empty);
        }

        public Hypermedia.Hypermedia GetOrTranslateHypermedia
        (
            string path,
            string name,
            string comment,
            string extension
        )
        {
            return GetOrTranslateHypermediaAsync(path, name, comment).Result;
        }

        public async Task<Hypermedia.Hypermedia> GetOrTranslateHypermediaAsync
        (
            string path,
            string name,
            string comment,
            string extension
        )
        {
            Hypermedia.Hypermedia hypermedia = null;
            try
            {
                hypermedia = await GetHypermediaFromIPFSLinkAsync(path);
                return hypermedia;
            }
            catch(ArgumentException a) { }

            hypermedia = await TranslateRawIPFSToHypermediaAsync(path, name, comment, extension);
            return hypermedia;
        }

        public Hypermedia.Hypermedia GetHypermediaFromIPFSLink(string path)
        {
            return GetHypermediaFromIPFSLinkAsync(path).Result;
        }

        public async Task<Hypermedia.Hypermedia> GetHypermediaFromIPFSLinkAsync(string path)
        {
            Task<IFileSystemNode> files = null;
            CancellationToken token;
            do
            {
                CancellationTokenSource source = new CancellationTokenSource();
                token = source.Token;
                files = _manager.Engine().FileSystem.ListFileAsync(path, token);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                bool isTimeout = false, isTaskCompleted = false;

                while (!isTimeout && !isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                    {
                        isTimeout = true;
                        if (files.Status != TaskStatus.RanToCompletion)
                        {
                            source.Cancel();
                            break;
                        }
                    }

                    if (files.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            if (files.Result.IsDirectory)
            {
                var result = await IsHypermediaEnabled(files.Result, false);
                if (result.Item1)
                {
                    return result.Item2;
                }
                else
                {
                    throw new ArgumentException($"{nameof(path)} not a valid hypermedia", nameof(path));
                }
            }
            else
            {
                var result = await IsHypermediaEnabled(files.Result, true);
                if (result.Item1)
                {
                    return result.Item2;
                }
                else
                {
                    throw new ArgumentException($"{nameof(path)} not a valid hypermedia", nameof(path));
                }
            }
        }

        public Hypermedia.Hypermedia TranslateRawIPFSToHypermedia
        (
           string path,
           string name,
           string comment
        )
        {
            return TranslateRawIPFSToHypermediaAsync(path, name, comment).Result;
        }

        public async Task<Hypermedia.Hypermedia> TranslateRawIPFSToHypermediaAsync
        (
           string path,
           string name,
           string comment
        )
        {
            return await TranslateRawIPFSToHypermediaAsync(path, name, comment, string.Empty);
        }

        public Hypermedia.Hypermedia TranslateRawIPFSToHypermedia
        (
            string path,
            string name,
            string comment,
            string extension
        )
        {
            return TranslateRawIPFSToHypermediaAsync(path, name, comment, extension).Result;
        }

        public async Task<Hypermedia.Hypermedia> TranslateRawIPFSToHypermediaAsync
        (
            string path,
            string name,
            string comment,
            string extension
        )
        {
            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010()
            {
                Name = name,
                Comment = comment,
                Encoding = Encoding.UTF8,
                CreatedDateTime = DateTime.Now,
                IsRawIPFS = true,
                CreatedBy = "hypermedia-loader"
            };

            Task<IFileSystemNode> files = null;
            CancellationToken token;
            do
            {
                CancellationTokenSource source = new CancellationTokenSource();
                token = source.Token;
                files = _manager.Engine().FileSystem.ListFileAsync(path, token);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                bool isTimeout = false, isTaskCompleted = false;

                while (!isTimeout && !isTaskCompleted)
                {
                    if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                    {
                        isTimeout = true;
                        if (files.Status != TaskStatus.RanToCompletion)
                        {
                            source.Cancel();
                            break;
                        }
                    }

                    if (files.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            if (files.Result.IsDirectory)
            {
                hypermedia.IsDirectoryWrapped = true;
                hypermedia.Entities = await RecursiveSearchOfEntitiesAsync(await files, hypermedia);
                hypermedia.Size = GetRawHypermediaSize(hypermedia);
            }
            else
            {
                //If raw IPFS link is non directory wrapped - errors may occur
                if (files.Result.Links.Count() <= 0) //TODO test it on non-folder object or folder->folder object and change logic
                {
                    hypermedia.IsDirectoryWrapped = false;
                    Hypermedia.File file = new Hypermedia.File
                    {
                        Path = files.Result.Id,
                        Name = name,
                        Extension = extension,
                        IsSingleBlock = true,
                        Attributes = System.IO.FileAttributes.Normal,
                        LastModifiedDateTime = DateTime.Now,
                        Size = (ulong)files.Result.Size
                    };
                    file.SetHash(await GetFileContent(file));
                    hypermedia.Entities.AddWithParent(file, hypermedia);
                    hypermedia.Size = GetRawHypermediaSize(hypermedia);
                }
                else
                {
                    Hypermedia.File file = new Hypermedia.File
                    {
                        Path = files.Result.Id,
                        Name = name,
                        Extension = extension,
                        IsSingleBlock = false,
                        Attributes = System.IO.FileAttributes.Normal,
                        LastModifiedDateTime = DateTime.Now,
                        Size = (ulong)files.Result.Size
                    };
                    file.Blocks = await ExtractBlocksAsync(files.Result, file);
                    file.SetHash();
                    hypermedia.Entities.AddWithParent(file, hypermedia);
                    hypermedia.Size = GetRawHypermediaSize(hypermedia);
                }
            }
            hypermedia.SetHash();
            hypermedia.SetTopic();
            return hypermedia;
        }

        public string TranslateHypermediaToIPFSLink(Hypermedia.Hypermedia hypermedia)
        {
            if(hypermedia.IsDirectoryWrapped)
            {
                return hypermedia.Path;
            }
            else
            {
                throw new ArgumentException("Only directory wrapped entities can be translated to IPFS link with metadata preservation", nameof(hypermedia.IsDirectoryWrapped));
            }
        }

        public async Task<string> TranslateHypermediaToIPFSLinkAsync(Hypermedia.Hypermedia hypermedia)
        {
            return await Task.Run(() =>
            {
                return TranslateHypermediaToIPFSLink(hypermedia);
            });
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(List<Hypermedia.Hypermedia> hypermediaChain)
        {
            return await Task.Run(() =>
            {
                return ConstructWrappedHypermedia(hypermediaChain);
            });
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia(List<Hypermedia.Hypermedia> hypermediaChain)
        {
            if (hypermediaChain.Count < 2)
            {
                throw new ArgumentException($"Number of entities inside {nameof(hypermediaChain)} list must be greater than 1", nameof(hypermediaChain.Count));
            }
            for (int i = 0; i < hypermediaChain.Count; ++i)
            {
                if (i < hypermediaChain.Count - 1)
                {
                    hypermediaChain[i].Entities.AddWithParent(hypermediaChain[i + 1], hypermediaChain[i]);
                }
                else
                {
                    break;
                }
            }
            return hypermediaChain[0];
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(Hypermedia.Hypermedia outerHypermedia, Hypermedia.Hypermedia innerHypermedia)
        {
            return await Task.Run(() =>
            {
                return ConstructWrappedHypermedia(outerHypermedia, innerHypermedia);
            });
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia(Hypermedia.Hypermedia outerHypermedia, Hypermedia.Hypermedia innerHypermedia)
        {
            outerHypermedia.Entities.AddWithParent(innerHypermedia, outerHypermedia);
            return outerHypermedia;
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(Hypermedia.Hypermedia outerHypermedia, string pathToInnerHypermedia)
        {
            Hypermedia.Hypermedia inner = await GetHypermediaFromIPFSLinkAsync(pathToInnerHypermedia);

            outerHypermedia.Entities.AddWithParent(inner, outerHypermedia);
            return outerHypermedia;
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia(Hypermedia.Hypermedia outerHypermedia, string pathToInnerHypermedia)
        {
            return ConstructWrappedHypermediaAsync(outerHypermedia, pathToInnerHypermedia).Result;
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(string pathToOuterHypermedia, string pathToInnerHypermedia)
        {
            Hypermedia.Hypermedia outer = await GetHypermediaFromIPFSLinkAsync(pathToOuterHypermedia);
            Hypermedia.Hypermedia inner = await GetHypermediaFromIPFSLinkAsync(pathToInnerHypermedia);

            outer.Entities.AddWithParent(inner, outer);
            return outer;
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia(string pathToOuterHypermedia, string pathToInnerHypermedia)
        {
            return ConstructWrappedHypermediaAsync(pathToOuterHypermedia, pathToInnerHypermedia).Result;
        }

        public async Task<Models.Hypermedia> ConvertToRealmModelAsync(Hypermedia.Hypermedia hypermedia, string downloadPath, long queuePosition)
        {
            return await Task.Run(() =>
            {
                return ConvertToRealmModel(hypermedia, downloadPath, queuePosition);
            });
        }

        public Models.Hypermedia ConvertToRealmModel(Hypermedia.Hypermedia hypermedia, string downloadPath, long queuePosition)
        {
            return ConvertToRealmModel(hypermedia, downloadPath, false, false, queuePosition, Models.WrappingOptions.SubDirectory, Models.Priority.Normal, Models.Status.Checking);
        }

        public async Task<Models.Hypermedia> ConvertToRealmModelAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            long queuePosition,
            Models.WrappingOptions wrappingOptions,
            Models.Priority priority,
            Models.Status status
        )
        {
            return await Task.Run(() =>
            {
                return ConvertToRealmModel(hypermedia, downloadPath, isAttributesPreservationEnabled, isContinuousDownloadingEnabled, queuePosition, wrappingOptions, priority, status);
            });
        }

        public Models.Hypermedia ConvertToRealmModel
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            bool isAttributesPreservationEnabled,
            bool isContinuousDownloadingEnabled,
            long queuePosition,
            Models.WrappingOptions wrappingOptions,
            Models.Priority priority,
            Models.Status status
        )
        {
            if(!(hypermedia.Parent is null))
            {
                throw new ArgumentException("Only parentless hypermedias can be converted", nameof(hypermedia));
            }

            string path = string.Empty;
            if (wrappingOptions == Models.WrappingOptions.SubDirectory)
            {
                path = System.IO.Path.Combine(downloadPath, hypermedia.Name);
            }
            else
            {
                path = downloadPath;
            }
            //TODO add logic for download location verification according to WrappingOptions (possibly in download service)
            Models.Hypermedia realmHypermedia = new Models.Hypermedia
            {
                Path = hypermedia.Path,
                Name = hypermedia.Name,
                Comment = hypermedia.Comment,
                Encoding = hypermedia.Encoding,
                Created = hypermedia.CreatedDateTime,
                Added = DateTimeOffset.UtcNow,
                Completed = null,
                CreatedBy = hypermedia.CreatedBy,
                CreatorPeer = hypermedia.CreatorPeer,
                IsAttributesPreservationEnabled = isAttributesPreservationEnabled,
                IsContinuousDownloadingEnabled = isContinuousDownloadingEnabled,
                Topic = hypermedia.Topic,
                DefaultSeedingMessage = hypermedia.DefaultSeedingMessage,
                DefaultSubscriptionMessage = hypermedia.DefaultSubscriptionMessage,
                Size = (long)hypermedia.Size,
                InternalPath = path,
                Priority = priority,
                Status = status,
                WrappingOptions = wrappingOptions,
                Hash = hypermedia.Hash,
                Progress = 0.0,
                QueuePosition = queuePosition,
                //TODO add database logic to add into list of hypermedias only hypermedias without parent
                Parent = null,
                Version = hypermedia.Version
            };

            foreach (var e in hypermedia.Entities)
            {
                if (e is Hypermedia.Directory)
                {
                    realmHypermedia.Directories.Add(ConvertToRealmModelDirectory(e as Hypermedia.Directory, priority, status, realmHypermedia));
                }
                else if (e is Hypermedia.File)
                {
                    realmHypermedia.Files.Add(ConvertToRealmModelFile(e as Hypermedia.File, priority, status, realmHypermedia));
                }
                else if (e is Hypermedia.Hypermedia)
                {
                    realmHypermedia.Hypermedias.Add(ConvertToRealmModelHypermedia(e as Hypermedia.Hypermedia, priority, status, realmHypermedia));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(hypermedia.Entities));
                }
            }

            return realmHypermedia;
        }

        private Models.Hypermedia ConvertToRealmModelHypermedia
        (
            Hypermedia.Hypermedia hypermedia,
            Models.Priority priority,
            Models.Status status,
            Models.Hypermedia parent
        )
        {
            if (parent.Path != hypermedia.Parent.Path || parent.Hash != hypermedia.Parent.Hash)
            {
                throw new ArgumentException("Metadata parent and Realm object parent does not match!", nameof(parent));
            }
            string path = string.Empty;
            if(parent.WrappingOptions == Models.WrappingOptions.SubDirectory)
            {
                path = System.IO.Path.Combine(parent.InternalPath, hypermedia.Name);
            }
            else
            {
                path = parent.InternalPath;
            }

            Models.Hypermedia realmHypermedia = new Models.Hypermedia
            {
                Path = hypermedia.Path,
                Name = hypermedia.Name,
                Comment = hypermedia.Comment,
                Encoding = hypermedia.Encoding,
                Created = hypermedia.CreatedDateTime,
                Added = DateTimeOffset.UtcNow,
                Completed = null,
                CreatedBy = hypermedia.CreatedBy,
                CreatorPeer = hypermedia.CreatorPeer,
                IsAttributesPreservationEnabled = parent.IsAttributesPreservationEnabled,
                IsContinuousDownloadingEnabled = parent.IsContinuousDownloadingEnabled,
                Topic = hypermedia.Topic,
                DefaultSeedingMessage = hypermedia.DefaultSeedingMessage,
                DefaultSubscriptionMessage = hypermedia.DefaultSubscriptionMessage,
                Size = (long)hypermedia.Size,
                InternalPath = path,
                Priority = priority,
                Status = status,
                WrappingOptions = parent.WrappingOptions,
                Hash = hypermedia.Hash,
                Progress = 0.0,
                QueuePosition = parent.QueuePosition,
                Parent = parent,
                Version = hypermedia.Version
            };

            foreach(var e in hypermedia.Entities)
            {
                if (e is Hypermedia.Directory)
                {
                    realmHypermedia.Directories.Add(ConvertToRealmModelDirectory(e as Hypermedia.Directory, priority, status, realmHypermedia));
                }
                else if (e is Hypermedia.File)
                {
                    realmHypermedia.Files.Add(ConvertToRealmModelFile(e as Hypermedia.File, priority, status, realmHypermedia));
                }
                else if(e is Hypermedia.Hypermedia)
                {
                    realmHypermedia.Hypermedias.Add(ConvertToRealmModelHypermedia(e as Hypermedia.Hypermedia, priority, status, realmHypermedia));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(hypermedia.Entities));
                }
            }

            return realmHypermedia;
        }

        private Models.Directory ConvertToRealmModelDirectory
        (
            Hypermedia.Directory directory,
            Models.Priority priority,
            Models.Status status,
            Models.IEntity parent
        )
        {
            if (parent.Path != directory.Parent.Path || parent.Hash != directory.Parent.Hash)
            {
                throw new ArgumentException("Metadata parent and Realm object parent does not match!", nameof(parent));
            }
            string path = string.Empty;
            if (parent is Models.Directory)
            {
                Models.Directory tmpDirectory = parent as Models.Directory;
                path = System.IO.Path.Combine(tmpDirectory.InternalPath, directory.Name);
            }
            else if (parent is Models.Hypermedia)
            {
                Models.Hypermedia hypermedia = parent as Models.Hypermedia;
                path = System.IO.Path.Combine(hypermedia.InternalPath, directory.Name);
            }
            else
            {
                throw new ArgumentException("Unknown parent type", nameof(parent));
            }

            Models.Directory realmDirectory = new Models.Directory
            {
                Path = directory.Path,
                Name = directory.Name,
                Attributes = directory.Attributes,
                InternalPath = path,
                LastModifiedDateTime = directory.LastModifiedDateTime,
                Priority = priority,
                Status = status,
                Size = (long)directory.Size,
                Hash = directory.Hash,
                Parent = parent
            };

            foreach(var se in directory.Entities)
            {
                if(se is Hypermedia.Directory)
                {
                    realmDirectory.Directories.Add(ConvertToRealmModelDirectory(se as Hypermedia.Directory, priority, status, realmDirectory));
                }
                else if(se is Hypermedia.File)
                {
                    realmDirectory.Files.Add(ConvertToRealmModelFile(se as Hypermedia.File, priority, status, realmDirectory));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(directory.Entities));
                }
            }

            return realmDirectory;
        }

        private Models.File ConvertToRealmModelFile
        (
            Hypermedia.File file,
            Models.Priority priority,
            Models.Status status,
            Models.IEntity parent
        )
        {
            if (parent.Path != file.Parent.Path || parent.Hash != file.Parent.Hash)
            {
                throw new ArgumentException("Metadata parent and Realm object parent does not match!", nameof(parent));
            }
            string path = string.Empty;
            if(parent is Models.Directory)
            {
                Models.Directory directory = parent as Models.Directory;
                path = System.IO.Path.Combine(directory.InternalPath, $"{file.Name}.{file.Extension}");
            }
            else if(parent is Models.Hypermedia)
            {
                Models.Hypermedia hypermedia = parent as Models.Hypermedia;
                path = System.IO.Path.Combine(hypermedia.InternalPath, $"{file.Name}.{file.Extension}");
            }
            else
            {
                throw new ArgumentException("Unknown parent type", nameof(parent));
            }

            Models.File realmFile = new Models.File
            {
                Path = file.Path,
                Name = file.Name,
                Extension = file.Extension,
                Attributes = file.Attributes,
                InternalPath = path,
                IsSingleBlock = file.IsSingleBlock,
                LastModifiedDateTime = file.LastModifiedDateTime,
                Priority = priority,
                Status = status,
                Size = (long)file.Size,
                Hash = file.Hash,
                Parent = parent
            };
            if(!realmFile.IsSingleBlock)
            {
                foreach(var b in file.Blocks)
                {
                    realmFile.Blocks.Add(ConvertToRealmModelBlock(b, priority, status, realmFile));
                }
            }
            return realmFile;
        }

        private Models.Block ConvertToRealmModelBlock
        (
            Hypermedia.Block block,
            Models.Priority priority,
            Models.Status status,
            Models.File parent
        )
        {
            if(parent.Path != block.Parent.Path || parent.Hash != block.Parent.Hash)
            {
                throw new ArgumentException("Metadata parent and Realm object parent does not match!", nameof(parent));
            }
            string path = System.IO.Path.Combine(parent.InternalPath, $"{parent.Path}_blocks", $"{block.Path}.block");
            Models.Block realmBlock = new Models.Block
            {
                Path = block.Path,
                Size = (long)block.Size,
                Hash = block.Hash,
                Priority = priority,
                Status = status,
                Parent = parent,
                InternalPath = path
            };
            return realmBlock;
        }

        private async Task<List<IEntity>> RecursiveSearchOfEntitiesAsync(IFileSystemNode node, IEntity parent)
        {
            var links = node.Links;
            List<Hypermedia.IEntity> entities = new List<Hypermedia.IEntity>();
            foreach (var l in links)
            {
                Task<IFileSystemNode> n = null;
                CancellationToken token;

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    n = _manager.Engine().FileSystem.ListFileAsync(l.Id);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(10))
                        {
                            isTimeout = true;
                            if (n.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (n.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                //TODO check (n was l)
                if (n.Result.IsDirectory)
                {
                    if(!(parent is Hypermedia.Hypermedia || parent is Hypermedia.Directory))
                    {
                        throw new ArgumentException("Only hypermedia and directory can be parent for directory", nameof(parent));
                    }
                    Hypermedia.Directory directory = new Hypermedia.Directory
                    {
                        Path = l.Id,
                        Name = l.Name,
                        Attributes = System.IO.FileAttributes.Directory,
                        LastModifiedDateTime = DateTime.Now,
                        Size = (ulong)l.Size,
                        Parent = parent
                    };
                    directory.Entities = await RecursiveSearchOfSystemEntitiesAsync(n.Result, directory);
                    entities.Add(directory);
                }
                else
                {

                    if (n.Result.Links.Count() <= 0)
                    {
                        Hypermedia.File file = new Hypermedia.File
                        {
                            Path = l.Id,
                            Name = System.IO.Path.GetFileNameWithoutExtension(l.Name) is null ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(l.Name),
                            Extension = System.IO.Path.GetExtension(l.Name) is null ? string.Empty : System.IO.Path.GetExtension(l.Name),
                            IsSingleBlock = true,
                            Attributes = System.IO.FileAttributes.Normal,
                            LastModifiedDateTime = DateTime.Now,
                            Size = (ulong)l.Size,
                            Parent = parent
                        };
                        file.SetHash(await GetFileContent(file));
                        entities.Add(file);
                    }
                    else
                    {
                        Hypermedia.File file = new Hypermedia.File
                        {
                            Path = l.Id,
                            Name = System.IO.Path.GetFileNameWithoutExtension(l.Name) is null ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(l.Name),
                            Extension = System.IO.Path.GetExtension(l.Name) is null ? string.Empty : System.IO.Path.GetExtension(l.Name),
                            IsSingleBlock = false,
                            Attributes = System.IO.FileAttributes.Normal,
                            LastModifiedDateTime = DateTime.Now,
                            Size = (ulong)l.Size,
                            Parent = parent
                        };
                        file.Blocks = await ExtractBlocksAsync(n.Result, file);
                        file.SetHash();
                        entities.Add(file);
                    }
                }
            }
            return entities;
        }

        private async Task<List<ISystemEntity>> RecursiveSearchOfSystemEntitiesAsync(IFileSystemNode node, IEntity parent)
        {
            var links = node.Links;
            List<Hypermedia.ISystemEntity> entities = new List<Hypermedia.ISystemEntity>();
            foreach (var l in links)
            {
                Task<IFileSystemNode> n = null;
                CancellationToken token;

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    n = _manager.Engine().FileSystem.ListFileAsync(l.Id);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(10))
                        {
                            isTimeout = true;
                            if (n.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (n.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                //TODO check (n was l)
                if (n.Result.IsDirectory)
                {
                    if (!(parent is Hypermedia.Hypermedia || parent is Hypermedia.Directory))
                    {
                        throw new ArgumentException("Only hypermedia and directory can be parent for directory", nameof(parent));
                    }
                    Hypermedia.Directory directory = new Hypermedia.Directory
                    {
                        Path = l.Id,
                        Name = l.Name,
                        Attributes = System.IO.FileAttributes.Directory,
                        LastModifiedDateTime = DateTime.Now,
                        Size = (ulong)l.Size
                    };
                    directory.Entities = await RecursiveSearchOfSystemEntitiesAsync(n.Result, directory);
                    directory.Parent = parent;
                    entities.Add(directory);
                }
                else
                {

                    if (n.Result.Links.Count() <= 0)
                    {
                        Hypermedia.File file = new Hypermedia.File
                        {
                            Path = l.Id,
                            Name = System.IO.Path.GetFileNameWithoutExtension(l.Name) is null ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(l.Name),
                            Extension = System.IO.Path.GetExtension(l.Name) is null ? string.Empty : System.IO.Path.GetExtension(l.Name),
                            IsSingleBlock = true,
                            Attributes = System.IO.FileAttributes.Normal,
                            LastModifiedDateTime = DateTime.Now,
                            Size = (ulong)l.Size,
                            Parent = parent
                        };
                        file.SetHash(await GetFileContent(file));
                        entities.Add(file);
                    }
                    else
                    {
                        Hypermedia.File file = new Hypermedia.File
                        {
                            Path = l.Id,
                            Name = System.IO.Path.GetFileNameWithoutExtension(l.Name) is null ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(l.Name),
                            Extension = System.IO.Path.GetExtension(l.Name) is null ? string.Empty : System.IO.Path.GetExtension(l.Name),
                            IsSingleBlock = false,
                            Attributes = System.IO.FileAttributes.Normal,
                            LastModifiedDateTime = DateTime.Now,
                            Size = (ulong)l.Size,
                            Parent = parent
                        };
                        file.Blocks = await ExtractBlocksAsync(n.Result, file);
                        file.SetHash();
                        entities.Add(file);
                    }
                }
            }
            return entities;
        }

        private async Task<List<Hypermedia.Block>> ExtractBlocksAsync(IFileSystemNode node, File parent)
        {
            var links = node.Links;
            List<Hypermedia.Block> blocks = new List<Hypermedia.Block>();
            foreach (var l in links)
            {
                Hypermedia.Block block = new Block
                {
                    Path = l.Id,
                    Size = (ulong)l.Size,
                    Parent = parent
                };
                block.SetHash(await GetBlockContent(block));
                blocks.Add(block);
            }
            return blocks;
        }

        private async Task<byte[]> GetFileContent(Hypermedia.File file)
        {
            if(!file.IsSingleBlock)
            {
                throw new ArgumentException("Only single blocked file can be parsed", nameof(file));
            }
            List<byte> content = new List<byte>();
            using (var stream = await _manager.Engine().FileSystem.GetAsync(file.Path, false))
            {
                int tmp = -1;
                do
                {
                    tmp = stream.ReadByte();
                    content.Add((byte)tmp);
                }
                while (tmp != -1);
            }
            if ((ulong)content.Count != file.Size)
            {
                throw new ArgumentException("Unexpected size", nameof(file.Size));
            }
            return content.ToArray();
        }

        private async Task<byte[]> GetBlockContent(Hypermedia.Block block)
        {
            List<byte> content = new List<byte>();
            using (var stream = await _manager.Engine().FileSystem.GetAsync(block.Path, false))
            {
                int tmp = -1;
                do
                {
                    tmp = stream.ReadByte();
                    content.Add((byte)tmp);
                }
                while (tmp != -1);
            }
            if((ulong)content.Count != block.Size)
            {
                throw new ArgumentException("Unexpected size", nameof(block.Size));
            }
            return content.ToArray();
        }

        private async Task<(bool, Hypermedia.Hypermedia)> IsHypermediaEnabled(IFileSystemNode node, bool IsAggressiveParsingEnabled)
        {
            var links = node.Links;
            bool isMetadataFound = false;
            bool isSerializationValid = false;
            Hypermedia.Hypermedia hypermedia = null;

            foreach (var l in links)
            {
                if (l.Name.EndsWith(".hyper"))
                {
                    isMetadataFound = true;
                    try
                    {
                        Task<System.IO.Stream> stream = null;
                        CancellationToken token;

                        do
                        {
                            CancellationTokenSource source = new CancellationTokenSource();
                            token = source.Token;
                            stream = _manager.Engine().FileSystem.GetAsync(l.Id, false);

                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            bool isTimeout = false, isTaskCompleted = false;

                            while (!isTimeout && !isTaskCompleted)
                            {
                                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                                {
                                    isTimeout = true;
                                    if (stream.Status != TaskStatus.RanToCompletion)
                                    {
                                        source.Cancel();
                                        break;
                                    }
                                }

                                if (stream.Status == TaskStatus.RanToCompletion)
                                {
                                    isTaskCompleted = true;
                                    break;
                                }
                            }

                            using (var buffer = stream.Result)
                            {
                                try
                                {
                                    hypermedia = await Hypermedia.Hypermedia.DeserializeAsync(buffer);
                                    isSerializationValid = true;
                                }
                                catch(Exception e)
                                {
                                    isSerializationValid = false;
                                }
                            }
                        }
                        while (token.IsCancellationRequested);
                    }
                    catch (Exception e)
                    {
                        isMetadataFound = false;
                    }
                    break;
                }
            }

            if(!isMetadataFound && IsAggressiveParsingEnabled)
            {
                foreach (var l in links)
                {
                    try
                    {
                        Task<System.IO.Stream> stream = null;
                        CancellationToken token;

                        do
                        {
                            CancellationTokenSource source = new CancellationTokenSource();
                            token = source.Token;
                            stream = _manager.Engine().FileSystem.GetAsync(l.Id, false);

                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            bool isTimeout = false, isTaskCompleted = false;

                            while (!isTimeout && !isTaskCompleted)
                            {
                                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(15))
                                {
                                    isTimeout = true;
                                    if (stream.Status != TaskStatus.RanToCompletion)
                                    {
                                        source.Cancel();
                                        break;
                                    }
                                }

                                if (stream.Status == TaskStatus.RanToCompletion)
                                {
                                    isTaskCompleted = true;
                                    break;
                                }
                            }

                            using (var buffer = stream.Result)
                            {
                                try
                                {
                                    hypermedia = await Hypermedia.Hypermedia.DeserializeAsync(buffer);
                                    isSerializationValid = true;
                                    isMetadataFound = true;
                                    break;
                                }
                                catch (Exception e)
                                {
                                    isSerializationValid = false;
                                    isMetadataFound = false;
                                }
                            }
                        }
                        while (token.IsCancellationRequested);
                        if(isMetadataFound && isSerializationValid)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        isMetadataFound = false;
                    }
                }
            }
            return (isMetadataFound && isSerializationValid, hypermedia);
        }

        private ulong GetRawHypermediaBlocksSize(List<Hypermedia.Block> blocks)
        {
            ulong size = 0;
            foreach (var b in blocks)
            {
                size += b.Size;
            }
            return size;
        }

        private ulong GetRawHypermediaFilesSize(List<Hypermedia.File> files)
        {
            ulong size = 0;
            foreach (var f in files)
            {
                if (f.IsSingleBlock)
                {
                    size += f.Size;
                }
                else
                {
                    size += GetRawHypermediaBlocksSize(f.Blocks);
                }
            }
            return size;
        }

        private ulong GetRawHypermediaDirectoriesSize(List<Hypermedia.Directory> directories)
        {
            ulong size = 0;

            List<Hypermedia.File> files = new List<Hypermedia.File>();
            List<Hypermedia.Directory> folders = new List<Hypermedia.Directory>();

            foreach (var d in directories)
            {
                foreach (var o in d.Entities)
                {
                    if (o is Hypermedia.File)
                    {
                        files.Add(o as Hypermedia.File);
                    }
                    else if (o is Hypermedia.Directory)
                    {
                        folders.Add(o as Hypermedia.Directory);
                    }
                }
            }

            if (files.Count > 0)
            {
                size += GetRawHypermediaFilesSize(files);
            }
            if (folders.Count > 0)
            {
                size += GetRawHypermediaDirectoriesSize(folders);
            }

            return size;
        }

        private ulong GetRawHypermediasSize(List<Hypermedia.Hypermedia> hypermedia)
        {
            ulong size = 0;

            List<Hypermedia.File> files = new List<Hypermedia.File>();
            List<Hypermedia.Directory> directories = new List<Hypermedia.Directory>();
            List<Hypermedia.Hypermedia> hypermedias = new List<Hypermedia.Hypermedia>();

            foreach(var h in hypermedia) 
            {
                foreach (var o in h.Entities)
                {
                    if (o is Hypermedia.File)
                    {
                        files.Add(o as Hypermedia.File);
                    }
                    else if (o is Hypermedia.Directory)
                    {
                        directories.Add(o as Hypermedia.Directory);
                    }
                    else if (o is Hypermedia.Hypermedia)
                    {
                        hypermedias.Add(o as Hypermedia.Hypermedia);
                    }
                }
            }
            if (files.Count > 0)
            {
                size += GetRawHypermediaFilesSize(files);
            }
            if (directories.Count > 0)
            {
                size += GetRawHypermediaDirectoriesSize(directories);
            }
            if (hypermedias.Count > 0)
            {
                size += GetRawHypermediasSize(hypermedias);
            }

            return size;
        }

        private ulong GetRawHypermediaSize(Hypermedia.Hypermedia hypermedia)
        {
            return GetRawHypermediasSize(new List<Hypermedia.Hypermedia>() { hypermedia });
        }
    }
}
