using System;
using System.Collections.Generic;
using System.Text;

using Ipfs.Engine;
using Ipfs.CoreApi;
using Ipfs.Manager.Tools.Options;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Ipfs.Manager.Tools;
using Ipfs.Hypermedia.Extensions;

namespace Ipfs.Manager.Services.Versions.UploadService
{
    //TODO: Add validation for / at the end of path in FileInfo/DirectoryInfo
    class BaseUploadService : IUploadService
    {
        private Manager _manager;

        public BaseUploadService(Manager manager)
        {
            _manager = manager;
        }

        public Hypermedia.Hypermedia UploadDirectory(string path, string name)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Directory doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = GetDirectorySize(new System.IO.DirectoryInfo(path), true)
            };

            hypermedia.Entities.AddWithParent(UploadDirectory(path, true), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }

        private Hypermedia.Directory UploadDirectory(string path, bool recursive)
        {
            var di = new System.IO.DirectoryInfo(path);
            Hypermedia.Directory directory = new Hypermedia.Directory
            {
                Name = di.Name,
                Attributes = di.Attributes,
                LastModifiedDateTime = di.LastWriteTime,
                Size = GetDirectorySize(di, recursive)
            };

            if (recursive)
            {
                System.IO.DirectoryInfo[] directoryInfos = di.GetDirectories();
                foreach (var d in directoryInfos)
                {
                    directory.Entities.AddWithParent(UploadDirectory(d.FullName, recursive), directory);
                }
            }

            System.IO.FileInfo[] fileInfos = di.GetFiles();
            foreach (var f in fileInfos)
            {
                directory.Entities.AddWithParent(UploadFile(f.FullName), directory);
            }

            directory.SetHash();
            directory.Path = $"HD{directory.Hash}";

            return directory;
        }

        private async Task<Hypermedia.Directory> UploadDirectoryAsync(string path, bool recursive)
        {
            var di = new System.IO.DirectoryInfo(path);
            Hypermedia.Directory directory = new Hypermedia.Directory
            {
                Name = di.Name,
                Attributes = di.Attributes,
                LastModifiedDateTime = di.LastWriteTime,
                Size = GetDirectorySize(di, recursive)
            };

            if (recursive)
            {
                System.IO.DirectoryInfo[] directoryInfos = di.GetDirectories();
                foreach (var d in directoryInfos)
                {
                    directory.Entities.AddWithParent(await UploadDirectoryAsync(d.FullName, recursive), directory);
                }
            }

            System.IO.FileInfo[] fileInfos = di.GetFiles();
            foreach (var f in fileInfos)
            {
                directory.Entities.AddWithParent(await UploadFileAsync(f.FullName), directory);
            }

            directory.SetHash();
            directory.Path = $"HD{directory.Hash}";

            return directory;
        }

        private Hypermedia.Directory UploadDirectory(string path, bool recursive, BlockSizeOptions options)
        {
            var di = new System.IO.DirectoryInfo(path);
            Hypermedia.Directory directory = new Hypermedia.Directory
            {
                Name = di.Name,
                Attributes = di.Attributes,
                LastModifiedDateTime = di.LastWriteTime,
                Size = GetDirectorySize(di, recursive)
            };

            if (recursive)
            {
                System.IO.DirectoryInfo[] directoryInfos = di.GetDirectories();
                foreach (var d in directoryInfos)
                {
                    directory.Entities.AddWithParent(UploadDirectory(d.FullName, recursive, options), directory);
                }
            }

            System.IO.FileInfo[] fileInfos = di.GetFiles();
            foreach (var f in fileInfos)
            {
                directory.Entities.AddWithParent(UploadFile(f.FullName, options), directory);
            }

            directory.SetHash();
            directory.Path = $"HD{directory.Hash}";

            return directory;
        }

