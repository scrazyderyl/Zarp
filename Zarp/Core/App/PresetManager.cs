using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using Zarp.Core.Datatypes;

namespace Zarp.Core.Service
{
    internal class PresetManager<T> where T : Preset
    {
        private Dictionary<string, T> Presets;

        public PresetManager()
        {
            Presets = new Dictionary<string, T>();
        }

        private static string[] OpenJsonMulti()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();

            return fileDialog.FileNames;
        }

        private static void SaveJson(JsonObject json)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.ShowDialog();
        }

        public bool Add(T preset)
        {
            return Presets.TryAdd(preset.Name, preset);
        }

        public void Remove(string name)
        {
            Presets.Remove(name);
        }

        public IEnumerable<T> GetPresets()
        {
            return Presets.Values;
        }
    }
}
