using System;
using System.Collections.ObjectModel;
using System.Windows;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RulePresetsViewModel : ObservableObject
    {
        public static IPresetCollection PresetCollection => Service.RulePresets;
        public static Func<IPreset?> CreateFunction => Create;

        private RulePreset? _SelectedPreset;
        public RulePreset? SelectedPreset
        {
            get => _SelectedPreset;
            set
            {
                _SelectedPreset = value;
                OnPresetSelectionChanged();
            }
        }

        public Visibility EditorVisibility { get; set; }
        public string RulesetPolicy { get; set; }
        public ObservableCollection<ApplicationInfo> Rules { get; set; }
        private int _SelectedRuleIndex;
        public int SelectedRuleIndex
        {
            get => _SelectedRuleIndex;
            set
            {
                SelectedPreset!.ApplicationRules.Remove(Rules[value].Id);
                Rules.RemoveAt(value);
            }
        }

        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public RulePresetsViewModel()
        {
            EditorVisibility = Visibility.Hidden;
            Rules = new ObservableCollection<ApplicationInfo>();
            _SelectedRuleIndex = -1;
            RulesetPolicy = string.Empty;

            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);
        }

        public static IPreset? Create()
        {
            Service.DialogReturnValue = null;
            new CreateRulePresetView().ShowDialog();
            return (IPreset?)Service.DialogReturnValue;
        }

        private void OnPresetSelectionChanged()
        {
            if (_SelectedPreset == null)
            {
                Rules = new ObservableCollection<ApplicationInfo>();
                RulesetPolicy = string.Empty;
                EditorVisibility = Visibility.Hidden;
            }
            else
            {
                Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset!.ApplicationRules);
                RulesetPolicy = SelectedPreset.ApplicationRules.IsWhitelist ? "Block all except" : "Allow all except";
                EditorVisibility = Visibility.Visible;
            }

            OnPropertyChanged(nameof(Rules));
            OnPropertyChanged(nameof(RulesetPolicy));
            OnPropertyChanged(nameof(EditorVisibility));
        }

        public void OpenApplicationSelector(object? parameter)
        {
            ApplicationSelectorView selector = new ApplicationSelectorView();
            selector.ShowDialog();

            if (!selector.Confirmed)
            {
                return;
            }

            foreach (ApplicationInfo application in selector.Selected)
            {
                SelectedPreset!.ApplicationRules.Add(application);
            }
            Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset!.ApplicationRules);

            OnPropertyChanged(nameof(Rules));
        }
    }
}
