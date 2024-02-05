using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Zarp.Core.Datatypes
{
    public interface PresetCollection : IEnumerable<string>
    {
        public Preset this[string name] { get; set; }
        public bool Add(Preset preset);
        public bool Remove(string name);
        public bool Rename(string name, string newName);
        public bool Contains(string name);
        public Preset? Deserialize(string json);
    }

    internal class PresetCollection<T> : PresetCollection where T : Preset
    {
        private Dictionary<string, T> Presets;

        public PresetCollection()
        {
            Presets = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        public T this[string name]
        {
            get => Presets[name];
            set => Presets[name] = (T)value;
        }

        Preset PresetCollection.this[string name]
        {
            get => this[name];
            set => this[name] = (T)value;
        }

        public bool Add(Preset preset) => Presets.TryAdd(preset.Name, (T)preset);
        public bool Remove(string name) => Presets.Remove(name);

        public bool Rename(string oldName, string newName)
        {
            if (!Presets.TryGetValue(oldName, out T? preset) || !Presets.TryAdd(newName, preset!))
            {
                return false;
            }

            Presets.Remove(oldName);
            preset!.Name = newName;

            return true;
        }

        public bool Contains(string name) => Presets.ContainsKey(name);

        public Preset? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                IncludeFields = true
            });
        }

        public IEnumerator<string> GetEnumerator() => Presets.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
