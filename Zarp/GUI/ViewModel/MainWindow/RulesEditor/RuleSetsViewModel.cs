using System;
using System.Collections.ObjectModel;
using System.Windows;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RuleSetsViewModel : ObservableObject
    {
        public static IPresetCollection PresetCollection => PresetManager.RuleSets;
        public static Func<Preset?> CreateFunction => Create;

        private RuleSet? _SelectedRuleSet;
        public RuleSet? SelectedRuleSet
        {
            get => _SelectedRuleSet;
            set
            {
                _SelectedRuleSet = value;
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
                SelectedRuleSet!._ApplicationRules.Remove(Rules[value]);
                Rules.RemoveAt(value);
            }
        }

        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public RuleSetsViewModel()
        {
            EditorVisibility = Visibility.Hidden;
            Rules = new ObservableCollection<ApplicationInfo>();
            _SelectedRuleIndex = -1;
            RulesetPolicy = string.Empty;

            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);
        }

        public static Preset? Create()
        {
            Session.DialogReturnValue = null;
            new CreateRuleSetView().ShowDialog();
            return (Preset?)Session.DialogReturnValue;
        }

        private void OnPresetSelectionChanged()
        {
            if (_SelectedRuleSet == null)
            {
                Rules = new ObservableCollection<ApplicationInfo>();
                RulesetPolicy = string.Empty;
                EditorVisibility = Visibility.Hidden;
            }
            else
            {
                Rules = new ObservableCollection<ApplicationInfo>(SelectedRuleSet!._ApplicationRules);
                RulesetPolicy = SelectedRuleSet._IsApplicationWhitelist ? "Block all except" : "Allow all except";
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
                SelectedRuleSet!._ApplicationRules.Add(application);
            }
            Rules = new ObservableCollection<ApplicationInfo>(SelectedRuleSet!._ApplicationRules);

            OnPropertyChanged(nameof(Rules));
        }
    }
}
