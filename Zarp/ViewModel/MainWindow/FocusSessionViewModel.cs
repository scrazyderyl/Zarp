using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel.MainWindow
{
    internal class FocusSessionViewModel
    {
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }

        public ObservableCollection<string> Rules { get; set; }

        public FocusSessionViewModel()
        {
            Rules = new ObservableCollection<string>();

            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
        }

        public void CreatePreset(object? parameter)
        {
            new TextInputView().ShowDialog();
        }

        public void RemovePreset(object? parameter)
        {

        }

        public void ImportPreset(object? parameter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();
        }

        public void ExportPreset(object? parameter)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.ShowDialog();
        }
    }
}
