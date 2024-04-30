using System;
using System.Collections.Generic;
using System.IO;

namespace Zarp.Common.Cache
{
    internal abstract class FileCache<T>
    {
        private Dictionary<string, CachedFile> _Cache;

        public FileCache()
        {
            _Cache = new();
        }

        public bool Get(string path, out T? data)
        {
            try
            {
                FileInfo file = new(path);

                if (_Cache.TryGetValue(file.FullName, out CachedFile entry))
                {
                    // Entry found
                    if (entry.Updated)
                    {
                        // Entry up to date
                        data = entry.Data;
                    }
                    else if (TryFetch(file.FullName, out data))
                    {
                        // Update outdated entry
                        entry.Data = data;
                        entry.LastUpdate = entry.Info.LastWriteTime;
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
                    if (TryFetch(file.FullName, out data))
                    {
                        // Create new entry
                        _Cache.Add(file.FullName, new CachedFile(file, data));
                    }
                    else
                    {
                        // Failed to fetch
                        return false;
                    }
                }
            }
            catch
            {
                data = default;
                return false;
            }

            return true;
        }

        protected abstract bool TryFetch(string path, out T data);

        private struct CachedFile
        {
            public bool Updated
            {
                get
                {
                    Info.Refresh();
                    return LastUpdate == Info.LastWriteTime;
                }
            }

            public FileInfo Info;
            public DateTime LastUpdate;
            public T Data;

            public CachedFile(FileInfo info, T data)
            {
                Info = info;
                LastUpdate = info.LastWriteTime;
                Data = data;
            }
        }
    }
}
