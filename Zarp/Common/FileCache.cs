using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Common
{
    public abstract class FileCache<T>
    {
        protected Dictionary<string, CachedFileData<T>> Cache;

        public FileCache()
        {
            Cache = new Dictionary<string, CachedFileData<T>>();
        }

        public bool Get(string path, out T? data)
        {
            FileInfo file = new FileInfo(path);

            if (Cache.TryGetValue(file.FullName, out CachedFileData<T>? entry))
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
                    entry = new CachedFileData<T>(file, data);
                    Cache.Add(file.FullName, entry);
                }
                else
                {
                    // Failed to fetch
                    return false;
                }
            }

            return true;
        }

        protected abstract bool TryFetch(string path, out T? data);
    }
}
