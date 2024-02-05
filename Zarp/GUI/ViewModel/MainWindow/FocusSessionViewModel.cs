using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class FocusSessionViewModel : ObservableObject
    {
        public static IPresetCollection PresetCollection => Service.FocusSessionPresets;
        public static Func<IPreset?> CreateFunction => Create;

        private FocusSessionPreset? _SelectedFocusSessionPreset;
        public FocusSessionPreset? SelectedFocusSessionPreset
        {
            get => _SelectedFocusSessionPreset;
            set
            {
                _SelectedFocusSessionPreset = value;
                OnFocusSessionSelectionChanged();
                OnEventSelectionChanged();
            }
        }

        public Visibility MainEditorVisibility { get; set; }

        public IList? EventList { get; set; }
        private Event? _SelectedEvent;
        public Event? SelectedEvent
        {
            get => _SelectedEvent;
            set
            {
                _SelectedEvent = value;
                OnEventSelectionChanged();
            }
        }

        public ObservableCollection<string> RulePresets { get; set; }
        private bool _IsEventActivity;
        public bool IsEventActivity
        {
            get { return _IsEventActivity; }
            set
            {
                _IsEventActivity = value;
                OnEventTypeChanged();
            }
        }
        private bool _IsEventOfflineBreak;
        public bool IsEventOfflineBreak
        {
            get { return _IsEventOfflineBreak; }
            set
            {
                _IsEventOfflineBreak = value;
                OnEventTypeChanged();
            }
        }

        private string? _EventName;
        public string? EventName
        {
            get => _EventName;
            set
            {
                _EventName = value;
                _SelectedEvent!.Name = value!;
                OnEventSelectionChanged();
            }
        }
        private string? _EventDuration;
        public string? EventDuration
        {
            get { return _EventDuration; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _EventDuration = value;
                    return;
                }

                try
                {
                    value = value.TrimStart('0');
                    int duration = int.Parse(value);

                    if (duration > 0)
                    {
                        _EventDuration = value;
                        _SelectedEvent!.Duration = duration;
                    }
                }
                catch { }
            }
        }
        private int _EventDurationUnitsIndex;
        public int EventDurationUnitsIndex
        {
            get { return _EventDurationUnitsIndex; }
            set
            {
                _EventDurationUnitsIndex = value;
                _SelectedEvent!.DurationUnit = _EventDurationUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
            }
        }
        private int _SelectedRulePresetIndex;
        public int SelectedRulePresetIndex
        {
            get { return _SelectedRulePresetIndex; }
            set
            {
                _SelectedRulePresetIndex = value;

                if (value == -1)
                {
                    _SelectedEvent!.Rules = null;
                }
                else
                {
                    _SelectedEvent!.Rules = Service.RulePresets[RulePresets[_SelectedRulePresetIndex]];
                }
            }
        }
        public Visibility EventEditorVisibility { get; set; }
        public Visibility ActivityParametersVisibility { get; set; }

        public FocusSessionViewModel()
        {
            MainEditorVisibility = Visibility.Hidden;

            RulePresets = new ObservableCollection<string>(Service.RulePresets);
            _SelectedRulePresetIndex = -1;
            EventEditorVisibility = Visibility.Hidden;
            ActivityParametersVisibility = Visibility.Collapsed;
        }

        public static IPreset? Create()
        {
            Service.DialogReturnValue = null;
            new CreateFocusSessionView().ShowDialog();
            return (IPreset?)Service.DialogReturnValue;
        }

        void OnFocusSessionSelectionChanged()
        {
            if (_SelectedFocusSessionPreset == null)
            {
                EventList = null;
                MainEditorVisibility = Visibility.Hidden;
            }
            else
            {
                EventList = SelectedFocusSessionPreset!.Events;
                MainEditorVisibility = Visibility.Visible;
            }

            OnPropertyChanged(nameof(EventList));
            OnPropertyChanged(nameof(MainEditorVisibility));
        }

        void OnEventSelectionChanged()
        {
            if (_SelectedEvent == null)
            {
                EventEditorVisibility = Visibility.Hidden;
                OnPropertyChanged(nameof(EventEditorVisibility));
                return;
            }

            _EventName = _SelectedEvent.Name;
            EventDuration = _SelectedEvent.Duration.ToString();
            EventDurationUnitsIndex = _SelectedEvent.DurationUnit == TimeUnit.Minutes ? 0 : 1;
            OnPropertyChanged(nameof(EventName));
            OnPropertyChanged(nameof(EventDuration));
            OnPropertyChanged(nameof(EventDurationUnitsIndex));

            switch (_SelectedEvent.Type)
            {
                case EventType.Regular:
                    IsEventActivity = true;
                    ActivityParametersVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(IsEventActivity));
                    break;
                case EventType.OfflineBreak:
                    IsEventOfflineBreak = true;
                    ActivityParametersVisibility = Visibility.Collapsed;
                    OnPropertyChanged(nameof(IsEventOfflineBreak));
                    break;
            }

            OnPropertyChanged(nameof(ActivityParametersVisibility));

            SelectedRulePresetIndex = _SelectedEvent.Rules == null ? -1 : RulePresets.IndexOf(_SelectedEvent.Rules.Name);
            EventEditorVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(SelectedRulePresetIndex));
            OnPropertyChanged(nameof(EventEditorVisibility));
        }

        void OnEventTypeChanged()
        {
            if (IsEventActivity)
            {
                ActivityParametersVisibility = Visibility.Visible;
                _SelectedEvent!.Type = EventType.Regular;
            }
            else
            {
                ActivityParametersVisibility = Visibility.Collapsed;
                _SelectedEvent!.Type = EventType.OfflineBreak;
            }

            OnPropertyChanged(nameof(ActivityParametersVisibility));
        }
    }
}
