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
using Ipfs.Manager.Tools.Options;

namespace Ipfs.Manager.Services.Versions.HypermediaService
{
    class BaseHypermediaService : IHypermediaService
    {
        private Manager _manager;

        public BaseHypermediaService(Manager manager)
        {
            _manager = manager;
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
            var result = await IsHypermediaEnabled(files.Result, files.Result.IsDirectory ? false : true);
            if (result.Item1)
            {
                return result.Item2;
            }
            else
            {
                throw new ArgumentException($"{nameof(path)} not a valid hypermedia", nameof(path));
            }
        }

        public string TranslateHypermediaToIPFSLink(Hypermedia.Hypermedia hypermedia)
        {
            if(string.IsNullOrEmpty(hypermedia.Path) || string.IsNullOrWhiteSpace(hypermedia.Path))
            {
                return CreateHypermediaWithPath(hypermedia).Path;
            }
            else
            {
                return hypermedia.Path;
            }
        }

        public async Task<string> TranslateHypermediaToIPFSLinkAsync(Hypermedia.Hypermedia hypermedia)
        {
            if (string.IsNullOrEmpty(hypermedia.Path) || string.IsNullOrWhiteSpace(hypermedia.Path))
            {
                return (await CreateHypermediaWithPathAsync(hypermedia)).Path;
            }
            else
            {
                return hypermedia.Path;
            }
        }

