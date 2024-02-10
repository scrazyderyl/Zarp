using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Zarp.Core.Datatypes
{
    internal class PresetCollection<T> : IPresetCollection where T : IPreset
    {
        private Dictionary<string, T> _Presets;

        public PresetCollection()
        {
            _Presets = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        public T this[string name]
        {
            get => _Presets[name];
            set => _Presets[name] = (T)value;
        }

        IPreset IPresetCollection.this[string name]
        {
            get => this[name];
            set => this[name] = (T)value;
        }

        public bool Add(IPreset preset) => _Presets.TryAdd(preset.Name, (T)preset);
        public bool Remove(string name) => _Presets.Remove(name);

        public bool Rename(string oldName, string newName)
        {
            if (!_Presets.TryGetValue(oldName, out T? preset) || !_Presets.TryAdd(newName, preset!))
            {
                return false;
            }

            _Presets.Remove(oldName);
            preset!.Name = newName;

            return true;
        }

        public bool Contains(string name) => _Presets.ContainsKey(name);

        public IPreset? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                IncludeFields = true
            });
        }

        public IEnumerator<string> GetEnumerator() => _Presets.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
