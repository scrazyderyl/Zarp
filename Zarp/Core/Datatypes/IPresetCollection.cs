using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal interface IPresetCollection : IEnumerable<string>
    {
        public IPreset this[string name] { get; set; }
        public bool Add(IPreset preset);
        public bool Remove(string name);
        public bool Rename(string name, string newName);
        public bool Contains(string name);
        public IPreset? Deserialize(string json);
    }
}
