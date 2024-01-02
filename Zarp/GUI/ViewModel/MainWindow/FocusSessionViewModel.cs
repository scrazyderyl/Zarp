using System;
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
        public static PresetCollection PresetCollection => Service.FocusSessionPresets;
        public static Func<Preset?> CreateFunction => Create;

        private FocusSessionPreset? _SelectedFocusSessionPreset;
        public FocusSessionPreset? SelectedFocusSessionPreset
        {
            get => _SelectedFocusSessionPreset;
            set
            {
                _SelectedFocusSessionPreset = value;
                OnFocusSessionSelectionChanged();
            }
        }

        public Visibility MainEditorVisibility { get; set; }

        public ObservableCollection<Event> Events { get; set; }
        public string? LoopCount { get; set; }
        public int _SelectedEventIndex;
        public int SelectedEventIndex
        {
            get { return _SelectedEventIndex; }
            set
            {
                _SelectedEventIndex = value;
                OnEventSelectionChanged();
            }
        }

        public RelayCommand AddEventCommand { get; set; }
        public RelayCommand RemoveEventCommand { get; set; }
        public RelayCommand MoveUpCommand { get; set; }
        public RelayCommand MoveDownCommand { get; set; }
        public bool IsAddEventButtonEnabled { get; set; }
        public bool IsRemoveEventButtonEnabled { get; set; }
        public bool IsMoveUpButtonEnabled { get; set; }
        public bool IsMoveDownButtonEnabled { get; set; }

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
                Events[SelectedEventIndex].Name = value;
                int index = SelectedEventIndex;
                Events = new ObservableCollection<Event>(_SelectedFocusSessionPreset.Events);
                _SelectedEventIndex = index;
                OnPropertyChanged(nameof(Events));
                OnPropertyChanged(nameof(SelectedEventIndex));
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
                        Events[SelectedEventIndex].Duration = duration;
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
                Events[SelectedEventIndex].DurationUnit = _EventDurationUnitsIndex == 0 ? TimeUnit.Minutes : TimeUnit.Hours;
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
                    Events[_SelectedEventIndex].Rules = null;
                }
                else
                {
                    Events[_SelectedEventIndex].Rules = (RulePreset)Service.RulePresets.Get((string)RulePresets[_SelectedRulePresetIndex]);
                }
            }
        }
        public Visibility EventEditorVisibility { get; set; }
        public Visibility ActivityParametersVisibility { get; set; }

        public FocusSessionViewModel()
        {
            MainEditorVisibility = Visibility.Hidden;

            Events = new ObservableCollection<Event>();
            _SelectedEventIndex = -1;
            IsAddEventButtonEnabled = false;
            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;
            AddEventCommand = new RelayCommand(AddEvent);
            RemoveEventCommand = new RelayCommand(RemoveEvent);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);

            RulePresets = new ObservableCollection<string>(Service.RulePresets);
            _SelectedRulePresetIndex = -1;
            EventEditorVisibility = Visibility.Hidden;
            ActivityParametersVisibility = Visibility.Collapsed;
        }

        public static Preset? Create()
        {
            Service.DialogReturnValue = null;
            new CreateFocusSessionView().ShowDialog();
            return (Preset?)Service.DialogReturnValue;
        }

        void OnFocusSessionSelectionChanged()
        {
            if (_SelectedFocusSessionPreset == null)
            {
                IsAddEventButtonEnabled = false;
                MainEditorVisibility = Visibility.Hidden;
            }
            else
            {
                IsAddEventButtonEnabled = true;
                FocusSessionPreset SelectedFocusSession = _SelectedFocusSessionPreset;
                Events = new ObservableCollection<Event>(SelectedFocusSession.Events);
                LoopCount = SelectedFocusSession.LoopCount.ToString();
                MainEditorVisibility = Visibility.Visible;
                OnPropertyChanged(nameof(Events));
                OnPropertyChanged(nameof(LoopCount));
            }

            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;

            OnPropertyChanged(nameof(IsAddEventButtonEnabled));
            OnPropertyChanged(nameof(IsRemoveEventButtonEnabled));
            OnPropertyChanged(nameof(IsMoveUpButtonEnabled));
            OnPropertyChanged(nameof(IsMoveDownButtonEnabled));
            OnPropertyChanged(nameof(MainEditorVisibility));
        }

        void OnEventSelectionChanged()
        {
            if (SelectedEventIndex == -1)
            {
                IsRemoveEventButtonEnabled = false;
                IsMoveUpButtonEnabled = false;
                IsMoveDownButtonEnabled = false;
                EventEditorVisibility = Visibility.Hidden;
                OnPropertyChanged(nameof(IsRemoveEventButtonEnabled));
                OnPropertyChanged(nameof(EventEditorVisibility));
                return;
            }

            IsRemoveEventButtonEnabled = true;

            if (SelectedEventIndex == 0)
            {
                IsMoveUpButtonEnabled = false;
            }
            else
            {
                IsMoveUpButtonEnabled = true;
            }

            if (SelectedEventIndex == Events.Count - 1)
            {
                IsMoveDownButtonEnabled = false;
            }
            else
            {
                IsMoveDownButtonEnabled = true;
            }

            OnPropertyChanged(nameof(IsRemoveEventButtonEnabled));
            OnPropertyChanged(nameof(IsMoveUpButtonEnabled));
            OnPropertyChanged(nameof(IsMoveDownButtonEnabled));

            Event SelectedEvent = Events[SelectedEventIndex];
            _EventName = SelectedEvent.Name;
            EventDuration = SelectedEvent.Duration.ToString();
            EventDurationUnitsIndex = SelectedEvent.DurationUnit == TimeUnit.Minutes ? 0 : 1;
            OnPropertyChanged(nameof(EventName));
            OnPropertyChanged(nameof(EventDuration));
            OnPropertyChanged(nameof(EventDurationUnitsIndex));

            switch (SelectedEvent.Type)
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

            SelectedRulePresetIndex = SelectedEvent.Rules == null ? -1 : RulePresets.IndexOf(SelectedEvent.Rules.Name);
            EventEditorVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(SelectedRulePresetIndex));
            OnPropertyChanged(nameof(EventEditorVisibility));
        }

        void OnEventTypeChanged()
        {
            if (IsEventActivity)
            {
                ActivityParametersVisibility = Visibility.Visible;
                Events[SelectedEventIndex].Type = EventType.Regular;
            }
            else
            {
                ActivityParametersVisibility = Visibility.Collapsed;
                Events[SelectedEventIndex].Type = EventType.OfflineBreak;
            }

            OnPropertyChanged(nameof(ActivityParametersVisibility));
        }

        public void AddEvent(object? parameter)
        {
            Event newEvent = new Event(string.Empty, 30, TimeUnit.Minutes, EventType.Regular, null);

            if (SelectedEventIndex == -1)
            {
                _SelectedFocusSessionPreset!.NewEvent(newEvent);
                _SelectedEventIndex = Events.Count;
                Events.Add(newEvent);
            }
            else
            {
                int newIndex = SelectedEventIndex + 1;
                _SelectedFocusSessionPreset!.AddEventAtIndex(SelectedEventIndex, newEvent);
                Events.Insert(SelectedEventIndex + 1, newEvent);
                _SelectedEventIndex = newIndex;
            }

            OnEventSelectionChanged();
            OnPropertyChanged(nameof(SelectedEventIndex));
            OnPropertyChanged(nameof(Events));
        }

        public void RemoveEvent(object? parameter)
        {
            int newIndex = SelectedEventIndex - 1;
            _SelectedFocusSessionPreset!.RemoveEvent(SelectedEventIndex);
            Events.RemoveAt(SelectedEventIndex);
            SelectedEventIndex = newIndex == -1 && Events.Count > 0 ? 0 : newIndex;
            OnPropertyChanged(nameof(SelectedEventIndex));
        }

        public void MoveUp(object? parameter)
        {
            _SelectedFocusSessionPreset!.SwapEvents(SelectedEventIndex, SelectedEventIndex - 1);

            int newIndex = SelectedEventIndex;
            Event temp = Events[SelectedEventIndex];
            Events[SelectedEventIndex] = Events[SelectedEventIndex - 1];
            Events[newIndex - 1] = temp;

            SelectedEventIndex = newIndex - 1;
            OnPropertyChanged(nameof(SelectedEventIndex));
        }

        public void MoveDown(object? parameter)
        {
            _SelectedFocusSessionPreset!.SwapEvents(SelectedEventIndex, SelectedEventIndex + 1);

            int oldIndex = SelectedEventIndex;
            Event temp = Events[SelectedEventIndex];
            Events[SelectedEventIndex] = Events[SelectedEventIndex + 1];
            Events[oldIndex + 1] = temp;

            SelectedEventIndex = oldIndex + 1;
            OnPropertyChanged(nameof(SelectedEventIndex));
        }

        public void EventTypeChanged()
        {
            if (IsEventActivity)
            {
                ActivityParametersVisibility = Visibility.Visible;
            }
            else
            {
                ActivityParametersVisibility = Visibility.Collapsed;
            }
        }
    }
}
