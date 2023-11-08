using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace Zarp.Core
{
    public class PresetManager<T> where T : Preset
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
            try
            {
                Presets.Add(preset.Name, preset);
            } catch {
                return false;
            }

            return true;
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