        public Hypermedia.Hypermedia CreateHypermediaWithPath(Hypermedia.Hypermedia hypermedia)
        {
            Task<IFileSystemNode> file;
            CancellationToken token;

            using (var stream = new System.IO.MemoryStream())
            {
                Hypermedia.Hypermedia.Serialize(stream, hypermedia);

                BlockSizeOptions options = BlockSizeOptions.kB_128;
                if (stream.Length < (int)BlockSizeOptions.kB_128 || (stream.Length >= (int)BlockSizeOptions.kB_128 && stream.Length < (int)BlockSizeOptions.kB_256))
                {
                    options = BlockSizeOptions.kB_128;
                }
                else if (stream.Length >= (int)BlockSizeOptions.kB_256 && stream.Length < (int)BlockSizeOptions.kB_512)
                {
                    options = BlockSizeOptions.kB_256;
                }
                else if (stream.Length >= (int)BlockSizeOptions.kB_512 && stream.Length < (int)BlockSizeOptions.MB_1)
                {
                    options = BlockSizeOptions.kB_512;
                }
                else if (stream.Length >= (int)BlockSizeOptions.MB_1 && stream.Length < (int)BlockSizeOptions.MB_2)
                {
                    options = BlockSizeOptions.MB_1;
                }
                else if (stream.Length >= (int)BlockSizeOptions.MB_2)
                {
                    options = BlockSizeOptions.MB_2;
                }

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if works. And check if name created correctly
                    file = _manager.Engine().FileSystem.AddAsync(
                        stream,
                        "metadata.hyper",
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = true,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)options
                        }
                    );

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromMinutes(1))
                        {
                            isTimeout = true;
                            if (file.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (file.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
            }
            hypermedia.Path = file.Result.Id;
            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> CreateHypermediaWithPathAsync(Hypermedia.Hypermedia hypermedia)
        {
            Task<IFileSystemNode> file;
            CancellationToken token;

            using (var stream = new System.IO.MemoryStream())
            {
                await Hypermedia.Hypermedia.SerializeAsync(stream, hypermedia);

                BlockSizeOptions options = BlockSizeOptions.kB_128;
                if (stream.Length < (int)BlockSizeOptions.kB_128 || (stream.Length >= (int)BlockSizeOptions.kB_128 && stream.Length < (int)BlockSizeOptions.kB_256))
                {
                    options = BlockSizeOptions.kB_128;
                }
                else if (stream.Length >= (int)BlockSizeOptions.kB_256 && stream.Length < (int)BlockSizeOptions.kB_512)
                {
                    options = BlockSizeOptions.kB_256;
                }
                else if (stream.Length >= (int)BlockSizeOptions.kB_512 && stream.Length < (int)BlockSizeOptions.MB_1)
                {
                    options = BlockSizeOptions.kB_512;
                }
                else if (stream.Length >= (int)BlockSizeOptions.MB_1 && stream.Length < (int)BlockSizeOptions.MB_2)
                {
                    options = BlockSizeOptions.MB_1;
                }
                else if (stream.Length >= (int)BlockSizeOptions.MB_2)
                {
                    options = BlockSizeOptions.MB_2;
                }

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if works. And check if name created correctly
                    file = _manager.Engine().FileSystem.AddAsync(
                        stream,
                        "metadata.hyper",
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = true,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)options
                        }
                    );

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromMinutes(1))
                        {
                            isTimeout = true;
                            if (file.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (file.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
            }
            hypermedia.Path = file.Result.Id;
            return hypermedia;
        }

        public Hypermedia.Hypermedia CreateHypermediaWithPath
        (
            Hypermedia.Hypermedia hypermedia,
            BlockSizeOptions options,
            string key
        )
        {
            Task<IFileSystemNode> file;
            CancellationToken token;

            using (var stream = new System.IO.MemoryStream()) 
            {
                Hypermedia.Hypermedia.Serialize(stream, hypermedia);
                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if works. And check if name created correctly
                    file = _manager.Engine().FileSystem.AddAsync(
                        stream,
                        "metadata.hyper", 
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = true,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)options,
                            ProtectionKey = key
                        }
                    );

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromMinutes(1))
                        {
                            isTimeout = true;
                            if (file.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (file.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
            }
            hypermedia.Path = file.Result.Id;
            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> CreateHypermediaWithPathAsync
        (
            Hypermedia.Hypermedia hypermedia,
            BlockSizeOptions options,
            string key
        )
        {
            Task<IFileSystemNode> file;
            CancellationToken token;

            using (var stream = new System.IO.MemoryStream())
            {
                await Hypermedia.Hypermedia.SerializeAsync(stream, hypermedia);
                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if works. And check if name created correctly
                    file = _manager.Engine().FileSystem.AddAsync(
                        stream,
                        "metadata.hyper", 
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = true,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)options,
                            ProtectionKey = key
                        }
                    );

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    bool isTimeout = false, isTaskCompleted = false;

                    while (!isTimeout && !isTaskCompleted)
                    {
                        if (stopwatch.Elapsed >= TimeSpan.FromMinutes(1))
                        {
                            isTimeout = true;
                            if (file.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (file.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
            }
            hypermedia.Path = file.Result.Id;
            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync(List<Hypermedia.Hypermedia> hypermediaChain)
        {
            if (hypermediaChain.Count < 2)
            {
                throw new ArgumentException($"Number of entities inside {nameof(hypermediaChain)} list must be greater than 1", nameof(hypermediaChain.Count));
            }
            //TODO: Check if works
            for (int i = 0; i < hypermediaChain.Count - 1; ++i)
            {
                if (string.IsNullOrEmpty(hypermediaChain[i].Path) || string.IsNullOrWhiteSpace(hypermediaChain[i].Path))
                {
                    hypermediaChain[i] = await CreateHypermediaWithPathAsync(hypermediaChain[i]);
                }
                if (string.IsNullOrEmpty(hypermediaChain[i + 1].Path) || string.IsNullOrWhiteSpace(hypermediaChain[i + 1].Path))
                {
                    hypermediaChain[i + 1] = await CreateHypermediaWithPathAsync(hypermediaChain[i + 1]);
                }
                hypermediaChain[i].Entities.AddWithParent(hypermediaChain[i + 1], hypermediaChain[i]);
            }
            return hypermediaChain[0];
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia(List<Hypermedia.Hypermedia> hypermediaChain)
        {
            if (hypermediaChain.Count < 2)
            {
                throw new ArgumentException($"Number of entities inside {nameof(hypermediaChain)} list must be greater than 1", nameof(hypermediaChain.Count));
            }
            //TODO: Check if works
            for (int i = 0; i < hypermediaChain.Count - 1; ++i)
            {
                if (string.IsNullOrEmpty(hypermediaChain[i].Path) || string.IsNullOrWhiteSpace(hypermediaChain[i].Path))
                {
                    hypermediaChain[i] = CreateHypermediaWithPath(hypermediaChain[i]);
                }
                if (string.IsNullOrEmpty(hypermediaChain[i + 1].Path) || string.IsNullOrWhiteSpace(hypermediaChain[i + 1].Path))
                {
                    hypermediaChain[i + 1] = CreateHypermediaWithPath(hypermediaChain[i + 1]);
                }
                hypermediaChain[i].Entities.AddWithParent(hypermediaChain[i + 1], hypermediaChain[i]);
            }
            return hypermediaChain[0];
        }

        public async Task<Hypermedia.Hypermedia> ConstructWrappedHypermediaAsync
        (
            Hypermedia.Hypermedia outerHypermedia,
            Hypermedia.Hypermedia innerHypermedia
        )
        {
            return await ConstructWrappedHypermediaAsync(new List<Hypermedia.Hypermedia> { outerHypermedia, innerHypermedia });
        }

        public Hypermedia.Hypermedia ConstructWrappedHypermedia
        (
            Hypermedia.Hypermedia outerHypermedia,
            Hypermedia.Hypermedia innerHypermedia
        )
        {
            return ConstructWrappedHypermedia(new List<Hypermedia.Hypermedia> { outerHypermedia, innerHypermedia });
        }

        public async Task<Models.Hypermedia> ConvertToRealmModelAsync
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            long queuePosition
        )
        {
            return await Task.Run(() =>
            {
                return ConvertToRealmModel(hypermedia, downloadPath, queuePosition);
            });
        }

        public Models.Hypermedia ConvertToRealmModel
        (
            Hypermedia.Hypermedia hypermedia,
            string downloadPath,
            long queuePosition
        )
        {
            return ConvertToRealmModel
            (
                hypermedia,
                downloadPath,
                false,
                false,
                queuePosition,
                Models.WrappingOptions.SubDirectory,
                Models.Priority.Normal, 
                Models.Status.Checking
            );
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
                return ConvertToRealmModel
                (
                    hypermedia,
                    downloadPath,
                    isAttributesPreservationEnabled,
                    isContinuousDownloadingEnabled,
                    queuePosition,
                    wrappingOptions,
                    priority,
                    status
                );
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
                ProgressRaw = 0.0,
                QueuePosition = queuePosition,
                Parent = null,
                Version = hypermedia.Version,
                Index = -1
            };

            long index = 0;

            foreach (var e in hypermedia.Entities)
            {
                if (e is Hypermedia.Directory)
                {
                    realmHypermedia.Directories.Add(ConvertToRealmModelDirectory(e as Hypermedia.Directory, priority, status, realmHypermedia, index));
                }
                else if (e is Hypermedia.File)
                {
                    realmHypermedia.Files.Add(ConvertToRealmModelFile(e as Hypermedia.File, priority, status, realmHypermedia, index));
                }
                else if (e is Hypermedia.Hypermedia)
                {
                    realmHypermedia.Hypermedias.Add(ConvertToRealmModelHypermedia(e as Hypermedia.Hypermedia, priority, status, realmHypermedia, index));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(hypermedia.Entities));
                }
                ++index;
            }

            return realmHypermedia;
        }

        private Models.Hypermedia ConvertToRealmModelHypermedia
        (
            Hypermedia.Hypermedia hypermedia,
            Models.Priority priority,
            Models.Status status,
            Models.Hypermedia parent,
            long index
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
                ProgressRaw = 0.0,
                QueuePosition = parent.QueuePosition,
                Parent = parent,
                Version = hypermedia.Version,
                Index = index
            };

            long innerIndex = 0;

            foreach(var e in hypermedia.Entities)
            {
                if (e is Hypermedia.Directory)
                {
                    realmHypermedia.Directories.Add(ConvertToRealmModelDirectory(e as Hypermedia.Directory, priority, status, realmHypermedia, innerIndex));
                }
                else if (e is Hypermedia.File)
                {
                    realmHypermedia.Files.Add(ConvertToRealmModelFile(e as Hypermedia.File, priority, status, realmHypermedia, innerIndex));
                }
                else if(e is Hypermedia.Hypermedia)
                {
                    realmHypermedia.Hypermedias.Add(ConvertToRealmModelHypermedia(e as Hypermedia.Hypermedia, priority, status, realmHypermedia, innerIndex));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(hypermedia.Entities));
                }
                ++innerIndex;
            }

            return realmHypermedia;
        }

        private Models.Directory ConvertToRealmModelDirectory
        (
            Hypermedia.Directory directory,
            Models.Priority priority,
            Models.Status status,
            Models.IEntity parent,
            long index
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
                ProgressRaw = 0.0,
                InternalPath = path,
                LastModifiedDateTime = directory.LastModifiedDateTime,
                Priority = priority,
                Status = status,
                Size = (long)directory.Size,
                Hash = directory.Hash,
                Parent = parent,
                Index = index
            };

            long innerIndex = 0;

            foreach (var se in directory.Entities)
            {
                if(se is Hypermedia.Directory)
                {
                    realmDirectory.Directories.Add(ConvertToRealmModelDirectory(se as Hypermedia.Directory, priority, status, realmDirectory, innerIndex));
                }
                else if(se is Hypermedia.File)
                {
                    realmDirectory.Files.Add(ConvertToRealmModelFile(se as Hypermedia.File, priority, status, realmDirectory, innerIndex));
                }
                else
                {
                    throw new ArgumentException("Unknow entity type", nameof(directory.Entities));
                }
                ++innerIndex;
            }

            return realmDirectory;
        }

        private Models.File ConvertToRealmModelFile
        (
            Hypermedia.File file,
            Models.Priority priority,
            Models.Status status,
            Models.IEntity parent,
            long index
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

            string blockStore = string.Empty;
            if (parent is Models.Directory)
            {
                Models.Directory directory = parent as Models.Directory;
                blockStore = System.IO.Path.Combine(directory.InternalPath, $"{file.Path}_blocks");
            }
            else if (parent is Models.Hypermedia)
            {
                Models.Hypermedia hypermedia = parent as Models.Hypermedia;
                blockStore = System.IO.Path.Combine(hypermedia.InternalPath, $"{file.Path}_blocks");
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
                ProgressRaw = 0.0,
                InternalPath = path,
                BlockStorePath = blockStore,
                IsSingleBlock = file.IsSingleBlock,
                LastModifiedDateTime = file.LastModifiedDateTime,
                Priority = priority,
                Status = status,
                Size = (long)file.Size,
                Hash = file.Hash,
                Parent = parent,
                Index = index
            };

            long innerIndex = 0;

            if (!realmFile.IsSingleBlock)
            {
                foreach(var b in file.Blocks)
                {
                    realmFile.Blocks.Add(ConvertToRealmModelBlock(b, priority, status, realmFile, innerIndex));
                    ++innerIndex;
                }
            }
            return realmFile;
        }

        private Models.Block ConvertToRealmModelBlock
        (
            Hypermedia.Block block,
            Models.Priority priority,
            Models.Status status,
            Models.File parent,
            long index
        )
        {
            if(parent.Path != block.Parent.Path || parent.Hash != block.Parent.Hash)
            {
                throw new ArgumentException("Metadata parent and Realm object parent does not match!", nameof(parent));
            }
            string path = System.IO.Path.Combine(parent.BlockStorePath, $"{block.Path}.block");
            Models.Block realmBlock = new Models.Block
            {
                Path = block.Path,
                Size = (long)block.Size,
                Hash = block.Hash,
                Priority = priority,
                Status = status,
                Parent = parent,
                InternalPath = path,
                Index = index
            };
            return realmBlock;
        }

        private async Task<(bool, Hypermedia.Hypermedia)> IsHypermediaEnabled(IFileSystemNode node, bool IsAggressiveParsingEnabled)
        {
            bool isMetadataFound = false;
            bool isSerializationValid = false;
            Hypermedia.Hypermedia hypermedia = null;

            if(node.IsDirectory)
            {
                var links = node.Links;
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
                                    catch (Exception e)
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

                if (!isMetadataFound && IsAggressiveParsingEnabled)
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
                            if (isMetadataFound && isSerializationValid)
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
            }
            else
            {
                if (IsAggressiveParsingEnabled)
                {
                    try
                    {
                        Task<System.IO.Stream> stream = null;
                        CancellationToken token;

                        do
                        {
                            CancellationTokenSource source = new CancellationTokenSource();
                            token = source.Token;
                            stream = _manager.Engine().FileSystem.GetAsync(node.Id, false);

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
                    }
                    catch (Exception e)
                    {
                        isMetadataFound = false;
                    }
                }
                else
                {
                    return (false, null);
                }
            }
           
            return (isMetadataFound && isSerializationValid, hypermedia);
        }
    }
}
