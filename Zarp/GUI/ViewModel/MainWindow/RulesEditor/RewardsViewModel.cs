using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RewardsViewModel : ObservableObject
    {
        public ObservableCollection<RewardPreset> RewardPresets { get; set; }
        private int _SelectedRewardPresetIndex { get; set; }
        public int SelectedRewardPresetIndex
        {
            get { return _SelectedRewardPresetIndex; }
            set
            {
                _SelectedRewardPresetIndex = value;
                OnRewardPresetSelectionChanged();
            }
        }

        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand RenamePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }

        public Visibility EditorVisibility { get; set; }
        private bool _RewardEnabled;
        public bool RewardEnabled
        {
            get { return _RewardEnabled; }
            set
            {
                _RewardEnabled = value;

                if (value)
                {
                    Core.Service.Zarp.Blocker.EnableReward(RewardPresets[_SelectedRewardPresetIndex]);
                }
                else
                {
                    Core.Service.Zarp.Blocker.DisableReward(RewardPresets[_SelectedRewardPresetIndex].Name);
                }
            }
        }
        private bool _FocusSessionOptionSelected;
        public bool FocusSessionOptionSelected
        {
            get { return _FocusSessionOptionSelected; }
            set
            {
                _FocusSessionOptionSelected = value;

                if (value)
                {
                    FocusSessionSelectorVisibility = Visibility.Visible;
                    RewardPresets[_SelectedRewardPresetIndex].RequirementType = RewardRequirement.FocusSessionCompletion;
                }
                else
                {
                    FocusSessionSelectorVisibility = Visibility.Hidden;
                }

                OnPropertyChanged(nameof(FocusSessionSelectorVisibility));
            }
        }
        public Visibility FocusSessionSelectorVisibility { get; set; }
        public ObservableCollection<FocusSessionPreset> FocusSessionPresets { get; set; }
        private int _SelectedFocusSessionIndex;
        public int SelectedFocusSessionIndex
        {
            get { return _SelectedFocusSessionIndex; }
            set
            {
                _SelectedFocusSessionIndex = value;

                if (value == -1)
                {
                    RewardPresets[_SelectedRewardPresetIndex].CompletionRequirement = null;
                }
                else
                {
                    RewardPresets[_SelectedRewardPresetIndex].CompletionRequirement = FocusSessionPresets[_SelectedFocusSessionIndex];
                }
            }
        }
        private bool _ActiveTimeOptionSelected;
        public bool ActiveTimeOptionSelected
        {
            get { return _ActiveTimeOptionSelected; }
            set
            {
                _ActiveTimeOptionSelected = value;

                if (value)
                {
                    ActiveTimeInputVisibility = Visibility.Visible;
                    RewardPresets[_SelectedRewardPresetIndex].RequirementType = RewardRequirement.ActiveTime;
                }
                else
                {
                    ActiveTimeInputVisibility = Visibility.Hidden;
                }

                OnPropertyChanged(nameof(ActiveTimeInputVisibility));
            }
        }
        public Visibility ActiveTimeInputVisibility { get; set; }
        private string? _ActiveTime;
        public string? ActiveTime
        {
            get { return _ActiveTime; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _ActiveTime = value;
                    return;
                }

                try
                {
                    value = value.TrimStart('0');
                    int activeTime = int.Parse(value);

                    if (activeTime > 0)
                    {
                        _ActiveTime = value;
                        RewardPresets[_SelectedRewardPresetIndex].ActiveTimeRequirement = activeTime;
                    }
                }
                catch { }
            }
        }
        private int _ActiveTimeUnitsIndex;
        public int ActiveTimeUnitsIndex
        {
            get { return _ActiveTimeUnitsIndex; }
            set
            {
                _ActiveTimeUnitsIndex = value;

                if (_SelectedRewardPresetIndex == -1)
                {
                    return;
                }

                RewardPresets[_SelectedRewardPresetIndex].ActiveTimeUnit = _ActiveTimeUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
            }
        }

        private string? _TimeEarned;
        public string? TimeEarned
        {
            get { return _TimeEarned; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _TimeEarned = value;
                    return;
                }

                try
                {
                    value = value.TrimStart('0');
                    int timeEarned = int.Parse(value);

                    if (timeEarned > 0)
                    {
                        _TimeEarned = value;
                        RewardPresets[_SelectedRewardPresetIndex].EarnedTime = timeEarned;
                    }
                }
                catch { }
            }
        }
        private int _TimeEarnedUnitsIndex;
        public int TimeEarnedUnitsIndex
        {
            get { return _TimeEarnedUnitsIndex; }
            set
            {
                _TimeEarnedUnitsIndex = value;

                if (_SelectedRewardPresetIndex == -1)
                {
                    return;
                }

                RewardPresets[_SelectedRewardPresetIndex].EarnedTimeUnit = _TimeEarnedUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
            }
        }

        public ObservableCollection<ApplicationInfo> Applications { get; set; }
        private int _SelectedApplicationIndex;
        public int SelectedApplicationIndex
        {
            get { return _SelectedApplicationIndex; }
            set
            {
                _SelectedApplicationIndex = value;

                if (value == -1)
                {
                    return;
                }

                RewardPresets[_SelectedRewardPresetIndex].ApplicationRules.RemoveRule
                (Applications[_SelectedApplicationIndex].Id);
                Applications.RemoveAt(value);
                _SelectedApplicationIndex = -1;
                OnPropertyChanged(nameof(SelectedApplicationIndex));
            }
        }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public RewardsViewModel()
        {
            RewardPresets = new ObservableCollection<RewardPreset>(Core.Service.Zarp.RewardPresetManager.GetPresets());
            _SelectedRewardPresetIndex = -1;
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);

            EditorVisibility = Visibility.Hidden;
            FocusSessionPresets = new ObservableCollection<FocusSessionPreset>(Core.Service.Zarp.FocusSessionPresetManager.GetPresets());
            FocusSessionSelectorVisibility = Visibility.Hidden;
            _SelectedFocusSessionIndex = -1;
            ActiveTimeInputVisibility = Visibility.Hidden;

            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);
            _SelectedApplicationIndex = -1;
        }

        private void OnRewardPresetSelectionChanged()
        {
            if (_SelectedRewardPresetIndex == -1)
            {
                EditorVisibility = Visibility.Hidden;
                OnPropertyChanged(nameof(EditorVisibility));
                return;
            }

            RewardPreset SelectedRewardPreset = RewardPresets[_SelectedRewardPresetIndex];
            RewardEnabled = Core.Service.Zarp.Blocker.IsRewardEnabled(SelectedRewardPreset.Name);
            OnPropertyChanged(nameof(RewardEnabled));

            switch (SelectedRewardPreset.RequirementType)
            {
                case RewardRequirement.FocusSessionCompletion:
                    FocusSessionOptionSelected = true;
                    _SelectedFocusSessionIndex = FocusSessionPresets.IndexOf(SelectedRewardPreset.CompletionRequirement);
                    _ActiveTime = null;
                    _ActiveTimeUnitsIndex = 0;
                    OnPropertyChanged(nameof(FocusSessionOptionSelected));
                    OnPropertyChanged(nameof(SelectedFocusSessionIndex));
                    OnPropertyChanged(nameof(ActiveTime));
                    OnPropertyChanged(nameof(ActiveTimeUnitsIndex));
                    break;
                case RewardRequirement.ActiveTime:
                    ActiveTimeOptionSelected = true;
                    _SelectedFocusSessionIndex = -1;
                    _ActiveTime = SelectedRewardPreset.ActiveTimeRequirement.ToString();
                    _ActiveTimeUnitsIndex = SelectedRewardPreset.ActiveTimeUnit == TimeUnit.Minutes ? 0 : 1;
                    OnPropertyChanged(nameof(SelectedFocusSessionIndex));
                    OnPropertyChanged(nameof(ActiveTimeOptionSelected));
                    OnPropertyChanged(nameof(ActiveTime));
                    OnPropertyChanged(nameof(ActiveTimeUnitsIndex));
                    break;
            }

            _TimeEarned = SelectedRewardPreset.EarnedTime.ToString();
            _TimeEarnedUnitsIndex = SelectedRewardPreset.EarnedTimeUnit == TimeUnit.Minutes ? 0 : 1;
            OnPropertyChanged(nameof(TimeEarned));
            OnPropertyChanged(nameof(TimeEarnedUnitsIndex));

            Applications = new ObservableCollection<ApplicationInfo>(SelectedRewardPreset.ApplicationRules.GetRules());
            OnPropertyChanged(nameof(Applications));

            EditorVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(EditorVisibility));
        }

        public void CreatePreset(object? parameter)
        {
            Core.Service.Zarp.DialogReturnValue = null;
            new CreateRewardView().ShowDialog();
            RewardPreset? newPreset = (RewardPreset)Core.Service.Zarp.DialogReturnValue;

            if (newPreset == null)
            {
                return;
            }

            RewardPresets = new ObservableCollection<RewardPreset>(Core.Service.Zarp.RewardPresetManager.GetPresets());
            SelectedRewardPresetIndex = RewardPresets.Count - 1;
            OnPropertyChanged(nameof(RewardPresets));
            OnPropertyChanged(nameof(SelectedRewardPresetIndex));
        }

        public void RemovePreset(object? parameter)
        {

        }

        public void RenamePreset(object? parameter)
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
            Core.Service.Zarp.DialogReturnValue = null;
            new ApplicationSelectorView().ShowDialog();
            List<ApplicationInfo>? newRules = (List<ApplicationInfo>?)Core.Service.Zarp.DialogReturnValue;

            if (newRules == null)
            {
                return;
            }

            RulePreset SelectedPreset = RewardPresets[_SelectedRewardPresetIndex];
            SelectedPreset.ApplicationRules.AddRules(newRules);
            Applications = new ObservableCollection<ApplicationInfo>(SelectedPreset.ApplicationRules.GetRules());
            OnPropertyChanged(nameof(Applications));
            _SelectedApplicationIndex = -1;
            OnPropertyChanged(nameof(SelectedApplicationIndex));
        }
    }
}
