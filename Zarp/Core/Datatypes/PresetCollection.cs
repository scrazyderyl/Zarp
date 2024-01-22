using System;
using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    public interface PresetCollection : IEnumerable<string>
    {
        public Preset this[string name] { get; set; }
        public bool Add(Preset preset);
        public bool Remove(string name);
        public bool Rename(string name, string newName);
        public bool ContainsKey(string name);
    }

    internal class PresetCollection<T> : Dictionary<string, T>, PresetCollection where T : Preset
    {
        public PresetCollection() : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        Preset PresetCollection.this[string name]
        {
            get => this[name];
            set => this[name] = (T)value;
        }

        public bool Add(Preset preset) => TryAdd(preset.Name, (T)preset);

        public bool Rename(string oldName, string newName)
        {
            if (!TryGetValue(oldName, out T? preset) || !TryAdd(newName, preset!))
            {
                return false;
            }

            Remove(oldName);
            preset!.Name = newName;

            return true;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
