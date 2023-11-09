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
        public RelayCommand RenamePresetCommand {  get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }


        public ObservableCollection<RulePreset> RulePresets { get; set; }

        public RewardsViewModel()
        {
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
        }

        public void CreatePreset(object? parameter)
        {

        }

        public void RenamePreset(object? parameter)
        {

        }

        public void RemovePreset(object? parameter)
        {

        }

        public void ImportPreset(object? parameter)
        {

        }

        public void DuplicatePreset(object? parameter)
        {

        }

        public void ExportPreset(object? parameter)
        {

        }
    }
}
