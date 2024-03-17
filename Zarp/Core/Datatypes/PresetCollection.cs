using System;
using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal class PresetCollection<T> : IPresetCollection where T : Preset
    {
        private Dictionary<Guid, T> _Presets;
        private HashSet<string> _UsedNames;

        public PresetCollection()
        {
            _Presets = new Dictionary<Guid, T>();
            _UsedNames = new HashSet<string>();
        }

        public T this[Guid guid]
        {
            get => _Presets[guid];
            set => _Presets[guid] = value;
        }

        Preset IPresetCollection.this[Guid guid]
        {
            get => this[guid];
            set => this[guid] = (T)value;
        }

        public bool Add(Preset preset)
        {
            if (_UsedNames.Contains(preset._Name) || !_Presets.TryAdd(preset.Guid, (T)preset))
            {
                return false;
            }

            _UsedNames.Add(preset._Name);

            return true;
        }

        public bool Remove(Guid GUID)
        {
            if (!_Presets.Remove(GUID, out T? preset))
            {
                return false;
            }

            _UsedNames.Remove(preset!._Name);
            return false;
        }

        public bool Rename(Guid guid, string newName)
        {
            if (!_Presets.TryGetValue(guid, out T? preset) || !_UsedNames.Add(newName))
            {
                return false;
            }

            _UsedNames.Remove(preset._Name);
            preset!._Name = newName;
            return true;
        }

        public bool Contains(string name) => _UsedNames.Contains(name);
        public bool Contains(Guid guid) => _Presets.ContainsKey(guid);

        public IEnumerator<Preset> GetEnumerator()
        {
            foreach (Preset preset in _Presets.Values)
            {
                yield return preset;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