        private async Task<Hypermedia.Directory> UploadDirectoryAsync(string path, bool recursive, BlockSizeOptions options)
        {
            var di = new System.IO.DirectoryInfo(path);
            Hypermedia.Directory directory = new Hypermedia.Directory
            {
                Name = di.Name,
                Attributes = di.Attributes,
                LastModifiedDateTime = di.LastWriteTime,
                Size = GetDirectorySize(di, recursive)
            };

            if (recursive)
            {
                System.IO.DirectoryInfo[] directoryInfos = di.GetDirectories();
                foreach (var d in directoryInfos)
                {
                    directory.Entities.AddWithParent(await UploadDirectoryAsync(d.FullName, recursive, options), directory);
                }
            }

            System.IO.FileInfo[] fileInfos = di.GetFiles();
            foreach (var f in fileInfos)
            {
                directory.Entities.AddWithParent(await UploadFileAsync(f.FullName, options), directory);
            }

            directory.SetHash();
            directory.Path = $"HD{directory.Hash}";

            return directory;
        }


        private ulong GetDirectorySize(System.IO.DirectoryInfo directoryInfo, bool recursive)
        {
            ulong size = 0;
            System.IO.FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach(var f in fileInfos)
            {
                size += (ulong)f.Length;
            }

            if (recursive)
            {
                System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (var d in directoryInfos)
                {
                    size += GetDirectorySize(d, recursive);
                }
            }

            return size;
        }

