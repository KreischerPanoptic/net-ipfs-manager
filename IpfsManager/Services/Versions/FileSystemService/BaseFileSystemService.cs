using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ipfs.Manager.Cryptography;
using Ipfs.Manager.Models;
using Ipfs.Manager.Tools;

namespace Ipfs.Manager.Services.Versions.FileSystemService
{
    class BaseFileSystemService : IFileSystemService
    {
        private Manager _manager;

        public BaseFileSystemService(Manager manager)
        {
            _manager = manager;
        }
        public bool ClearTempFileSystem(Models.Hypermedia hypermedia)
        {
            List<Models.File> filesToBeCleaned = new List<File>();

            foreach (var f in hypermedia.Files)
            {
                if (f.Status == Status.Completed || f.Status == Status.Seeding)
                {
                    if (!f.IsSingleBlock)
                    {
                        filesToBeCleaned.Add(f);
                    }
                }
                else
                {
                    throw new ArgumentException("Not all files downloaded");
                }
            }

            foreach (var d in hypermedia.Directories)
            {
                filesToBeCleaned.AddRange(GetFilesFromDirectory(d));
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                filesToBeCleaned.AddRange(GetFilesFromHypermedia(h));
            }

            if(filesToBeCleaned.Count <= 0)
            {
                return true;
            }

            List<string> pathes = new List<string>();

            foreach (var f in filesToBeCleaned)
            {
                pathes.Add(f.BlockStorePath);
            }

            foreach(var p in pathes)
            {
                try
                {
                    System.IO.Directory.Delete(p, true);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        private List<Models.File> GetFilesFromHypermedia(Models.Hypermedia hypermedia)
        {
            List<Models.File> filesToBeCleaned = new List<File>();

            foreach (var f in hypermedia.Files)
            {
                if (f.Status == Status.Completed || f.Status == Status.Seeding)
                {
                    if (!f.IsSingleBlock)
                    {
                        filesToBeCleaned.Add(f);
                    }
                }
                else
                {
                    throw new ArgumentException("Not all files downloaded");
                }
            }

            foreach (var d in hypermedia.Directories)
            {
                filesToBeCleaned.AddRange(GetFilesFromDirectory(d));
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                filesToBeCleaned.AddRange(GetFilesFromHypermedia(h));
            }

            return filesToBeCleaned;
        }

        private List<Models.File> GetFilesFromDirectory(Models.Directory directory)
        {
            List<Models.File> filesToBeCleaned = new List<File>();

            foreach (var f in directory.Files)
            {
                if (f.Status == Status.Completed || f.Status == Status.Seeding)
                {
                    if (!f.IsSingleBlock)
                    {
                        filesToBeCleaned.Add(f);
                    }
                }
                else
                {
                    throw new ArgumentException("Not all files downloaded");
                }
            }

            foreach (var d in directory.Directories)
            {
                filesToBeCleaned.AddRange(GetFilesFromDirectory(d));
            }

            return filesToBeCleaned;
        }

        public bool CheckAndFixFileSystemModel(Models.Hypermedia hypermedia)
        {
            return CheckAndFixHypermediaFileSystemModel(hypermedia);
        }

        private bool CheckAndFixHypermediaFileSystemModel(Models.Hypermedia hypermedia)
        {
            string basePath = hypermedia.InternalPath;
            if (!System.IO.Directory.Exists(basePath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                catch
                {
                    return false;
                }
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                if (!CheckAndFixHypermediaFileSystemModel(h))
                {
                    return false;
                }
            }

            foreach (var d in hypermedia.Directories)
            {
                if (!CheckAndFixDirectoryFileSystemModel(d))
                {
                    return false;
                }
            }

            foreach (var f in hypermedia.Files)
            {
                if (!CheckAndFixFileFileSystemModel(f))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckAndFixDirectoryFileSystemModel(Models.Directory directory)
        {
            string basePath = directory.InternalPath;
            if (!System.IO.Directory.Exists(basePath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                catch
                {
                    return false;
                }
            }

            if(GetRelativeParent(directory).IsAttributesPreservationEnabled)
            {
                try
                {
                    var di = new System.IO.DirectoryInfo(basePath);
                    if (di.Attributes == directory.Attributes.GetValueOrDefault(System.IO.FileAttributes.Directory))
                    {
                        di.Attributes = directory.Attributes.GetValueOrDefault(System.IO.FileAttributes.Directory);
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var di = new System.IO.DirectoryInfo(basePath);
                    if (di.Attributes == System.IO.FileAttributes.Directory)
                    {
                        di.Attributes = System.IO.FileAttributes.Directory;
                    }
                }
                catch
                {
                    return false;
                }
            }

            foreach (var d in directory.Directories)
            {
                if (!CheckAndFixDirectoryFileSystemModel(d))
                {
                    return false;
                }
            }

            foreach (var f in directory.Files)
            {
                if (!CheckAndFixFileFileSystemModel(f))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckAndFixFileFileSystemModel(Models.File file)
        {
            string basePath = file.InternalPath;
            if (!System.IO.File.Exists(basePath))
            {
                try
                {
                    System.IO.File.Create(basePath);
                    if (!System.IO.Directory.Exists(file.BlockStorePath))
                    {
                        var di = System.IO.Directory.CreateDirectory(file.BlockStorePath);
                        di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
                    }
                }
                catch
                {
                    return false;
                }
            }

            if (GetRelativeParent(file).IsAttributesPreservationEnabled)
            {
                try
                {
                    var fi = new System.IO.FileInfo(basePath);
                    if (fi.Attributes != file.Attributes.GetValueOrDefault(System.IO.FileAttributes.Normal))
                    {
                        fi.Attributes = file.Attributes.GetValueOrDefault(System.IO.FileAttributes.Normal);
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var fi = new System.IO.FileInfo(basePath);
                    if (fi.Attributes != System.IO.FileAttributes.Normal)
                    {
                        fi.Attributes = System.IO.FileAttributes.Normal;
                    }
                }
                catch
                {
                    return false;
                }
            }

            foreach (var b in file.Blocks)
            {
                if (!CheckAndFixBlockFileSystemModel(b))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckAndFixBlockFileSystemModel(Models.Block block)
        {
            string basePath = block.InternalPath;

            if (!System.IO.File.Exists(basePath))
            {
                try
                {
                    System.IO.File.Create(basePath);
                    System.IO.File.SetAttributes(basePath, System.IO.FileAttributes.Hidden);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private Models.Hypermedia GetRelativeParent(IEntity entity)
        {
            if(entity.Parent is Models.Hypermedia)
            {
                return entity.Parent as Models.Hypermedia;
            }
            else
            {
                return GetRelativeParent(entity.Parent);
            }
        }
     
        public bool CreateFileSystemModel(Models.Hypermedia hypermedia)
        {
            return CreateHypermediaFileSystemModel(hypermedia);
        }

        private bool CreateHypermediaFileSystemModel(Models.Hypermedia hypermedia)
        {
            string basePath = hypermedia.InternalPath;
            try
            {
                if (hypermedia.WrappingOptions == WrappingOptions.SubDirectory)
                {
                    System.IO.Directory.CreateDirectory(basePath);
                }
                else
                {
                    if(!System.IO.Directory.Exists(basePath))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                if (!CreateHypermediaFileSystemModel(h))
                {
                    return false;
                }
            }

            foreach (var d in hypermedia.Directories)
            {
                if (!CreateDirectoryFileSystemModel(d))
                {
                    return false;
                }
            }

            foreach (var f in hypermedia.Files)
            {
                if (!CreateFileFileSystemModel(f))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CreateDirectoryFileSystemModel(Models.Directory directory)
        {
            string basePath = directory.InternalPath;
            try
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            catch
            {
                return false;
            }

            if (GetRelativeParent(directory).IsAttributesPreservationEnabled)
            {
                try
                {
                    var di = new System.IO.DirectoryInfo(basePath);
                    di.Attributes = directory.Attributes.GetValueOrDefault(System.IO.FileAttributes.Directory);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var di = new System.IO.DirectoryInfo(basePath);
                    di.Attributes = System.IO.FileAttributes.Directory;
                }
                catch
                {
                    return false;
                }
            }

            foreach (var d in directory.Directories)
            {
                if (!CreateDirectoryFileSystemModel(d))
                {
                    return false;
                }
            }

            foreach (var f in directory.Files)
            {
                if (!CreateFileFileSystemModel(f))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CreateFileFileSystemModel(Models.File file)
        {
            string basePath = file.InternalPath;
            try
            {
                System.IO.File.Create(basePath);
                var di = System.IO.Directory.CreateDirectory(file.BlockStorePath);
                di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
            }
            catch
            {
                return false;
            }

            if (GetRelativeParent(file).IsAttributesPreservationEnabled)
            {
                try
                {
                    var fi = new System.IO.FileInfo(basePath);
                    fi.Attributes = file.Attributes.GetValueOrDefault(System.IO.FileAttributes.Normal);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var fi = new System.IO.FileInfo(basePath);
                    fi.Attributes = System.IO.FileAttributes.Normal;
                }
                catch
                {
                    return false;
                }
            }

            foreach (var b in file.Blocks)
            {
                if (!CreateBlockFileSystemModel(b))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CreateBlockFileSystemModel(Models.Block block)
        {
            string basePath = block.InternalPath;
            try
            {
                System.IO.File.Create(basePath);
                System.IO.File.SetAttributes(basePath, System.IO.FileAttributes.Hidden);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia)
        {
            return await DeleteHypermediaAsync(hypermedia, false);
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia)
        {
            return DeleteHypermedia(hypermedia, false);
        }

        public async Task<bool> DeleteHypermediaAsync(Models.Hypermedia hypermedia, bool isFileDeletionRequested)
        {
            if (!System.IO.Directory.Exists(hypermedia.InternalPath))
            {
                throw new ArgumentException("Hypermedia download path does not exist", nameof(hypermedia));
            }

            bool isAllDone = true;
            foreach (var f in hypermedia.Files)
            {
                if (!await UnpinFileAsync(f))
                {
                    isAllDone = false;
                }
                if (isFileDeletionRequested)
                {
                    try
                    {
                        System.IO.File.Delete(f.InternalPath);
                        if(System.IO.Directory.Exists(f.BlockStorePath))
                        {
                            try
                            {
                                System.IO.Directory.Delete(f.BlockStorePath, true);
                            }
                            catch
                            {
                                isAllDone = false;
                            }
                        }
                    }
                    catch
                    {
                        isAllDone = false;
                    }
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!await UnpinDirectoryAsync(d))
                {
                    isAllDone = false;
                }
                if (isFileDeletionRequested)
                {
                    try
                    {
                        System.IO.Directory.Delete(d.InternalPath, true);
                    }
                    catch
                    {
                        isAllDone = false;
                    }
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (h.WrappingOptions == WrappingOptions.SubDirectory)
                {
                    if (!await UnpinHypermediaAsync(h))
                    {
                        isAllDone = false;
                    }

                    if (isFileDeletionRequested)
                    {
                        try
                        {
                            System.IO.Directory.Delete(h.InternalPath, true);
                        }
                        catch
                        {
                            isAllDone = false;
                        }
                    }
                }
                else
                {
                    if (!await DeleteHypermediaAsync(h, isFileDeletionRequested))
                    {
                        isAllDone = false;
                    }
                }
            }

            if (isFileDeletionRequested)
            {
                try
                {
                    if (hypermedia.WrappingOptions == WrappingOptions.SubDirectory)
                    {
                        System.IO.Directory.Delete(hypermedia.InternalPath, true);
                    }
                }
                catch
                {
                    isAllDone = false;
                }
            }

            return isAllDone;
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested)
        {
            if (!System.IO.Directory.Exists(hypermedia.InternalPath))
            {
                throw new ArgumentException("Hypermedia download path does not exist", nameof(hypermedia));
            }

            bool isAllDone = true;
            foreach (var f in hypermedia.Files)
            {
                if (!UnpinFile(f))
                {
                    isAllDone = false;
                }
                if (isFileDeletionRequested)
                {
                    try
                    {
                        System.IO.File.Delete(f.InternalPath);
                        if (System.IO.Directory.Exists(f.BlockStorePath))
                        {
                            try
                            {
                                System.IO.Directory.Delete(f.BlockStorePath, true);
                            }
                            catch
                            {
                                isAllDone = false;
                            }
                        }
                    }
                    catch
                    {
                        isAllDone = false;
                    }
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    isAllDone = false;
                }
                if (isFileDeletionRequested)
                {
                    try
                    {
                        System.IO.Directory.Delete(d.InternalPath, true);
                    }
                    catch
                    {
                        isAllDone = false;
                    }
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (h.WrappingOptions == WrappingOptions.SubDirectory)
                {
                    if (!UnpinHypermedia(h))
                    {
                        isAllDone = false;
                    }

                    if (isFileDeletionRequested)
                    {
                        try
                        {
                            System.IO.Directory.Delete(h.InternalPath, true);
                        }
                        catch
                        {
                            isAllDone = false;
                        }
                    }
                }
                else
                {
                    if (!DeleteHypermedia(h, isFileDeletionRequested))
                    {
                        isAllDone = false;
                    }
                }
            }

            if (isFileDeletionRequested)
            {
                try
                {
                    if (hypermedia.WrappingOptions == WrappingOptions.SubDirectory)
                    {
                        System.IO.Directory.Delete(hypermedia.InternalPath, true);
                    }
                }
                catch
                {
                    isAllDone = false;
                }
            }

            return isAllDone;
        }

        private async Task<bool> UnpinHypermediaAsync(Models.Hypermedia hypermedia)
        {
            bool isAllUnpinned = true;
            foreach (var f in hypermedia.Files)
            {
                if (!await UnpinFileAsync(f))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!await UnpinDirectoryAsync(d))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!await UnpinHypermediaAsync(h))
                {
                    isAllUnpinned = false;
                }
            }
            return isAllUnpinned;
        }

        private bool UnpinHypermedia(Models.Hypermedia hypermedia)
        {
            bool isAllUnpinned = true;
            foreach (var f in hypermedia.Files)
            {
                if (!UnpinFile(f))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!UnpinHypermedia(h))
                {
                    isAllUnpinned = false;
                }
            }
            return isAllUnpinned;
        }

        private async Task<bool> UnpinDirectoryAsync(Models.Directory directory)
        {
            bool isAllUnpinned = true;
            foreach (var f in directory.Files)
            {
                if (!await UnpinFileAsync(f))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var d in directory.Directories)
            {
                if (!await UnpinDirectoryAsync(d))
                {
                    isAllUnpinned = false;
                }
            }
            return isAllUnpinned;
        }

        private bool UnpinDirectory(Models.Directory directory)
        {
            bool isAllUnpinned = true;
            foreach (var f in directory.Files)
            {
                if (!UnpinFile(f))
                {
                    isAllUnpinned = false;
                }
            }
            foreach (var d in directory.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    isAllUnpinned = false;
                }
            }
            return isAllUnpinned;
        }

        private async Task<bool> UnpinFileAsync(Models.File file)
        {
            if (!file.IsSingleBlock)
            {
                bool isAllUnpinned = true;
                foreach (var b in file.Blocks)
                {
                    if (!await UnpinBlockAsync(b))
                    {
                        isAllUnpinned = false;
                    }
                }
                return isAllUnpinned;
            }
            else
            {
                var r = await _manager.Engine().Pin.RemoveAsync(file.Path);
                return r.First() == file.Path;
            }
        }

        private bool UnpinFile(Models.File file)
        {
            if (!file.IsSingleBlock)
            {
                bool isAllUnpinned = true;
                foreach (var b in file.Blocks)
                {
                    if (!UnpinBlock(b))
                    {
                        isAllUnpinned = false;
                    }
                }
                return isAllUnpinned;
            }
            else
            {
                var r = _manager.Engine().Pin.RemoveAsync(file.Path).Result;
                return r.First() == file.Path;
            }
        }

        private async Task<bool> UnpinBlockAsync(Models.Block block)
        {
            var r = await _manager.Engine().Pin.RemoveAsync(block.Path);
            //TODO: Check if correctly unpinning
            return r.First() == block.Path;
        }

        private bool UnpinBlock(Models.Block block)
        {
            var r = _manager.Engine().Pin.RemoveAsync(block.Path).Result;
            //TODO: Check if correctly unpinning
            return r.First() == block.Path;
        }

        public bool IsBlockValid(Models.Block block)
        {
            string blockPath = block.InternalPath;
            if(!System.IO.File.Exists(blockPath))
            {
                throw new ArgumentException("Block is not created yet, or misconfigurated", nameof(block));
            }

            List<byte> bytes = ByteTools.GetByteFromFile(blockPath);

            KeccakManaged keccak = new KeccakManaged(512);
            var buf = keccak.ComputeHash(bytes.ToArray());

            StringBuilder sb = new StringBuilder();
            foreach (var b in buf)
            {
                sb.Append(b.ToString("X2"));
            }
            string internalHash = sb.ToString();

            //TODO: Check if works
            return internalHash == block.Hash;
        }

        public bool IsDirectoryValid(Models.Directory directory)
        {
            if (!System.IO.Directory.Exists(directory.InternalPath))
            {
                throw new ArgumentException("Directory is not created yet, or misconfigurated", nameof(directory));
            }

            List<IEntity> entities = new List<IEntity>();
            foreach (var f in directory.Files)
            {
                if (!IsFileValid(f))
                {
                    return false;
                }
                entities.Add(f);
            }
            foreach (var d in directory.Directories)
            {
                if (!IsDirectoryValid(d))
                {
                    return false;
                }
                entities.Add(d);
            }

            entities = entities.OrderBy(e => e.Index).ToList();

            KeccakManaged keccak = new KeccakManaged(512);
            List<string> entitesHashes = new List<string>();
            foreach (var e in entities)
            {
                entitesHashes.Add(e.Hash);
            }
            List<byte> buffer = new List<byte>();
            foreach (var eh in entitesHashes)
            {
                buffer.AddRange(Encoding.UTF8.GetBytes(eh));
            }

            var buf = keccak.ComputeHash(buffer.ToArray());

            StringBuilder sb = new StringBuilder();
            foreach (var b in buf)
            {
                sb.Append(b.ToString("X2"));
            }
            string internalHash = sb.ToString();

            //TODO: Check if works
            return internalHash == directory.Hash;
        }

        public bool IsFileValid(Models.File file)
        {
            if (!System.IO.File.Exists(file.InternalPath))
            {
                throw new ArgumentException("File is not created yet, or misconfigurated", nameof(file));
            }

            KeccakManaged keccak = new KeccakManaged(512);
            if(!file.IsSingleBlock)
            {
                string blockStorePath = System.IO.Path.Combine(file.InternalPath, $"{file.Path}_blocks");

                if(!System.IO.Directory.Exists(blockStorePath))
                {
                    throw new ArgumentException("File is declared as multiblock, yet does not contain blocks", nameof(file));
                }
                foreach(var b in file.Blocks)
                {
                    if(!IsBlockValid(b))
                    {
                        return false;
                    }
                }
                List<string> blockHashes = new List<string>();
                foreach (var b in file.Blocks)
                {
                    blockHashes.Add(b.Hash);
                }
                List<byte> buffer = new List<byte>();
                foreach (var bh in blockHashes)
                {
                    buffer.AddRange(Encoding.UTF8.GetBytes(bh));
                }

                var buf = keccak.ComputeHash(buffer.ToArray());

                StringBuilder sb = new StringBuilder();
                foreach (var b in buf)
                {
                    sb.Append(b.ToString("X2"));
                }
                string internalHash = sb.ToString();

                //TODO: Check if works
                return internalHash == file.Hash;
            }
            else
            {
                List<byte> bytes = ByteTools.GetByteFromFile(file.InternalPath);

                var buf = keccak.ComputeHash(bytes.ToArray());

                StringBuilder sb = new StringBuilder();
                foreach (var b in buf)
                {
                    sb.Append(b.ToString("X2"));
                }
                string internalHash = sb.ToString();

                //TODO: Check if works
                return internalHash == file.Hash;
            }
        }

        public bool IsHypermediaValid(Models.Hypermedia hypermedia)
        {
            if (!System.IO.Directory.Exists(hypermedia.InternalPath))
            {
                throw new ArgumentException("Hypermedia is misconfigurated", nameof(hypermedia));
            }

            List<IEntity> entities = new List<IEntity>();
            foreach (var f in hypermedia.Files)
            {
                if (!IsFileValid(f))
                {
                    return false;
                }
                entities.Add(f);
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!IsDirectoryValid(d))
                {
                    return false;
                }
                entities.Add(d);
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!IsHypermediaValid(h))
                {
                    return false;
                }
                entities.Add(h);
            }

            entities = entities.OrderBy(e => e.Index).ToList();

            KeccakManaged keccak = new KeccakManaged(512);

            List<string> entitesHashes = new List<string>();
            foreach (var e in entities)
            {
                entitesHashes.Add(e.Hash);
            }
            List<byte> buffer = new List<byte>();
            foreach (var eh in entitesHashes)
            {
                buffer.AddRange(Encoding.UTF8.GetBytes(eh));
            }

            var buf = keccak.ComputeHash(buffer.ToArray());

            StringBuilder sb = new StringBuilder();
            foreach (var b in buf)
            {
                sb.Append(b.ToString("X2"));
            }
            string internalHash = sb.ToString();

            //TODO: Check if works
            return internalHash == hypermedia.Hash;
        }

        public async Task<bool> IsBlockValidAsync(Block block)
        {
            return await Task.Run(async () =>
            {
                return IsBlockValid(block);
            }).ConfigureAwait(false);
        }

        public async Task<bool> IsFileValidAsync(File file)
        {
            return await Task.Run(async () =>
            {
                return IsFileValid(file);
            }).ConfigureAwait(false);
        }

        public async Task<bool> IsDirectoryValidAsync(Directory directory)
        {
            return await Task.Run(async () =>
            {
                return IsDirectoryValid(directory);
            }).ConfigureAwait(false);
        }

        public async Task<bool> IsHypermediaValidAsync(Models.Hypermedia hypermedia)
        {
            return await Task.Run(async () =>
            {
                return IsHypermediaValid(hypermedia);
            }).ConfigureAwait(false);
        }

        public Hypermedia.Hypermedia OpenHypermedia(string path)
        {
            if(!System.IO.File.Exists(path))
            {
                throw new ArgumentException("Mentioned file can not be found", nameof(path));
            }
            //TODO check if correct
            if (System.IO.Path.GetExtension(path) != "hyper")
            {
                throw new ArgumentException("Unknown file format detected", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = null;
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                try
                {
                    hypermedia = Hypermedia.Hypermedia.Deserialize(fs);
                }
                catch
                {
                    throw new ArgumentException("Deserialization error encountered while processing mentioned file", nameof(path));
                }
            }
            return hypermedia;
        }

        public async Task<Hypermedia.Hypermedia> OpenHypermediaAsync(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException("Mentioned file can not be found", nameof(path));
            }
            //TODO check if correct
            if (System.IO.Path.GetExtension(path) != "hyper")
            {
                throw new ArgumentException("Unknown file format detected", nameof(path));
            }

            Hypermedia.Hypermedia hypermedia = null;
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                try
                {
                    hypermedia = await Hypermedia.Hypermedia.DeserializeAsync(fs);
                }
                catch
                {
                    throw new ArgumentException("Deserialization error encountered while processing mentioned file", nameof(path));
                }
            }
            return hypermedia;
        }

        public bool ReconstructFile(Models.File file)
        {
            List<byte> buffer = new List<byte>();
            foreach (var block in file.Blocks)
            {
                buffer.Clear();
                try
                {
                    buffer = ByteTools.GetByteFromFile(block.InternalPath);
                }
                catch
                {
                    return false;
                }

                try
                {
                    //TODO check if it works
                    using (var fileStream = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                    {
                        foreach (var b in buffer)
                        {
                            fileStream.WriteByte(b);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> ReconstructFileAsync(File file)
        {
            return await Task.Run(async () =>
            {
                return ReconstructFile(file);
            }).ConfigureAwait(false);
        }

        public void SaveHypermedia(Hypermedia.Hypermedia hypermedia, string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Choosen directory does not exist", nameof(path));
            }

            if (System.IO.File.Exists(hypermedia.Name + ".hyper"))
            {
                throw new ArgumentException("File already exists.", nameof(path));
            }

            using (var fs = new System.IO.FileStream(hypermedia.Name + ".hyper", System.IO.FileMode.Create))
            {
                Hypermedia.Hypermedia.Serialize(fs, hypermedia);
            }
        }

        public void SaveHypermedia(Hypermedia.Hypermedia hypermedia, string path, string name)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Choosen directory does not exist", nameof(path));
            }

            if (System.IO.File.Exists(name + ".hyper"))
            {
                throw new ArgumentException("File already exists.", nameof(path));
            }

            using (var fs = new System.IO.FileStream(name + ".hyper", System.IO.FileMode.Create))
            {
                Hypermedia.Hypermedia.Serialize(fs, hypermedia);
            }
        }

        public async Task SaveHypermediaAsync(Hypermedia.Hypermedia hypermedia, string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Choosen directory does not exist", nameof(path));
            }

            if (System.IO.File.Exists(hypermedia.Name + ".hyper"))
            {
                throw new ArgumentException("File already exists.", nameof(path));
            }

            using (var fs = new System.IO.FileStream(hypermedia.Name + ".hyper", System.IO.FileMode.Create))
            {
                await Hypermedia.Hypermedia.SerializeAsync(fs, hypermedia);
            }
        }

        public async Task SaveHypermediaAsync(Hypermedia.Hypermedia hypermedia, string path, string name)
        {
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException("Choosen directory does not exist", nameof(path));
            }

            if (System.IO.File.Exists(name + ".hyper"))
            {
                throw new ArgumentException("File already exists.", nameof(path));
            }

            using (var fs = new System.IO.FileStream(name + ".hyper", System.IO.FileMode.Create))
            {
                await Hypermedia.Hypermedia.SerializeAsync(fs, hypermedia);
            }
        }

        public async Task<Models.Hypermedia> UpdateStatusAsync(Models.Hypermedia hypermedia, Status origin)
        {
            return await UpdateHypermediaStatusAsync(hypermedia, origin);
        }

        private async Task<Models.Hypermedia> UpdateHypermediaStatusAsync(Models.Hypermedia hypermedia, Status origin)
        {

            bool isHypermediaValid = await IsHypermediaValidAsync(hypermedia);

            bool isAllFilesReady = true;
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                Models.File tmpFile = await UpdateFileStatusAsync(hypermedia.Files[i]);
                if (!await IsFileValidAsync(tmpFile))
                {
                    isAllFilesReady = false;
                }
                hypermedia.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = await UpdateDirectoryStatusAsync(hypermedia.Directories[i]);
                if (!await IsDirectoryValidAsync(tmpDirectory))
                {
                    isAllDirectoriesReady = false;
                }
                hypermedia.Directories[i] = tmpDirectory;
            }

            bool isAllHypermediasReady = true;
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                Models.Hypermedia tmpHypermedia = await UpdateHypermediaStatusAsync(hypermedia.Hypermedias[i], hypermedia.Hypermedias[i].Status);
                if (!await IsHypermediaValidAsync(tmpHypermedia))
                {
                    isAllHypermediasReady = false;
                }
                hypermedia.Hypermedias[i] = tmpHypermedia;
            }

            switch (origin)
            {
                case Status.Checking:
                    throw new ArgumentException($"{nameof(origin)} can't be equal to {nameof(Status.Checking)} during status update", nameof(origin));
                case Status.Completed:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = Status.Completed;
                    }
                    else
                    {
                        hypermedia.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Downloading;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixHypermediaFileSystemModel(hypermedia))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                hypermedia.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                hypermedia.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            hypermedia.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    throw new Exception($"{nameof(Hypermedia)} can't be reconstructed");
                case Status.Seeding:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        hypermedia.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    hypermedia.Status = Status.Stopped;
                    break;
                default:
                    hypermedia.Status = Status.Error;
                    break;
            }

            return hypermedia;
        }

        public async Task<Models.Directory> UpdateDirectoryStatusAsync(Models.Directory directory)
        {
            bool isDirectoryValid = await IsDirectoryValidAsync(directory);

            bool isAllFilesReady = true;
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                Models.File tmpFile = await UpdateFileStatusAsync(directory.Files[i]);
                if (!await IsFileValidAsync(tmpFile))
                {
                    isAllFilesReady = false;
                }
                directory.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = await UpdateDirectoryStatusAsync(directory.Directories[i]);
                if (!await IsDirectoryValidAsync(tmpDirectory))
                {
                    isAllDirectoriesReady = false;
                }
                directory.Directories[i] = tmpDirectory;
            }

            switch (directory.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(directory.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = Status.Completed;
                    }
                    else
                    {
                        directory.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Downloading;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixDirectoryFileSystemModel(directory))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                directory.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                directory.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            directory.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    throw new Exception($"{nameof(Directory)} can't be reconstructed");
                case Status.Seeding:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        directory.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    directory.Status = Status.Stopped;
                    break;
                default:
                    directory.Status = Status.Error;
                    break;
            }

            return directory;
        }

        public async Task<Models.File> UpdateFileStatusAsync(Models.File file)
        {
            bool isFileValid = await IsFileValidAsync(file);

            if (!file.IsSingleBlock)
            {
                bool isAllBlockReady = true;
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    Models.Block tmpBlock = await UpdateBlockStatusAsync(file.Blocks[i]);
                    if (!await IsBlockValidAsync(tmpBlock))
                    {
                        isAllBlockReady = false;
                    }
                    file.Blocks[i] = tmpBlock;
                }
                if (isAllBlockReady)
                {
                    if (file.Status == Status.Downloading)
                    {
                        foreach (var b in file.Blocks)
                        {
                            b.Status = Status.ReadyForReconstruction;
                        }
                        file.Status = Status.ReadyForReconstruction;
                    }
                    return file;
                }
            }

            switch (file.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(file.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isFileValid)
                    {
                        file.Status = Status.Completed;
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Downloading;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixFileFileSystemModel(file))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                file.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                file.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            file.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isFileValid)
                    {
                        file.Status = Status.ReadyForReconstruction;
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Seeding:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    file.Status = Status.Stopped;
                    break;
                default:
                    file.Status = Status.Error;
                    break;
            }

            return file;
        }

        public async Task<Models.Block> UpdateBlockStatusAsync(Models.Block block)
        {
            bool isBlockValid = await IsBlockValidAsync(block);

            switch (block.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(block.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isBlockValid)
                    {
                        block.Status = Status.Completed;
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Downloading;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixBlockFileSystemModel(block))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                block.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                block.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            block.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isBlockValid)
                    {
                        block.Status = Status.ReadyForReconstruction;
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Seeding:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    block.Status = Status.Stopped;
                    break;
                default:
                    block.Status = Status.Error;
                    break;
            }

            return block;
        }

        public Models.Hypermedia UpdateStatus(Models.Hypermedia hypermedia, Status origin)
        {
            return UpdateHypermediaStatus(hypermedia, origin);
        }

        private Models.Hypermedia UpdateHypermediaStatus(Models.Hypermedia hypermedia, Status origin)
        {
            bool isHypermediaValid = IsHypermediaValid(hypermedia);

            bool isAllFilesReady = true;
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                Models.File tmpFile = UpdateFileStatus(hypermedia.Files[i]);
                if (!IsFileValid(tmpFile))
                {
                    isAllFilesReady = false;
                }
                hypermedia.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = UpdateDirectoryStatus(hypermedia.Directories[i]);
                if (!IsDirectoryValid(tmpDirectory))
                {
                    isAllDirectoriesReady = false;
                }
                hypermedia.Directories[i] = tmpDirectory;
            }

            bool isAllHypermediasReady = true;
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                Models.Hypermedia tmpHypermedia = UpdateHypermediaStatus(hypermedia.Hypermedias[i], hypermedia.Hypermedias[i].Status);
                if (!IsHypermediaValid(tmpHypermedia))
                {
                    isAllHypermediasReady = false;
                }
                hypermedia.Hypermedias[i] = tmpHypermedia;
            }

            switch (origin)
            {
                case Status.Checking:
                    throw new ArgumentException($"{nameof(origin)} can't be equal to {nameof(Status.Checking)} during status update", nameof(origin));
                case Status.Completed:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = Status.Completed;
                    }
                    else
                    {
                        hypermedia.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Downloading;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixHypermediaFileSystemModel(hypermedia))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                hypermedia.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                hypermedia.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            hypermedia.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    throw new Exception($"{nameof(Hypermedia)} can't be reconstructed");
                case Status.Seeding:
                    if (isHypermediaValid && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            hypermedia.Status = Status.Seeding;
                        }
                        else
                        {
                            hypermedia.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        hypermedia.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    hypermedia.Status = Status.Stopped;
                    break;
                default:
                    hypermedia.Status = Status.Error;
                    break;
            }

            return hypermedia;
        }

        public Models.Directory UpdateDirectoryStatus(Models.Directory directory)
        {
            bool isDirectoryValid = IsDirectoryValid(directory);

            bool isAllFilesReady = true;
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                Models.File tmpFile = UpdateFileStatus(directory.Files[i]);
                if (!IsFileValid(tmpFile))
                {
                    isAllFilesReady = false;
                }
                directory.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = UpdateDirectoryStatus(directory.Directories[i]);
                if (!IsDirectoryValid(tmpDirectory))
                {
                    isAllDirectoriesReady = false;
                }
                directory.Directories[i] = tmpDirectory;
            }

            switch (directory.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(directory.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = Status.Completed;
                    }
                    else
                    {
                        directory.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Downloading;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixDirectoryFileSystemModel(directory))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                directory.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                directory.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            directory.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    throw new Exception($"{nameof(Directory)} can't be reconstructed");
                case Status.Seeding:
                    if (isDirectoryValid && isAllDirectoriesReady && isAllFilesReady)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            directory.Status = Status.Seeding;
                        }
                        else
                        {
                            directory.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        directory.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    directory.Status = Status.Stopped;
                    break;
                default:
                    directory.Status = Status.Error;
                    break;
            }

            return directory;
        }

        public Models.File UpdateFileStatus(Models.File file)
        {
            bool isFileValid = IsFileValid(file);

            if (!file.IsSingleBlock) 
            {
                bool isAllBlockReady = true;
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    Models.Block tmpBlock = UpdateBlockStatus(file.Blocks[i]);
                    if (!IsBlockValid(tmpBlock))
                    {
                        isAllBlockReady = false;
                    }
                    file.Blocks[i] = tmpBlock;
                }
                if (isAllBlockReady)
                {
                    if (file.Status == Status.Downloading)
                    {
                        foreach(var b in file.Blocks)
                        {
                            b.Status = Status.ReadyForReconstruction;
                        }
                        file.Status = Status.ReadyForReconstruction;
                    }
                    return file;
                }
            }

            switch (file.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(file.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isFileValid)
                    {
                        file.Status = Status.Completed;
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Downloading;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (CheckAndFixFileFileSystemModel(file))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                file.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                file.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            file.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isFileValid)
                    {
                        file.Status = Status.ReadyForReconstruction;
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Seeding:
                    if (isFileValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            file.Status = Status.Seeding;
                        }
                        else
                        {
                            file.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        file.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    file.Status = Status.Stopped;
                    break;
                default:
                    file.Status = Status.Error;
                    break;
            }

            return file;
        }

        public Models.Block UpdateBlockStatus(Models.Block block)
        {
            bool isBlockValid = IsBlockValid(block);

            switch(block.Status)
            {
                case Status.Checking:
                    throw new Exception($"{nameof(block.Status)} can't be equal to {nameof(Status.Checking)} during status update");
                case Status.Completed:
                    if (isBlockValid)
                    {
                        block.Status = Status.Completed;
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Connecting:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Downloading:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Downloading;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.Error:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if(CheckAndFixBlockFileSystemModel(block))
                        {
                            if (_manager.Engine().IsStarted)
                            {
                                block.Status = Status.ReadyForDownload;
                            }
                            else
                            {
                                block.Status = Status.Connecting;
                            }
                        }
                        else
                        {
                            block.Status = Status.Error;
                        }
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.ReadyForDownload;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isBlockValid)
                    {
                        block.Status = Status.ReadyForReconstruction;
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Seeding:
                    if (isBlockValid)
                    {
                        if (_manager.Engine().IsStarted)
                        {
                            block.Status = Status.Seeding;
                        }
                        else
                        {
                            block.Status = Status.Connecting;
                        }
                    }
                    else
                    {
                        block.Status = Status.Error;
                    }
                    break;
                case Status.Stopped:
                    block.Status = Status.Stopped;
                    break;
                default:
                    block.Status = Status.Error;
                    break;
            }

            return block;
        }
    }
}
