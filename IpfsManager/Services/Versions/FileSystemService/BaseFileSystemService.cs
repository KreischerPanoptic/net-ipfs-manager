using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ipfs.Manager.Cryptography;
using Ipfs.Manager.Models;

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
                string blockStorage = System.IO.Path.Combine(f.InternalPath, $"{f.Path}_blocks");
                //TODO check if block storage path is correct 
                foreach(var b in f.Blocks)
                {
                    if(new string(b.InternalPath.Reverse().SkipWhile(x => x != '\\').Skip(1).Reverse().ToArray()) != blockStorage)
                    {
                        return false;
                    }
                }
                pathes.Add(blockStorage);
            }

            foreach(var p in pathes)
            {
                try
                {
                    System.IO.Directory.Delete(p, true);
                }
                catch(Exception e)
                {
                    //TODO add logging of exceptions
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
                catch (Exception e)
                {
                    hypermedia.Status = Status.Error;
                    return false;
                }
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                if (!CheckAndFixHypermediaFileSystemModel(h))
                {
                    h.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                h.Status = Status.ReadyForDownload;
            }

            foreach (var d in hypermedia.Directories)
            {
                if (!CheckAndFixDirectoryFileSystemModel(d))
                {
                    d.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                d.Status = Status.ReadyForDownload;
            }

            foreach (var f in hypermedia.Files)
            {
                if (!CheckAndFixFileFileSystemModel(f))
                {
                    f.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                f.Status = Status.ReadyForDownload;
            }

            hypermedia.Status = Status.ReadyForDownload;
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
                catch (Exception e)
                {
                    directory.Status = Status.Error;
                    return false;
                }
            }

            foreach (var d in directory.Directories)
            {
                if (!CheckAndFixDirectoryFileSystemModel(d))
                {
                    d.Status = Status.Error;
                    directory.Status = Status.Error;
                    return false;
                }
                d.Status = Status.ReadyForDownload;
            }

            foreach (var f in directory.Files)
            {
                if (!CheckAndFixFileFileSystemModel(f))
                {
                    f.Status = Status.Error;
                    directory.Status = Status.Error;
                    return false;
                }
                f.Status = Status.ReadyForDownload;
            }
            directory.Status = Status.ReadyForDownload;
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
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(file.InternalPath, $"{file.Path}_blocks")))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(file.InternalPath, $"{file.Path}_blocks"));
                    }
                }
                catch
                {
                    file.Status = Status.Error;
                    return false;
                }
            }

            foreach (var b in file.Blocks)
            {
                if (!CheckAndFixBlockFileSystemModel(b))
                {
                    b.Status = Status.Error;
                    file.Status = Status.Error;
                    return false;
                }
                b.Status = Status.ReadyForDownload;
            }
            file.Status = Status.ReadyForDownload;
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
                }
                catch
                {
                    block.Status = Status.Error;
                    return false;
                }
            }
            block.Status = Status.ReadyForDownload;
            return true;
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
                System.IO.Directory.CreateDirectory(basePath);
            }
            catch (Exception e)
            {
                hypermedia.Status = Status.Error;
                return false;
            }

            foreach (var h in hypermedia.Hypermedias)
            {
                if (!CreateHypermediaFileSystemModel(h))
                {
                    h.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                h.Status = Status.ReadyForDownload;
            }

            foreach (var d in hypermedia.Directories)
            {
                if (!CreateDirectoryFileSystemModel(d))
                {
                    d.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                d.Status = Status.ReadyForDownload;
            }

            foreach (var f in hypermedia.Files)
            {
                if (!CreateFileFileSystemModel(f))
                {
                    f.Status = Status.Error;
                    hypermedia.Status = Status.Error;
                    return false;
                }
                f.Status = Status.ReadyForDownload;
            }

            hypermedia.Status = Status.ReadyForDownload;
            return true;
        }

        private bool CreateDirectoryFileSystemModel(Models.Directory directory)
        {
            string basePath = directory.InternalPath;
            try
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            catch (Exception e)
            {
                directory.Status = Status.Error;
                return false;
            }

            foreach (var d in directory.Directories)
            {
                if (!CreateDirectoryFileSystemModel(d))
                {
                    d.Status = Status.Error;
                    directory.Status = Status.Error;
                    return false;
                }
                d.Status = Status.ReadyForDownload;
            }

            foreach (var f in directory.Files)
            {
                if (!CreateFileFileSystemModel(f))
                {
                    f.Status = Status.Error;
                    directory.Status = Status.Error;
                    return false;
                }
                f.Status = Status.ReadyForDownload;
            }
            directory.Status = Status.ReadyForDownload;
            return true;
        }

        private bool CreateFileFileSystemModel(Models.File file)
        {
            string basePath = file.InternalPath;
            try
            {
                System.IO.File.Create(basePath);
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(file.InternalPath, $"{file.Path}_blocks"));
            }
            catch (Exception e)
            {
                file.Status = Status.Error;
                return false;
            }

            foreach (var b in file.Blocks)
            {
                if (!CreateBlockFileSystemModel(b))
                {
                    b.Status = Status.Error;
                    file.Status = Status.Error;
                    return false;
                }
                b.Status = Status.ReadyForDownload;
            }
            file.Status = Status.ReadyForDownload;
            return true;
        }

        private bool CreateBlockFileSystemModel(Models.Block block)
        {
            string basePath = block.InternalPath;
            try
            {
                System.IO.File.Create(basePath);
            }
            catch (Exception e)
            {
                block.Status = Status.Error;
                return false;
            }
            block.Status = Status.ReadyForDownload;
            return true;
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia)
        {
            return DeleteHypermedia(hypermedia, false);
        }

        public bool DeleteHypermedia(Models.Hypermedia hypermedia, bool isFileDeletionRequested)
        {
            if (!System.IO.Directory.Exists(hypermedia.InternalPath))
            {
                throw new ArgumentException("Hypermedia download path does not exist", nameof(hypermedia));
            }

            foreach (var f in hypermedia.Files)
            {
                if (!UnpinFile(f))
                {
                    return false;
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    return false;
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!UnpinHypermedia(h))
                {
                    return false;
                }
            }

            if(isFileDeletionRequested)
            {
                try
                {
                    System.IO.Directory.Delete(hypermedia.InternalPath, true);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }

        private bool UnpinHypermedia(Models.Hypermedia hypermedia)
        {
            foreach (var f in hypermedia.Files)
            {
                if (!UnpinFile(f))
                {
                    return false;
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    return false;
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!UnpinHypermedia(h))
                {
                    return false;
                }
            }
            return true;
        }

        private bool UnpinDirectory(Models.Directory directory)
        {
            foreach (var f in directory.Files)
            {
                if (!UnpinFile(f))
                {
                    return false;
                }
            }
            foreach (var d in directory.Directories)
            {
                if (!UnpinDirectory(d))
                {
                    return false;
                }
            }
            return true;
        }

        private bool UnpinFile(Models.File file)
        {
            if (!file.IsSingleBlock)
            {
                foreach (var b in file.Blocks)
                {
                    if (!UnpinBlock(b))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                var r = _manager.Engine().Pin.RemoveAsync(file.Path).Result;
                return r.First() == file.Path;
            }
        }

        private bool UnpinBlock(Models.Block block)
        {
            var r = _manager.Engine().Pin.RemoveAsync(block.Path).Result;
            //TODO check
            return r.First() == block.Path;
        }

        public bool IsBlockValid(Models.Block block)
        {
            string blockPath = block.InternalPath;
            if(!System.IO.File.Exists(blockPath))
            {
                throw new ArgumentException("Block is not downloaded yet, or misconfigurated", nameof(block));
            }

            List<byte> bytes = new List<byte>();
            try
            {
                using (var fs = new System.IO.FileStream(blockPath, System.IO.FileMode.Open))
                {
                    bool isEndOfStream = false;
                    while (!isEndOfStream)
                    {
                        int b = fs.ReadByte();
                        if (b != -1)
                        {
                            bytes.Add((byte)b);
                        }
                        else
                        {
                            isEndOfStream = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new AggregateException("Unknown error encountered", e);
            }

            KeccakManaged keccak = new KeccakManaged(512);
            var buf = keccak.ComputeHash(bytes.ToArray());

            StringBuilder sb = new StringBuilder();
            foreach (var b in buf)
            {
                sb.Append(b.ToString("X2"));
            }
            string internalHash = sb.ToString();

            //TODO check if works
            if(internalHash == block.Hash)
            {
                block.Status = Status.Seeding;
            }
            return internalHash == block.Hash;
        }

        public bool IsDirectoryValid(Models.Directory directory)
        {
            if (!System.IO.Directory.Exists(directory.InternalPath))
            {
                throw new ArgumentException("Directory is not downloaded yet, or misconfigurated", nameof(directory));
            }

            foreach(var f in directory.Files)
            {
                if (!IsFileValid(f))
                {
                    return false;
                }
            }
            foreach (var d in directory.Directories)
            {
                if (!IsDirectoryValid(d))
                {
                    return false;
                }
            }

            directory.Status = Status.Seeding;
            //TODO check if I can fix it, because right now it's unknown in which order entities were translated to Files and Directories
            return true;
        }

        public bool IsFileValid(Models.File file)
        {
            if (!System.IO.File.Exists(file.InternalPath))
            {
                throw new ArgumentException("File is not downloaded yet, or misconfigurated", nameof(file));
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

                //TODO check if works
                if (internalHash == file.Hash)
                {
                    file.Status = Status.Seeding;
                }
                else
                {
                    file.Status = Status.ReadyForReconstruction;
                }
                return internalHash == file.Hash;
            }
            else
            {
                List<byte> bytes = new List<byte>();

                try
                {
                    using (var fs = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Open))
                    {
                        bool isEndOfStream = false;
                        while (!isEndOfStream)
                        {
                            int b = fs.ReadByte();
                            if (b != -1)
                            {
                                bytes.Add((byte)b);
                            }
                            else
                            {
                                isEndOfStream = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new AggregateException("Unknown error encountered", e);
                }

                var buf = keccak.ComputeHash(bytes.ToArray());

                StringBuilder sb = new StringBuilder();
                foreach (var b in buf)
                {
                    sb.Append(b.ToString("X2"));
                }
                string internalHash = sb.ToString();

                //TODO check if works
                if (internalHash == file.Hash)
                {
                    file.Status = Status.Seeding;
                }
                return internalHash == file.Hash;
            }
        }

        public bool IsHypermediaValid(Models.Hypermedia hypermedia)
        {
            if (!System.IO.Directory.Exists(hypermedia.InternalPath))
            {
                throw new ArgumentException("Hypermedia is not downloaded yet, or misconfigurated", nameof(hypermedia));
            }

            foreach (var f in hypermedia.Files)
            {
                if (!IsFileValid(f))
                {
                    return false;
                }
            }
            foreach (var d in hypermedia.Directories)
            {
                if (!IsDirectoryValid(d))
                {
                    return false;
                }
            }
            foreach (var h in hypermedia.Hypermedias)
            {
                if (!IsHypermediaValid(h))
                {
                    return false;
                }
            }

            hypermedia.Status = Status.Seeding;
            //TODO check if I can fix it, because right now it's unknown in which order entities were translated to Files and Directories
            return true;
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
                catch(Exception e)
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
                catch (Exception e)
                {
                    throw new ArgumentException("Deserialization error encountered while processing mentioned file", nameof(path));
                }
            }
            return hypermedia;
        }

        public bool ReconstructFile(Models.File file)
        {
            System.IO.FileStream fileStream = null;
            List<byte> buffer = new List<byte>();
            foreach (var block in file.Blocks)
            {
                buffer.Clear();
                try
                {
                    using (var stream = new System.IO.FileStream(block.InternalPath, System.IO.FileMode.Open))
                    {
                        bool isEndOfStream = false;
                        while (!isEndOfStream)
                        {
                            int b = stream.ReadByte();
                            if (b != -1)
                            {
                                buffer.Add((byte)b);
                            }
                            else
                            {
                                isEndOfStream = true;
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }

                try
                {
                    //TODO check if it works
                    using (fileStream = new System.IO.FileStream(file.InternalPath, System.IO.FileMode.Append, System.IO.FileAccess.Write))
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

        public async Task<Models.Hypermedia> UpdateStatusAsync(Models.Hypermedia hypermedia)
        {
            return await UpdateHypermediaStatusAsync(hypermedia);
        }

        private async Task<Models.Hypermedia> UpdateHypermediaStatusAsync(Models.Hypermedia hypermedia)
        {
            string hypermediaPath = hypermedia.InternalPath;

            bool isHypermediaValid = await IsHypermediaValidAsync(hypermedia);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(hypermediaPath).Length == hypermedia.Size;

            bool isAllFilesReady = true;
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                Models.File tmpFile = await UpdateFileStatusAsync(hypermedia.Files[i]);
                if (tmpFile.Status == Status.Checking || tmpFile.Status == Status.Connecting || tmpFile.Status == Status.Downloading || tmpFile.Status == Status.Error || tmpFile.Status == Status.ReadyForDownload || tmpFile.Status == Status.ReadyForReconstruction || tmpFile.Status == Status.Stopped)
                {
                    if (! await IsFileValidAsync(tmpFile))
                    {
                        isAllFilesReady = false;
                    }
                }
                hypermedia.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = await UpdateDirectoryStatusAsync(hypermedia.Directories[i]);
                if (tmpDirectory.Status == Status.Checking || tmpDirectory.Status == Status.Connecting || tmpDirectory.Status == Status.Downloading || tmpDirectory.Status == Status.Error || tmpDirectory.Status == Status.ReadyForDownload || tmpDirectory.Status == Status.Stopped)
                {
                    if (! await IsDirectoryValidAsync(tmpDirectory))
                    {
                        isAllDirectoriesReady = false;
                    }
                }
                hypermedia.Directories[i] = tmpDirectory;
            }

            bool isAllHypermediasReady = true;
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                Models.Hypermedia tmpHypermedia = await UpdateHypermediaStatusAsync(hypermedia.Hypermedias[i]);
                if (tmpHypermedia.Status == Status.Checking || tmpHypermedia.Status == Status.Connecting || tmpHypermedia.Status == Status.Downloading || tmpHypermedia.Status == Status.Error || tmpHypermedia.Status == Status.ReadyForDownload || tmpHypermedia.Status == Status.Stopped)
                {
                    if (! await IsHypermediaValidAsync(tmpHypermedia))
                    {
                        isAllHypermediasReady = false;
                    }
                }
                hypermedia.Hypermedias[i] = tmpHypermedia;
            }

            Status tmpStatus = Status.Seeding;
            switch (hypermedia.Status)
            {
                case Status.Checking:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    hypermedia.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
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

        private async Task<Models.Directory> UpdateDirectoryStatusAsync(Models.Directory directory)
        {
            string directoryPath = directory.InternalPath;

            bool isDirectoryValid = await IsDirectoryValidAsync(directory);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(directoryPath).Length == directory.Size;

            bool isAllFilesReady = true;
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                Models.File tmpFile = await UpdateFileStatusAsync(directory.Files[i]);
                if (tmpFile.Status == Status.Checking || tmpFile.Status == Status.Connecting || tmpFile.Status == Status.Downloading || tmpFile.Status == Status.Error || tmpFile.Status == Status.ReadyForDownload || tmpFile.Status == Status.ReadyForReconstruction || tmpFile.Status == Status.Stopped)
                {
                    if ( !await IsFileValidAsync(tmpFile))
                    {
                        isAllFilesReady = false;
                    }
                }
                directory.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = await UpdateDirectoryStatusAsync(directory.Directories[i]);
                if (tmpDirectory.Status == Status.Checking || tmpDirectory.Status == Status.Connecting || tmpDirectory.Status == Status.Downloading || tmpDirectory.Status == Status.Error || tmpDirectory.Status == Status.ReadyForDownload || tmpDirectory.Status == Status.Stopped)
                {
                    if (! await IsDirectoryValidAsync(tmpDirectory))
                    {
                        isAllDirectoriesReady = false;
                    }
                }
                directory.Directories[i] = tmpDirectory;
            }

            Status tmpStatus = Status.Seeding;
            switch (directory.Status)
            {
                case Status.Checking:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    directory.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
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

        //TODO check what will happen if file is completed or seeding
        private async Task<Models.File> UpdateFileStatusAsync(Models.File file)
        {
            string filePath = file.InternalPath;

            bool isFileValid = await IsFileValidAsync(file);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(filePath).Length == file.Size;

            if (!file.IsSingleBlock)
            {
                bool isAllBlockReady = true;
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    Models.Block tmpBlock = await UpdateBlockStatusAsync(file.Blocks[i]);
                    if (tmpBlock.Status == Status.Checking || tmpBlock.Status == Status.Connecting || tmpBlock.Status == Status.Downloading || tmpBlock.Status == Status.Error || tmpBlock.Status == Status.ReadyForDownload || tmpBlock.Status == Status.Stopped)
                    {
                        if (! await IsBlockValidAsync(tmpBlock))
                        {
                            isAllBlockReady = false;
                        }
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
                }
                return file;
            }

            Status tmpStatus = Status.Seeding;
            switch (file.Status)
            {
                case Status.Checking:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    file.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
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

        private async Task<Models.Block> UpdateBlockStatusAsync(Models.Block block)
        {
            string blockPath = block.InternalPath;

            bool isBlockValid = await IsBlockValidAsync(block);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(blockPath).Length == block.Size;

            Status tmpStatus = Status.Seeding;
            switch (block.Status)
            {
                case Status.Checking:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    block.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    block.Status = Status.ReadyForReconstruction;
                    break;
                case Status.Seeding:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
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

        public Models.Hypermedia UpdateStatus(Models.Hypermedia hypermedia)
        {
            return UpdateHypermediaStatus(hypermedia);
        }
        
        private Models.Hypermedia UpdateHypermediaStatus(Models.Hypermedia hypermedia)
        {
            string hypermediaPath = hypermedia.InternalPath;

            bool isHypermediaValid = IsHypermediaValid(hypermedia);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(hypermediaPath).Length == hypermedia.Size;

            bool isAllFilesReady = true;
            for (int i = 0; i < hypermedia.Files.Count; ++i)
            {
                Models.File tmpFile = UpdateFileStatus(hypermedia.Files[i]);
                if (tmpFile.Status == Status.Checking || tmpFile.Status == Status.Connecting || tmpFile.Status == Status.Downloading || tmpFile.Status == Status.Error || tmpFile.Status == Status.ReadyForDownload || tmpFile.Status == Status.ReadyForReconstruction || tmpFile.Status == Status.Stopped)
                {
                    if (!IsFileValid(tmpFile))
                    {
                        isAllFilesReady = false;
                    }
                }
                hypermedia.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < hypermedia.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory = UpdateDirectoryStatus(hypermedia.Directories[i]);
                if (tmpDirectory.Status == Status.Checking || tmpDirectory.Status == Status.Connecting || tmpDirectory.Status == Status.Downloading || tmpDirectory.Status == Status.Error || tmpDirectory.Status == Status.ReadyForDownload || tmpDirectory.Status == Status.Stopped)
                {
                    if (!IsDirectoryValid(tmpDirectory))
                    {
                        isAllDirectoriesReady = false;
                    }
                }
                hypermedia.Directories[i] = tmpDirectory;
            }

            bool isAllHypermediasReady = true;
            for (int i = 0; i < hypermedia.Hypermedias.Count; ++i)
            {
                Models.Hypermedia tmpHypermedia = UpdateHypermediaStatus(hypermedia.Hypermedias[i]);
                if (tmpHypermedia.Status == Status.Checking || tmpHypermedia.Status == Status.Connecting || tmpHypermedia.Status == Status.Downloading || tmpHypermedia.Status == Status.Error || tmpHypermedia.Status == Status.ReadyForDownload || tmpHypermedia.Status == Status.Stopped)
                {
                    if (!IsHypermediaValid(tmpHypermedia))
                    {
                        isAllHypermediasReady = false;
                    }
                }
                hypermedia.Hypermedias[i] = tmpHypermedia;
            }

            Status tmpStatus = Status.Seeding;
            switch (hypermedia.Status)
            {
                case Status.Checking:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    hypermedia.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isHypermediaValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady && isAllHypermediasReady)
                    {
                        hypermedia.Status = tmpStatus;
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

        private Models.Directory UpdateDirectoryStatus(Models.Directory directory)
        {
            string directoryPath = directory.InternalPath;

            bool isDirectoryValid = IsDirectoryValid(directory);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(directoryPath).Length == directory.Size;

            bool isAllFilesReady = true;
            for (int i = 0; i < directory.Files.Count; ++i)
            {
                Models.File tmpFile = UpdateFileStatus(directory.Files[i]);
                if (tmpFile.Status == Status.Checking || tmpFile.Status == Status.Connecting || tmpFile.Status == Status.Downloading || tmpFile.Status == Status.Error || tmpFile.Status == Status.ReadyForDownload || tmpFile.Status == Status.ReadyForReconstruction || tmpFile.Status == Status.Stopped)
                {
                    if (!IsFileValid(tmpFile))
                    {
                        isAllFilesReady = false;
                    }
                }
                directory.Files[i] = tmpFile;
            }

            bool isAllDirectoriesReady = true;
            for (int i = 0; i < directory.Directories.Count; ++i)
            {
                Models.Directory tmpDirectory  = UpdateDirectoryStatus(directory.Directories[i]);
                if (tmpDirectory.Status == Status.Checking || tmpDirectory.Status == Status.Connecting || tmpDirectory.Status == Status.Downloading || tmpDirectory.Status == Status.Error || tmpDirectory.Status == Status.ReadyForDownload || tmpDirectory.Status == Status.Stopped)
                {
                    if (!IsDirectoryValid(tmpDirectory))
                    {
                        isAllDirectoriesReady = false;
                    }
                }
                directory.Directories[i] = tmpDirectory;
            }

            Status tmpStatus = Status.Seeding;
            switch (directory.Status)
            {
                case Status.Checking:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    directory.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isDirectoryValid && isSizeMatches && isAllDirectoriesReady && isAllFilesReady)
                    {
                        directory.Status = tmpStatus;
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

        //TODO check what will happen if file is completed or seeding
        private Models.File UpdateFileStatus(Models.File file)
        {
            string filePath = file.InternalPath;

            bool isFileValid = IsFileValid(file);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(filePath).Length == file.Size;

            if (!file.IsSingleBlock) 
            {
                bool isAllBlockReady = true;
                for (int i = 0; i < file.Blocks.Count; ++i)
                {
                    Models.Block tmpBlock = UpdateBlockStatus(file.Blocks[i]);
                    if (tmpBlock.Status == Status.Checking || tmpBlock.Status == Status.Connecting || tmpBlock.Status == Status.Downloading || tmpBlock.Status == Status.Error || tmpBlock.Status == Status.ReadyForDownload || tmpBlock.Status == Status.Stopped)
                    {
                        if (!IsBlockValid(tmpBlock))
                        {
                            isAllBlockReady = false;
                        }
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
                }
                return file;
            }

            Status tmpStatus = Status.Seeding;
            switch (file.Status)
            {
                case Status.Checking:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    file.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
                    }
                    break;
                case Status.Seeding:
                    if (isFileValid && isSizeMatches)
                    {
                        file.Status = tmpStatus;
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

        private Models.Block UpdateBlockStatus(Models.Block block)
        {
            string blockPath = block.InternalPath;

            bool isBlockValid = IsBlockValid(block);
            //TODO check if works
            bool isSizeMatches = new System.IO.FileInfo(blockPath).Length == block.Size;

            Status tmpStatus = Status.Seeding;
            switch(block.Status)
            {
                case Status.Checking:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Completed:
                    block.Status = Status.Completed;
                    break;
                case Status.Connecting:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Downloading:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.Error:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForDownload:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
                    }
                    break;
                case Status.ReadyForReconstruction:
                    block.Status = Status.ReadyForReconstruction;
                    break;
                case Status.Seeding:
                    if (isBlockValid && isSizeMatches)
                    {
                        block.Status = tmpStatus;
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
