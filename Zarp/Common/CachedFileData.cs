using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Common
{
    public class CachedFileData<T>
    {
        private readonly FileInfo _File;
        private DateTime _LastUpdate;
        private T _Data;

        public bool Updated => _LastUpdate >= _File.LastWriteTime;
        public T Data
        {
            get
            {  
                return _Data;
            }

            set
            {
                _Data = value;
                _LastUpdate = DateTime.Now;
            }
        }

        public CachedFileData(FileInfo file, T data)
        {
            _File = file;
            _Data = data;
            _LastUpdate = DateTime.Now;
        }
    }
}
