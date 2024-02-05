using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Zarp.Common.Cache
{
    internal abstract class DirectoryFileListCache<T> : IListCache<T>
    {
        private FileSystemWatcher _Watcher;
        private Dictionary<string, T> _Cache;
        private HashSet<string> _OutdatedFiles;

        public int Count => _Cache.Count + _OutdatedFiles.Count;
        public bool Updated => _OutdatedFiles.Count == 0;

        public DirectoryFileListCache(string path, string filter, bool includeSubdirectories)
        {
            _Watcher = new FileSystemWatcher(path, filter)
            {
                IncludeSubdirectories = includeSubdirectories
            };
            _Cache = new Dictionary<string, T>();
            _OutdatedFiles = new HashSet<string>();

            string[] files = Directory.GetFiles(path, filter, includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // Populate
            foreach (string normalizedPath in files)
            {
                if (TryFetch(normalizedPath, out T data))
                {
                    _Cache.Add(normalizedPath, data);
                }
            }

            // Register events
            _Watcher.Changed += OnChanged;
            _Watcher.Created += OnCreated;
            _Watcher.Deleted += OnDeleted;
            _Watcher.Renamed += OnRenamed;
            _Watcher.Error += OnError;
            _Watcher.EnableRaisingEvents = true;
        }

        ~DirectoryFileListCache()
        {
            _Watcher.Dispose();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            if (_Cache.Remove(e.FullPath))
            {
                _OutdatedFiles.Add(e.FullPath);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (TryFetch(e.FullPath, out T data))
            {
                _Cache.Add(e.FullPath, data);
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (_Cache.Remove(e.FullPath) || _OutdatedFiles.Remove(e.FullPath))
            {
                return;
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            // Up to date files
            if (_Cache.Remove(e.OldFullPath, out T? entry))
            {
                _Cache.Add(e.FullPath, entry);
                return;
            }

            // Outdated files
            if (_OutdatedFiles.Remove(e.OldFullPath))
            {
                _OutdatedFiles.Add(e.FullPath);
                return;
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {

        }

        private void Update()
        {
            foreach (string file in _OutdatedFiles)
            {
                if (TryFetch(file, out T data))
                {
                    _Cache.Add(file, data);
                }
            }

            _OutdatedFiles = new HashSet<string>();
        }

        protected abstract bool TryFetch(string path, out T data);

        public IEnumerator<T> GetEnumerator()
        {
            if (!Updated)
            {
                Update();
            }

            return _Cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
