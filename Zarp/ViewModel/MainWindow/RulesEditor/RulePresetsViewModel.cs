using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zarp.Core;
using Zarp.View;
using Zarp;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zarp.ViewModel.MainWindow.RulesEditor
{

    internal class RulePresetsViewModel
    {
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand {  get; set; }
        public RelayCommand ImportPresetCommand {  get; set; }
        public RelayCommand ExportPresetCommand { get; set; }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public ObservableCollection<string> Rules { get; set; }

        public RulePresetsViewModel()
        {
            Rules = new ObservableCollection<string>();

            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);
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

        public void OpenApplicationSelector(object? parameter)
        {
            new ApplicationSelectorView().ShowDialog();
        }
    }
}
