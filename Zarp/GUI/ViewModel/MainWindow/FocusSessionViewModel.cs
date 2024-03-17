using System;
using System.Collections;
using System.Collections.ObjectModel;
using Zarp.Core.App;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class FocusSessionViewModel : ObservableObject
    {
        public static IPresetCollection PresetCollection => Service.FocusSessionPresets;
        public static Func<Preset?> CreateFocusSessionFunction => CreateFocusSession;
        public static Func<object?> CreateEventFunction => CreateEvent;

        private FocusSession? _SelectedFocusSession;
        public FocusSession? SelectedFocusSession
        {
            get => _SelectedFocusSession;
            set
            {
                _SelectedFocusSession = value;
                OnFocusSessionSelectionChanged();
                OnEventSelectionChanged();
            }
        }

        public IList? EventList { get; set; }

        private FocusSessionEvent? _SelectedEvent;
        public FocusSessionEvent? SelectedEvent
        {
            get => _SelectedEvent;
            set
            {
                _SelectedEvent = value;
                OnEventSelectionChanged();
            }
        }

        public ObservableCollection<Preset> RulePresets { get; set; }

        private bool _IsEventActivity;
        public bool IsEventActivity
        {
            get => _IsEventActivity;
            set
            {
                _IsEventActivity = value;
                SelectedEvent!.Type = value ? EventType.Regular : EventType.OfflineBreak;
                OnPropertyChanged(nameof(IsEventActivity));
            }
        }

        private bool _IsEventOfflineBreak;
        public bool IsEventOfflineBreak
        {
            get => _IsEventOfflineBreak;
            set
            {
                _IsEventOfflineBreak = value;
                OnPropertyChanged(nameof(IsEventOfflineBreak));
            }
        }

        private string? _EventName;
        public string? EventName
        {
            get => _EventName;
            set
            {
                _EventName = value;
                SelectedEvent!.Name = value!;
                OnEventSelectionChanged();
            }
        }

        public int EventDuration
        {
            get => SelectedEvent == null ? 0 : SelectedEvent.Duration;
            set
            {
                if (SelectedEvent != null)
                {
                    SelectedEvent.Duration = value;
                }
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
                    _SelectedFocusSession!.SetEventRuleSet(SelectedEvent!, null);
                }
                else
                {

                    _SelectedFocusSession!.SetEventRuleSet(SelectedEvent!, (RuleSet)RulePresets[_SelectedRulePresetIndex]);
                }
            }
        }

        public FocusSessionViewModel()
        {
            RulePresets = new ObservableCollection<Preset>(Service.RulePresets);
            _SelectedRulePresetIndex = -1;
        }

        public static Preset? CreateFocusSession()
        {
            Service.DialogReturnValue = null;
            new CreateFocusSessionView().ShowDialog();
            return (Preset?)Service.DialogReturnValue;
        }

        static int Count = 0;

        private static object? CreateEvent()
        {
            Count++;
            return new FocusSessionEvent(Count.ToString(), 30, EventType.Regular, null);
        }

        private void OnFocusSessionSelectionChanged()
        {
            if (_SelectedFocusSession == null)
            {
                EventList = null;
            }
            else
            {
                EventList = SelectedFocusSession!._Events;
            }

            OnPropertyChanged(nameof(EventList));
            OnPropertyChanged(nameof(SelectedFocusSession));
        }

        private void OnEventSelectionChanged()
        {
            if (SelectedEvent == null)
            {
                OnPropertyChanged(nameof(SelectedEvent));
                return;
            }

            _EventName = SelectedEvent.Name;
            EventDuration = SelectedEvent.Duration;
            OnPropertyChanged(nameof(EventName));
            OnPropertyChanged(nameof(EventDuration));

            if (SelectedEvent.Type == EventType.Regular)
            {
                _IsEventActivity = true;
                _IsEventOfflineBreak = false;
            }
            else
            {
                _IsEventActivity = false;
                _IsEventOfflineBreak = true;
            }

            OnPropertyChanged(nameof(IsEventActivity));
            OnPropertyChanged(nameof(IsEventOfflineBreak));

            SelectedRulePresetIndex = SelectedEvent.Rules == null ? -1 : RulePresets.IndexOf(SelectedEvent.Rules);
            OnPropertyChanged(nameof(SelectedRulePresetIndex));
            OnPropertyChanged(nameof(SelectedEvent));
        }
    }
}
