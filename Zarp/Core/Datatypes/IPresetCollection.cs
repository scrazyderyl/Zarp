using System;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal interface IPresetCollection : IEnumerable<Preset>
    {
        public Preset this[Guid guid] { get; set; }
        public bool Add(Preset preset);
        public bool Remove(Guid guid);
        public bool Rename(Guid guid, string newName);
        public bool Contains(string name);
        public bool Contains(Guid guid);
    }
}