        public Hypermedia.Hypermedia UploadDirectory
        (
            string path,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        )
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Directory doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = GetDirectorySize(new System.IO.DirectoryInfo(path), recursive)
            };

            hypermedia.Entities.AddWithParent(UploadDirectory(path, recursive, options), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> UploadDirectoryAsync(string path, string name)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Directory doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = GetDirectorySize(new System.IO.DirectoryInfo(path), true)
            };

            hypermedia.Entities.AddWithParent(await UploadDirectoryAsync(path, true), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> UploadDirectoryAsync
        (
            string path,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        )
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Directory doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = GetDirectorySize(new System.IO.DirectoryInfo(path), recursive)
            };

            hypermedia.Entities.AddWithParent(await UploadDirectoryAsync(path, recursive, options), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public Hypermedia.Hypermedia UploadFile(string path, string name)
        {
            if(!System.IO.File.Exists(path))
            {
                throw new ArgumentException("File doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = (ulong)new System.IO.FileInfo(path).Length,
            };

            hypermedia.Entities.AddWithParent(UploadFile(path), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }

        private Hypermedia.File UploadFile(string path)
        {
            var fi = new System.IO.FileInfo(path);
            Hypermedia.File file = new Hypermedia.File
            {
                Name = fi.Name,
                Extension = fi.Extension,
                Attributes = fi.Attributes,
                LastModifiedDateTime = fi.LastWriteTime,
                Size = (ulong)fi.Length,
                //TODO: Check if more or equal is right to use
                IsSingleBlock = fi.Length >= (int)BlockSizeOptions.MB_2 ? false : true
            };

            if (file.IsSingleBlock)
            {
                Task<IFileSystemNode> node;
                CancellationToken token;

                List<byte> bytes = ByteTools.GetByteFromFile(path);

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if it really creates single block file
                    node = _manager.Engine().FileSystem.AddFileAsync
                    (
                        path,
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = false,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)BlockSizeOptions.MB_2
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
                            if (node.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (node.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                file.Path = node.Result.Id;
                file.SetHash(bytes.ToArray());
            }
            else
            {
                //TODO: check if works
                BlockSizeOptions options = BlockSizeOptions.kB_128;

                if(fi.Length <= 134217728) //128MB
                {
                    options = BlockSizeOptions.kB_128;
                }
                else if (fi.Length <= 268435456) //256MB
                {
                    options = BlockSizeOptions.kB_256;
                }
                else if (fi.Length <= 536870912) //512MB
                {
                    options = BlockSizeOptions.kB_512;
                }
                else if (fi.Length <= 1073741824) //1GB
                {
                    options = BlockSizeOptions.MB_1;
                }
                else
                {
                    options = BlockSizeOptions.MB_2;
                }

                System.IO.DirectoryInfo di = GetTmpBlocksDirectory(fi.Directory.FullName, file.Name);
                int blockSize = (int)options;
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    //WARNING: High probability of issues 
                    int offset = 0;
                    ulong index;
                    byte[] buffer = new byte[blockSize];
                    for (index = 0; index < (ulong)fi.Length / (ulong)blockSize; ++index)
                    {
                        buffer = new byte[blockSize];
                        fs.Read(buffer, offset, blockSize);

                        offset += blockSize;

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            bs.Write(buffer, 0, blockSize);
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                    if ((ulong)fi.Length % (ulong)blockSize != 0)
                    {
                        buffer = new byte[blockSize];
                        fs.Read(buffer, offset, (int)((ulong)fi.Length % (ulong)blockSize));

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index+1}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            bs.Write(buffer, 0, (int)((ulong)fi.Length % (ulong)blockSize));
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                }
                System.IO.Directory.Delete(di.FullName, true);

                file.SetHash();
                file.Path = $"HF{file.GetHash()}";
            }

            return file;
        }

        private async Task<Hypermedia.File> UploadFileAsync(string path)
        {
            var fi = new System.IO.FileInfo(path);
            Hypermedia.File file = new Hypermedia.File
            {
                Name = fi.Name,
                Extension = fi.Extension,
                Attributes = fi.Attributes,
                LastModifiedDateTime = fi.LastWriteTime,
                Size = (ulong)fi.Length,
                //TODO: Check if more or equal is right to use
                IsSingleBlock = fi.Length >= (int)BlockSizeOptions.MB_2 ? false : true
            };

            if (file.IsSingleBlock)
            {
                Task<IFileSystemNode> node;
                CancellationToken token;

                List<byte> bytes = ByteTools.GetByteFromFile(path);

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if it really creates single block file
                    node = _manager.Engine().FileSystem.AddFileAsync
                    (
                        path,
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = false,
                            RawLeaves = true,
                            Encoding = "base64",
                            Hash = "keccak-512",
                            ChunkSize = (int)BlockSizeOptions.MB_2
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
                            if (node.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (node.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                file.Path = node.Result.Id;
                file.SetHash(bytes.ToArray());
            }
            else
            {
                //TODO: check if works
                BlockSizeOptions options = BlockSizeOptions.kB_128;

                if (fi.Length <= 134217728) //128MB
                {
                    options = BlockSizeOptions.kB_128;
                }
                else if (fi.Length <= 268435456) //256MB
                {
                    options = BlockSizeOptions.kB_256;
                }
                else if (fi.Length <= 536870912) //512MB
                {
                    options = BlockSizeOptions.kB_512;
                }
                else if (fi.Length <= 1073741824) //1GB
                {
                    options = BlockSizeOptions.MB_1;
                }
                else
                {
                    options = BlockSizeOptions.MB_2;
                }

                System.IO.DirectoryInfo di = GetTmpBlocksDirectory(fi.Directory.FullName, file.Name);
                int blockSize = (int)options;
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    //WARNING: High probability of issues 
                    int offset = 0;
                    ulong index;
                    byte[] buffer = new byte[blockSize];
                    for (index = 0; index < (ulong)fi.Length / (ulong)blockSize; ++index)
                    {
                        buffer = new byte[blockSize];
                        await fs.ReadAsync(buffer, offset, blockSize);

                        offset += blockSize;

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            await bs.WriteAsync(buffer, 0, blockSize);
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                    if ((ulong)fi.Length % (ulong)blockSize != 0)
                    {
                        buffer = new byte[blockSize];
                        await fs.ReadAsync(buffer, offset, (int)((ulong)fi.Length % (ulong)blockSize));

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index + 1}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            await bs.WriteAsync(buffer, 0, (int)((ulong)fi.Length % (ulong)blockSize));
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                }
                System.IO.Directory.Delete(di.FullName, true);

                file.SetHash();
                file.Path = $"HF{file.GetHash()}";
            }

            return file;
        }

        private Hypermedia.Block UploadBlock
        (
            string path,
            Hypermedia.File parent,
            BlockSizeOptions options
        )
        {
            var fi = new System.IO.FileInfo(path);

            List<byte> bytes = ByteTools.GetByteFromFile(path);

            Hypermedia.Block block = new Hypermedia.Block
            {
                Parent = parent,
                Size = (ulong)fi.Length,
            };

            Task<IFileSystemNode> node;
            CancellationToken token;

            do
            {
                CancellationTokenSource source = new CancellationTokenSource();
                token = source.Token;
                //TODO: Check if it really creates single block file
                node = _manager.Engine().FileSystem.AddFileAsync
                (
                    path,
                    new AddFileOptions
                    {
                        Pin = true,
                        Wrap = false,
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
                        if (node.Status != TaskStatus.RanToCompletion)
                        {
                            source.Cancel();
                            break;
                        }
                    }

                    if (node.Status == TaskStatus.RanToCompletion)
                    {
                        isTaskCompleted = true;
                        break;
                    }
                }
            }
            while (token.IsCancellationRequested);

            block.Path = node.Result.Id;
            block.SetHash(bytes.ToArray());

            return block;
        }

        private System.IO.DirectoryInfo GetTmpBlocksDirectory(string path, string name)
        {
            string abPath = string.Empty;
            string uniq = string.Empty;
            int counter = 0;
            do
            {
                if (counter > 0)
                {
                    uniq = $"_{counter}";
                }
                abPath = System.IO.Path.Combine(path, $"block_{name}{uniq}");
            }
            while (System.IO.Directory.Exists(abPath));
            var di = System.IO.Directory.CreateDirectory(abPath);
            di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
            return di;
        }

        public Hypermedia.Hypermedia UploadFile
        (
            string path,
            string name,
            string comment,
            BlockSizeOptions options
        )
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException("File doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = (ulong)new System.IO.FileInfo(path).Length,
            };

            hypermedia.Entities.AddWithParent(UploadFile(path, options), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }

        private Hypermedia.File UploadFile(string path, BlockSizeOptions options)
        {
            var fi = new System.IO.FileInfo(path);
            Hypermedia.File file = new Hypermedia.File
            {
                Name = fi.Name,
                Extension = fi.Extension,
                Attributes = fi.Attributes,
                LastModifiedDateTime = fi.LastWriteTime,
                Size = (ulong)fi.Length,
                //TODO: Check if more or equal is right to use
                IsSingleBlock = fi.Length >= (int)options ? false : true
            };

            if (file.IsSingleBlock)
            {
                Task<IFileSystemNode> node;
                CancellationToken token;

                List<byte> bytes = ByteTools.GetByteFromFile(path);

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if it really creates single block file
                    node = _manager.Engine().FileSystem.AddFileAsync
                    (
                        path,
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = false,
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
                            if (node.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (node.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                file.Path = node.Result.Id;
                file.SetHash(bytes.ToArray());
            }
            else
            {
                System.IO.DirectoryInfo di = GetTmpBlocksDirectory(fi.Directory.FullName, file.Name);
                int blockSize = (int)options;
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    //WARNING: High probability of issues 
                    int offset = 0;
                    ulong index;
                    byte[] buffer = new byte[blockSize];
                    for (index = 0; index < (ulong)fi.Length / (ulong)blockSize; ++index)
                    {
                        buffer = new byte[blockSize];
                        fs.Read(buffer, offset, blockSize);

                        offset += blockSize;

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            bs.Write(buffer, 0, blockSize);
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                    if ((ulong)fi.Length % (ulong)blockSize != 0)
                    {
                        buffer = new byte[blockSize];
                        fs.Read(buffer, offset, (int)((ulong)fi.Length % (ulong)blockSize));

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index + 1}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            bs.Write(buffer, 0, (int)((ulong)fi.Length % (ulong)blockSize));
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                }
                System.IO.Directory.Delete(di.FullName, true);

                file.SetHash();
                file.Path = $"HF{file.GetHash()}";
            }

            return file;
        }

        private async Task<Hypermedia.File> UploadFileAsync(string path, BlockSizeOptions options)
        {
            var fi = new System.IO.FileInfo(path);
            Hypermedia.File file = new Hypermedia.File
            {
                Name = fi.Name,
                Extension = fi.Extension,
                Attributes = fi.Attributes,
                LastModifiedDateTime = fi.LastWriteTime,
                Size = (ulong)fi.Length,
                //TODO: Check if more or equal is right to use
                IsSingleBlock = fi.Length >= (int)options ? false : true
            };

            if (file.IsSingleBlock)
            {
                Task<IFileSystemNode> node;
                CancellationToken token;

                List<byte> bytes = ByteTools.GetByteFromFile(path);

                do
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    token = source.Token;
                    //TODO: Check if it really creates single block file
                    node = _manager.Engine().FileSystem.AddFileAsync
                    (
                        path,
                        new AddFileOptions
                        {
                            Pin = true,
                            Wrap = false,
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
                            if (node.Status != TaskStatus.RanToCompletion)
                            {
                                source.Cancel();
                                break;
                            }
                        }

                        if (node.Status == TaskStatus.RanToCompletion)
                        {
                            isTaskCompleted = true;
                            break;
                        }
                    }
                }
                while (token.IsCancellationRequested);
                file.Path = node.Result.Id;
                file.SetHash(bytes.ToArray());
            }
            else
            {
                System.IO.DirectoryInfo di = GetTmpBlocksDirectory(fi.Directory.FullName, file.Name);
                int blockSize = (int)options;
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    //WARNING: High probability of issues 
                    int offset = 0;
                    ulong index;
                    byte[] buffer = new byte[blockSize];
                    for (index = 0; index < (ulong)fi.Length / (ulong)blockSize; ++index)
                    {
                        buffer = new byte[blockSize];
                        await fs.ReadAsync(buffer, offset, blockSize);

                        offset += blockSize;

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            await bs.WriteAsync(buffer, 0, blockSize);
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                    if ((ulong)fi.Length % (ulong)blockSize != 0)
                    {
                        buffer = new byte[blockSize];
                        await fs.ReadAsync(buffer, offset, (int)((ulong)fi.Length % (ulong)blockSize));

                        string blockPath = System.IO.Path.Combine(di.FullName, $"{index + 1}.block");
                        using (var bs = new System.IO.FileStream(blockPath, System.IO.FileMode.CreateNew))
                        {
                            await bs.WriteAsync(buffer, 0, (int)((ulong)fi.Length % (ulong)blockSize));
                        }

                        file.Blocks.Add(UploadBlock(blockPath, file, options));

                        System.IO.File.Delete(blockPath);
                    }
                }
                System.IO.Directory.Delete(di.FullName, true);

                file.SetHash();
                file.Path = $"HF{file.GetHash()}";
            }

            return file;
        }

        public async Task<Hypermedia.Hypermedia> UploadFileAsync(string path, string name)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException("File doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = (ulong)new System.IO.FileInfo(path).Length,
            };

            hypermedia.Entities.AddWithParent(await UploadFileAsync(path), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> UploadFileAsync
        (
            string path,
            string name,
            string comment,
            BlockSizeOptions options
        )
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException("File doesn't exist", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null,
                Size = (ulong)new System.IO.FileInfo(path).Length,
            };

            hypermedia.Entities.AddWithParent(await UploadFileAsync(path, options), hypermedia);
            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> UploadEntitiesAsync(string[] paths, string name)
        {
            List<string> filePaths = new List<string>();
            List<string> directoryPaths = new List<string>();
            List<Hypermedia.Hypermedia> hypermedias = new List<Hypermedia.Hypermedia>();
            foreach (var p in paths)
            {
                try
                {
                    System.IO.FileAttributes attributes = System.IO.File.GetAttributes(p);
                    if (attributes.HasFlag(System.IO.FileAttributes.Directory))
                    {
                        directoryPaths.Add(p);
                    }
                    else
                    {
                        var fi = new System.IO.FileInfo(p);
                        if (fi.Extension == "hyper")
                        {
                            try
                            {
                                using (var fs = new System.IO.FileStream(p, System.IO.FileMode.Open))
                                {
                                    var hyper = await Hypermedia.Hypermedia.DeserializeAsync(fs);
                                    hypermedias.Add(hyper);
                                }
                            }
                            catch
                            {
                                filePaths.Add(p);
                            }
                        }
                        else
                        {
                            filePaths.Add(p);
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException($"Entity at path {p} doesn't exists", nameof(paths));
                }
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null
            };

            ulong size = 0;

            foreach (var h in hypermedias)
            {
                hypermedia = await _manager.HypermediaService.ConstructWrappedHypermediaAsync(hypermedia, h);
                size += h.Size;
            }

            foreach (var d in directoryPaths)
            {
                hypermedia.Entities.AddWithParent(await UploadDirectoryAsync(d, true), hypermedia);
                size += GetDirectorySize(new System.IO.DirectoryInfo(d), true);
            }

            foreach (var f in filePaths)
            {
                hypermedia.Entities.AddWithParent(await UploadFileAsync(f), hypermedia);
                size += (ulong)new System.IO.FileInfo(f).Length;
            }

            hypermedia.Size = size;

            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public Hypermedia.Hypermedia UploadEntities(string[] paths, string name)
        {
            List<string> filePaths = new List<string>();
            List<string> directoryPaths = new List<string>();
            List<Hypermedia.Hypermedia> hypermedias = new List<Hypermedia.Hypermedia>();
            foreach (var p in paths)
            {
                try
                {
                    System.IO.FileAttributes attributes = System.IO.File.GetAttributes(p);
                    if (attributes.HasFlag(System.IO.FileAttributes.Directory))
                    {
                        directoryPaths.Add(p);
                    }
                    else
                    {
                        var fi = new System.IO.FileInfo(p);
                        if(fi.Extension == "hyper")
                        {
                            try
                            {
                                using (var fs = new System.IO.FileStream(p, System.IO.FileMode.Open))
                                {
                                    var hyper = Hypermedia.Hypermedia.Deserialize(fs);
                                    hypermedias.Add(hyper);
                                }
                            }
                            catch
                            {
                                filePaths.Add(p);
                            }
                        }
                        else
                        {
                            filePaths.Add(p);
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException($"Entity at path {p} doesn't exists", nameof(paths));
                }
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null
            };

            ulong size = 0;

            foreach (var h in hypermedias)
            {
                hypermedia = _manager.HypermediaService.ConstructWrappedHypermedia(hypermedia, h);
                size += h.Size;
            }

            foreach(var d in directoryPaths)
            {
                hypermedia.Entities.AddWithParent(UploadDirectory(d, true), hypermedia);
                size += GetDirectorySize(new System.IO.DirectoryInfo(d), true);
            }

            foreach(var f in filePaths)
            {
                hypermedia.Entities.AddWithParent(UploadFile(f), hypermedia);
                size += (ulong)new System.IO.FileInfo(f).Length;
            }

            hypermedia.Size = size;

            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> UploadEntitiesAsync
        (
            string[] paths,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        )
        {
            List<string> filePaths = new List<string>();
            List<string> directoryPaths = new List<string>();
            List<Hypermedia.Hypermedia> hypermedias = new List<Hypermedia.Hypermedia>();
            foreach (var p in paths)
            {
                try
                {
                    System.IO.FileAttributes attributes = System.IO.File.GetAttributes(p);
                    if (attributes.HasFlag(System.IO.FileAttributes.Directory))
                    {
                        directoryPaths.Add(p);
                    }
                    else
                    {
                        var fi = new System.IO.FileInfo(p);
                        if (fi.Extension == "hyper")
                        {
                            try
                            {
                                using (var fs = new System.IO.FileStream(p, System.IO.FileMode.Open))
                                {
                                    var hyper = await Hypermedia.Hypermedia.DeserializeAsync(fs);
                                    hypermedias.Add(hyper);
                                }
                            }
                            catch
                            {
                                filePaths.Add(p);
                            }
                        }
                        else
                        {
                            filePaths.Add(p);
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException($"Entity at path {p} doesn't exists", nameof(paths));
                }
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = (await _manager.Engine().Generic.IdAsync()).Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null
            };

            ulong size = 0;

            foreach (var h in hypermedias)
            {
                hypermedia = await _manager.HypermediaService.ConstructWrappedHypermediaAsync(hypermedia, h);
                size += h.Size;
            }

            foreach (var d in directoryPaths)
            {
                hypermedia.Entities.AddWithParent(await UploadDirectoryAsync(d, recursive, options), hypermedia);
                size += GetDirectorySize(new System.IO.DirectoryInfo(d), true);
            }

            foreach (var f in filePaths)
            {
                hypermedia.Entities.AddWithParent(await UploadFileAsync(f, options), hypermedia);
                size += (ulong)new System.IO.FileInfo(f).Length;
            }

            hypermedia.Size = size;

            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = await _manager.HypermediaService.CreateHypermediaWithPathAsync(hypermedia);

            return hypermedia;
        }

        public Hypermedia.Hypermedia UploadEntities
        (
            string[] paths,
            string name,
            string comment,
            bool recursive,
            BlockSizeOptions options
        )
        {
            List<string> filePaths = new List<string>();
            List<string> directoryPaths = new List<string>();
            List<Hypermedia.Hypermedia> hypermedias = new List<Hypermedia.Hypermedia>();
            foreach (var p in paths)
            {
                try
                {
                    System.IO.FileAttributes attributes = System.IO.File.GetAttributes(p);
                    if (attributes.HasFlag(System.IO.FileAttributes.Directory))
                    {
                        directoryPaths.Add(p);
                    }
                    else
                    {
                        var fi = new System.IO.FileInfo(p);
                        if (fi.Extension == "hyper")
                        {
                            try
                            {
                                using (var fs = new System.IO.FileStream(p, System.IO.FileMode.Open))
                                {
                                    var hyper = Hypermedia.Hypermedia.Deserialize(fs);
                                    hypermedias.Add(hyper);
                                }
                            }
                            catch
                            {
                                filePaths.Add(p);
                            }
                        }
                        else
                        {
                            filePaths.Add(p);
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException($"Entity at path {p} doesn't exists", nameof(paths));
                }
            }

            Hypermedia.Hypermedia hypermedia = new Hypermedia.Versions.ver010.Hypermedia010
            {
                Name = name,
                Comment = comment,
                CreatedBy = "hypermedia/manager",
                CreatedDateTime = DateTime.Now,
                CreatorPeer = _manager.Engine().Generic.IdAsync().Result.Id.ToString(),
                IsRawIPFS = false,
                IsDirectoryWrapped = true,
                Parent = null
            };

            ulong size = 0;

            foreach (var h in hypermedias)
            {
                hypermedia = _manager.HypermediaService.ConstructWrappedHypermedia(hypermedia, h);
                size += h.Size;
            }

            foreach (var d in directoryPaths)
            {
                hypermedia.Entities.AddWithParent(UploadDirectory(d, recursive, options), hypermedia);
                size += GetDirectorySize(new System.IO.DirectoryInfo(d), true);
            }

            foreach (var f in filePaths)
            {
                hypermedia.Entities.AddWithParent(UploadFile(f, options), hypermedia);
                size += (ulong)new System.IO.FileInfo(f).Length;
            }

            hypermedia.Size = size;

            hypermedia.SetHash();
            hypermedia.SetTopic();
            hypermedia = _manager.HypermediaService.CreateHypermediaWithPath(hypermedia);

            return hypermedia;
        }
    }
}
