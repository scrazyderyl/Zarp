using System.Windows.Forms;

namespace Zarp.GUI.Util
{
    public class FileDialogs
    {
        public static string[] OpenExeMulti()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = ".exe",
                Filter = "Executable files (*.exe)|*.exe",
                Multiselect = true
            };

            fileDialog.ShowDialog();

            return fileDialog.FileNames;
        }

        public static string[] OpenJSONMulti()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = ".json",
                Filter = "JSON files (*.json)|*.json",
                Multiselect = true
            };

            fileDialog.ShowDialog();

            return fileDialog.FileNames;
        }

        public static string SaveJSON()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                DefaultExt = ".json",
                Filter = "JSON files (*.json)|*.json"
            };

            fileDialog.ShowDialog();

            return fileDialog.FileName;
        }
    }
}
