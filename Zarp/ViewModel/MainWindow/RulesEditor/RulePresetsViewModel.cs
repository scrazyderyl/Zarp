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
using Shell32;
using System.Diagnostics;

namespace Zarp.ViewModel.MainWindow.RulesEditor
{
    internal class RulePresetsViewModel : ObservableObject
    {
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand {  get; set; }
        public RelayCommand RenamePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand {  get; set; }
        public RelayCommand ExportPresetCommand { get; set; }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public ObservableCollection<RulePreset> RulePresets { get; set; }
        private int _SelectedPresetIndex;
        public int SelectedPresetIndex {
            get {  return _SelectedPresetIndex; }
            set
            {
                _SelectedPresetIndex = value;
                OnPresetSelectionChanged();
            }
        }
        public string RulesetPolicy {  get; set; }
        public ObservableCollection<ApplicationInfo> Rules { get; set; }

        public RulePresetsViewModel()
        {
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);

            RulePresets = new ObservableCollection<RulePreset>(Core.Zarp.RulePresetManager.GetPresets());
            _SelectedPresetIndex = -1;
            Rules = new ObservableCollection<ApplicationInfo>();
            RulesetPolicy = string.Empty;
        }

        private void OnPresetSelectionChanged()
        {
            if (SelectedPresetIndex == -1)
            {
                Rules = new ObservableCollection<ApplicationInfo>();
                RulesetPolicy = String.Empty;
            } else
            {
                RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
                Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset.GetApplicationRules());
                RulesetPolicy = SelectedPreset.IsApplicationWhitelist ? "Block all except" : "Allow all except";
            }

            OnPropertyChanged("Rules");
            OnPropertyChanged("RulesetPolicy");
        }

        public void CreatePreset(object? parameter)
        {
            Core.Zarp.DialogReturnValue = null;
            new CreateRulePresetView().ShowDialog();
            RulePreset? newPreset = (RulePreset)Core.Zarp.DialogReturnValue;

            if (newPreset == null)
            {
                return;
            }

            RulePresets = new ObservableCollection<RulePreset>(Core.Zarp.RulePresetManager.GetPresets());
            SelectedPresetIndex = RulePresets.Count - 1;
            OnPropertyChanged("SelectedPresetIndex");
            OnPropertyChanged("RulePresets");
        }

        public void RemovePreset(object? parameter)
        {
            RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
            Core.Zarp.RulePresetManager.Remove(SelectedPreset.Name);
            SelectedPresetIndex -= 1;
            RulePresets = new ObservableCollection<RulePreset>(Core.Zarp.RulePresetManager.GetPresets());
            OnPropertyChanged("SelectedPresetIndex");
            OnPropertyChanged("RulePresets");
        }

        public void RenamePreset(object? parmaeter)
        {

        }

        public void DuplicatePreset(object? parameter)
        {

        }

        public void ImportPreset(object? parameter)
        {

        }

        public void ExportPreset(object? parameter)
        {

        }

        public void OpenApplicationSelector(object? parameter)
        {
            Core.Zarp.DialogReturnValue = null;
            new ApplicationSelectorView().ShowDialog();
            List<ApplicationInfo> newRules = (List<ApplicationInfo>?)Core.Zarp.DialogReturnValue;

            if (newRules == null)
            {
                return;
            }

            RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
            SelectedPreset.AddApplicationRules(newRules);
            Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset.GetApplicationRules());

            OnPropertyChanged("Rules");
        }
    }
}
