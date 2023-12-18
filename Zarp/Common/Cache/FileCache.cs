using System;
using System.Collections.Generic;
using System.IO;

namespace Zarp.Common.Cache
{
    public abstract class FileCache<T>
    {
        private Dictionary<string, CachedFile> Cache;

        public FileCache()
        {
            Cache = new();
        }

        public bool Get(string path, out T data)
        {
            FileInfo file = new(path);

            if (Cache.TryGetValue(file.FullName, out CachedFile entry))
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
                    Cache.Add(file.FullName, new CachedFile(file, data));
                }
                else
                {
                    // Failed to fetch
                    return false;
                }
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
