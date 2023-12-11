using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Shapes;
using Zarp.Core.Datatypes;

namespace Zarp.Common
{
    public abstract class DirectoryListCache<T> : ListCache<T>
    {
        private readonly DirectoryInfo _Directory;
        private DateTime _LastUpdated;
        private Dictionary<string, CachedFileData<T>> _Cache;

        public bool IsUpdated => _LastUpdated >= _Directory.LastWriteTime;
        public int Count => _Cache.Count;

        public DirectoryListCache(string path)
        {
            _Directory = new DirectoryInfo(path);
            _Cache = new Dictionary<string, CachedFileData<T>>();

            Populate();
        }

        protected abstract IEnumerable<FileInfo> GetFiles(DirectoryInfo directory);

        public void Populate()
        {
            foreach (FileInfo file in GetFiles(_Directory))
            {
                if (TryFetch(file, out T value))
                {
                    CachedFileData<T> entry = new CachedFileData<T>(file, value);
                    _Cache.Add(file.FullName, entry);
                }
            }
        }

        public void Update()
        {
            HashSet<string> foundFiles = new HashSet<string>();

            // Add/update files
            foreach (FileInfo file in GetFiles(_Directory))
            {
                if (Update(file))
                {
                    foundFiles.Add(file.FullName);
                }
            }

            // Remove deleted files
            foreach (string fileName in _Cache.Keys)
            {
                if (!foundFiles.Contains(fileName))
                {
                    _Cache.Remove(fileName);
                }
            }

            _LastUpdated = DateTime.Now;
        }

        private bool Update(FileInfo file)
        {
            if (_Cache.TryGetValue(file.FullName, out CachedFileData<T>? entry))
            {
                // Entry found
                if (entry.Updated)
                {

                }
                else if (TryFetch(file, out T data))
                {
                    // Update outdated entry
                    entry.Data = data;
                }
                else
                {
                    // Failed to fetch
                    return false;
                }
            }
            else
            {
                // No entry found
                if (TryFetch(file, out T data))
                {
                    // Create new entry
                    entry = new CachedFileData<T>(file, data);
                    _Cache.Add(file.FullName, entry);
                }
                else
                {
                    // Failed to fetch
                    return false;
                }
            }

            return true;
        }

        protected abstract bool TryFetch(FileInfo file, out T data);

        public IEnumerator<T> GetEnumerator()
        {
            foreach (CachedFileData<T> entry in _Cache.Values)
            {
                yield return entry.Data;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
