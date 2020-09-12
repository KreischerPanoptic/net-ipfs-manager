using System;
using System.Collections.Generic;
using System.Text;

using Ipfs.Hypermedia;
using Ipfs.Engine;
using Ipfs.CoreApi;
using System.Security;

namespace Ipfs.Manager
{
    public class Manager : IDisposable
    {
        private static Manager _instance;
        private static object _sync = new Object();
        private static IpfsEngine _engine;

        protected Manager()
        {
            _engine = new IpfsEngine();
            _engine.Start();
        }

        protected Manager(string repoPath)
        {
            _engine = new IpfsEngine(repoPath);
            _engine.Start();
        }

        protected Manager(SecureString passphrase)
        {
            _engine = new IpfsEngine(passphrase);
            _engine.Start();
        }

        protected Manager(SecureString passphrase, string repoPath)
        {
            _engine = new IpfsEngine(passphrase, repoPath);
            _engine.Start();
        }

        protected Manager(char[] passphrase)
        {
            _engine = new IpfsEngine(passphrase);
            _engine.Start();
        }

        protected Manager(char[] passphrase, string repoPath)
        {
            _engine = new IpfsEngine(passphrase, repoPath);
            _engine.Start();
        }

        public static Manager Instance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager();
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager();
                        }
                }
            }
            return _instance;
        }

        public static Manager Instance(string repoPath)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager(repoPath);
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager(repoPath);
                        }
                }
            }
            return _instance;
        }

        public static Manager Instance(SecureString passphrase)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager(passphrase);
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager(passphrase);
                        }
                }
            }
            return _instance;
        }

        public static Manager Instance(SecureString passphrase, string repoPath)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager(passphrase, repoPath);
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager(passphrase, repoPath);
                        }
                }
            }
            return _instance;
        }

        public static Manager Instance(char[] passphrase)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager(passphrase);
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager(passphrase);
                        }
                }
            }
            return _instance;
        }

        public static Manager Instance(char[] passphrase, string repoPath)
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        if (_engine == null)
                        {
                            _instance = new Manager(passphrase, repoPath);
                        }
                        else
                        {
                            if (!_engine.IsStarted)
                                _instance = new Manager(passphrase, repoPath);
                        }
                }
            }
            return _instance;
        }

        public IpfsEngine Engine()
        {
            return _engine;
        }

        private void Init()
        {
            HypermediaService = new Services.Versions.HypermediaService.BaseHypermediaService(this);
            HypermediaServiceVersion = HypermediaService.GetType();
        }

        public Type HypermediaServiceVersion { get; set; }
        public Services.IHypermediaService HypermediaService { get; set; }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}
