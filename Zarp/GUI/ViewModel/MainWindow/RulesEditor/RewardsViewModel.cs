using System;
using System.Collections.ObjectModel;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RewardsViewModel : ObservableObject
    {
        public static IPresetCollection PresetCollection => Service.Rewards;
        public static Func<Preset?> CreateFunction => Create;

        private Reward? _SelectedReward;
        public Reward? SelectedReward
        {
            get => _SelectedReward;
            set
            {
                _SelectedReward = value;
                OnRewardSelectionChanged();
            }
        }

        private bool _RewardEnabled;
        public bool RewardEnabled
        {
            get { return _RewardEnabled; }
            set
            {
                _RewardEnabled = value;

                if (value)
                {
                    Service.EnableReward(_SelectedReward!);
                }
                else
                {
                    Service.DisableReward(_SelectedReward!);
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
                    _SelectedReward!.RequirementType = RewardRequirement.FocusSessionCompletion;
                }

                OnPropertyChanged(nameof(FocusSessionOptionSelected));
            }
        }

        public ObservableCollection<Preset> FocusSessions { get; set; }
        private int _SelectedFocusSessionIndex;
        public int SelectedFocusSessionIndex
        {
            get { return _SelectedFocusSessionIndex; }
            set
            {
                _SelectedFocusSessionIndex = value;

                if (value == -1)
                {
                    _SelectedReward!.CompletionRequirement = null;
                }
                else
                {
                    _SelectedReward!.CompletionRequirement = (FocusSession)FocusSessions[_SelectedFocusSessionIndex];
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
                    _SelectedReward!.RequirementType = RewardRequirement.ActiveTime;
                }

                OnPropertyChanged(nameof(ActiveTimeOptionSelected));
            }
        }

        public int ActiveTime
        {
            get => _SelectedReward == null ? 0 : _SelectedReward.ActiveTimeRequirement;
            set
            {
                if (_SelectedReward != null)
                {
                    _SelectedReward.ActiveTimeRequirement = value;
                }
            }
        }

        public int TimeEarned
        {
            get => _SelectedReward == null ? 0 : _SelectedReward.EarnedTime;
            set
            {
                if (_SelectedReward != null)
                {
                    _SelectedReward.EarnedTime = value;
                }
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

                _SelectedReward!.Rules._ApplicationRules.Remove(Applications[_SelectedApplicationIndex]);
                Applications.RemoveAt(value);
                _SelectedApplicationIndex = -1;
                OnPropertyChanged(nameof(SelectedApplicationIndex));
            }
        }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public RewardsViewModel()
        {
            FocusSessions = new ObservableCollection<Preset>(Service.FocusSessions);
            _SelectedFocusSessionIndex = -1;

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

        private void OnRewardSelectionChanged()
        {
            if (SelectedReward == null)
            {
                OnPropertyChanged(nameof(SelectedReward));
                return;
            }

            RewardEnabled = Service.IsRewardEnabled(SelectedReward);
            OnPropertyChanged(nameof(RewardEnabled));

            switch (SelectedReward.RequirementType)
            {
                case RewardRequirement.FocusSessionCompletion:
                    FocusSessionOptionSelected = true;
                    _SelectedFocusSessionIndex = SelectedReward.CompletionRequirement == null ? -1 : FocusSessions.IndexOf(SelectedReward.CompletionRequirement);
                    ActiveTime = 0;
                    OnPropertyChanged(nameof(FocusSessionOptionSelected));
                    OnPropertyChanged(nameof(SelectedFocusSessionIndex));
                    OnPropertyChanged(nameof(ActiveTime));
                    break;
                case RewardRequirement.ActiveTime:
                    ActiveTimeOptionSelected = true;
                    _SelectedFocusSessionIndex = -1;
                    ActiveTime = SelectedReward.ActiveTimeRequirement;
                    OnPropertyChanged(nameof(SelectedFocusSessionIndex));
                    OnPropertyChanged(nameof(ActiveTimeOptionSelected));
                    OnPropertyChanged(nameof(ActiveTime));
                    break;
            }

            TimeEarned = SelectedReward.EarnedTime;
            OnPropertyChanged(nameof(TimeEarned));

            Applications = new ObservableCollection<ApplicationInfo>(SelectedReward.Rules._ApplicationRules);
            OnPropertyChanged(nameof(Applications));
            OnPropertyChanged(nameof(SelectedReward));
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
                SelectedReward!.Rules._ApplicationRules.Add(application);
            }
            Applications = new ObservableCollection<ApplicationInfo>(SelectedReward!.Rules._ApplicationRules);

            OnPropertyChanged(nameof(Applications));
        }
    }
}
