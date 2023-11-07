using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel.MainWindow.RulesEditor
{
    internal class RewardsViewModel
    {
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }


        public static ObservableCollection<RulePreset> RulePresets { get; } = new ObservableCollection<RulePreset>();

        public RewardsViewModel()
        {
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
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

        public void DuplicatePreset(object? parameter)
        {
            new TextInputView().ShowDialog();
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
