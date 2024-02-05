using System;
using System.Collections.ObjectModel;
using System.Windows;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RewardsViewModel : ObservableObject
    {
        public static PresetCollection PresetCollection => Service.RewardPresets;
        public static Func<Preset?> CreateFunction => Create;

        private RewardPreset? _SelectedRewardPreset;
        public RewardPreset? SelectedRewardPreset
        {
            get => _SelectedRewardPreset;
            set
            {
                _SelectedRewardPreset = value;
                OnRewardPresetSelectionChanged();
            }
        }

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
                    Service.Blocker.EnableReward(_SelectedRewardPreset!);
                }
                else
                {
                    Service.Blocker.DisableReward(_SelectedRewardPreset!.Name);
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
                    _SelectedRewardPreset!.RequirementType = RewardRequirement.FocusSessionCompletion;
                }
                else
                {
                    FocusSessionSelectorVisibility = Visibility.Hidden;
                }

                OnPropertyChanged(nameof(FocusSessionSelectorVisibility));
            }
        }
        public Visibility FocusSessionSelectorVisibility { get; set; }
        public ObservableCollection<string> FocusSessionPresets { get; set; }
        private int _SelectedFocusSessionIndex;
        public int SelectedFocusSessionIndex
        {
            get { return _SelectedFocusSessionIndex; }
            set
            {
                _SelectedFocusSessionIndex = value;

                if (value == -1)
                {
                    _SelectedRewardPreset!.CompletionRequirement = null;
                }
                else
                {
                    _SelectedRewardPreset!.CompletionRequirement = Service.FocusSessionPresets[FocusSessionPresets[_SelectedFocusSessionIndex]];
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
                    _SelectedRewardPreset!.RequirementType = RewardRequirement.ActiveTime;
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
                        _SelectedRewardPreset!.ActiveTimeRequirement = activeTime;
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

                if (_SelectedRewardPreset == null)
                {
                    return;
                }

                _SelectedRewardPreset.ActiveTimeUnit = _ActiveTimeUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
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
                        _SelectedRewardPreset!.EarnedTime = timeEarned;
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

                if (_SelectedRewardPreset == null)
                {
                    return;
                }

                _SelectedRewardPreset!.EarnedTimeUnit = _TimeEarnedUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
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

                _SelectedRewardPreset!.Rules.ApplicationRules.Remove(Applications[_SelectedApplicationIndex].Id);
                Applications.RemoveAt(value);
                _SelectedApplicationIndex = -1;
                OnPropertyChanged(nameof(SelectedApplicationIndex));
            }
        }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public RewardsViewModel()
        {
            EditorVisibility = Visibility.Hidden;
            FocusSessionPresets = new ObservableCollection<string>(Service.FocusSessionPresets);
            FocusSessionSelectorVisibility = Visibility.Hidden;
            _SelectedFocusSessionIndex = -1;
            ActiveTimeInputVisibility = Visibility.Hidden;

            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);
            Applications = new ObservableCollection<ApplicationInfo>();
            _SelectedApplicationIndex = -1;
        }

        public static Preset? Create()
        {
            Service.DialogReturnValue = null;
            new CreateRewardView().ShowDialog();
            return (Preset?)Service.DialogReturnValue;
        }

        private void OnRewardPresetSelectionChanged()
        {
            if (SelectedRewardPreset == null)
            {
                EditorVisibility = Visibility.Hidden;
                OnPropertyChanged(nameof(EditorVisibility));
                return;
            }

            RewardEnabled = Service.Blocker.IsRewardEnabled(SelectedRewardPreset.Name);
            OnPropertyChanged(nameof(RewardEnabled));

            switch (SelectedRewardPreset.RequirementType)
            {
                case RewardRequirement.FocusSessionCompletion:
                    FocusSessionOptionSelected = true;
                    _SelectedFocusSessionIndex = SelectedRewardPreset.CompletionRequirement == null ? -1 : FocusSessionPresets.IndexOf(SelectedRewardPreset.CompletionRequirement.Name);
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

            Applications = new ObservableCollection<ApplicationInfo>(SelectedRewardPreset.Rules.ApplicationRules);
            OnPropertyChanged(nameof(Applications));

            EditorVisibility = Visibility.Visible;
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
                SelectedRewardPreset!.Rules.ApplicationRules.Add(application);
            }
            Applications = new ObservableCollection<ApplicationInfo>(SelectedRewardPreset!.Rules.ApplicationRules);

            OnPropertyChanged(nameof(Applications));
        }
    }
}
