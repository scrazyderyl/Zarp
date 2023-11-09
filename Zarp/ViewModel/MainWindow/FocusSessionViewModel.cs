using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel.MainWindow
{
    internal class FocusSessionViewModel : ObservableObject
    {
        public ObservableCollection<FocusSessionPreset> FocusSessionPresets { get; set; }
        public FocusSessionPreset SelectedFocusSession { get; set; }
        private int _SelectedFocusSessionIndex;
        public int SelectedFocusSessionIndex
        {
            get { return _SelectedFocusSessionIndex; }
            set
            {
                _SelectedFocusSessionIndex = value;
                OnFocusSessionSelectionChanged();
            }
        }
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand RenamePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }


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

        public ObservableCollection<RulePreset> RulePresets { get; set; }
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
            get { return _EventName; }
            set
            {
                _EventName = value;
                Events[SelectedEventIndex].Name = value;
                int index = SelectedEventIndex;
                Events = new ObservableCollection<Event>(FocusSessionPresets[SelectedFocusSessionIndex].GetEvents());
                _SelectedEventIndex = index;
                OnPropertyChanged("Events");
                OnPropertyChanged("SelectedEventIndex");
                OnEventSelectionChanged();
            }
        }
        private string? _EventDuration;
        public string? EventDuration
        {
            get { return _EventDuration; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _EventDuration = value;
                    return;
                }

                try
                {
                    int duration = int.Parse(value);

                    if (duration > 0)
                    {
                        Events[SelectedEventIndex].Duration = duration;
                        _EventDuration = value;
                    } else
                    {
                        EventDuration = _EventDuration;
                        OnPropertyChanged("EventDuration");
                    }
                }
                catch
                {
                    EventDuration = _EventDuration;
                    OnPropertyChanged("EventDuration");
                }
            }
        }
        private int _EventDurationUnitsIndex;
        public int EventDurationUnitsIndex
        {
            get { return _EventDurationUnitsIndex; }
            set
            {
                _EventDurationUnitsIndex = value;
                Events[SelectedEventIndex].Unit = _EventDurationUnitsIndex == 0 ? DurationUnit.Minutes : DurationUnit.Hours;
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
                } else
                {
                    Events[_SelectedEventIndex].Rules = RulePresets[_SelectedRulePresetIndex];
                }
            }
        }
        public Visibility EventEditorVisibility { get; set; }
        public Visibility ActivityParametersVisibility { get; set; }

        public FocusSessionViewModel()
        {
            FocusSessionPresets = new ObservableCollection<FocusSessionPreset>(Core.Zarp.FocusSessionPresetManager.GetPresets());
            _SelectedFocusSessionIndex = -1;
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);

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

            RulePresets = new ObservableCollection<RulePreset>(Core.Zarp.RulePresetManager.GetPresets());
            _SelectedRulePresetIndex = -1;
            EventEditorVisibility = Visibility.Hidden;
            ActivityParametersVisibility = Visibility.Collapsed;
        }

        void OnFocusSessionSelectionChanged()
        {
            if (SelectedFocusSessionIndex == -1)
            {
                IsAddEventButtonEnabled = false;
            }
            else
            {
                IsAddEventButtonEnabled = true;
                FocusSessionPreset SelectedFocusSession = FocusSessionPresets[SelectedFocusSessionIndex];
                Events = new ObservableCollection<Event>(SelectedFocusSession.GetEvents());
                LoopCount = SelectedFocusSession.LoopCount.ToString();
            }

            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;

            OnPropertyChanged("IsAddEventButtonEnabled");
            OnPropertyChanged("IsRemoveEventButtonEnabled");
            OnPropertyChanged("IsMoveUpButtonEnabled");
            OnPropertyChanged("IsMoveDownButtonEnabled");
            OnPropertyChanged("LoopCount");
            OnPropertyChanged("Events");
        }

        void OnEventSelectionChanged()
        {
            if (SelectedEventIndex == -1)
            {
                IsRemoveEventButtonEnabled = false;
                IsMoveUpButtonEnabled = false;
                IsMoveDownButtonEnabled = false;
                EventEditorVisibility = Visibility.Hidden;
                OnPropertyChanged("IsRemoveEventButtonEnabled");
                OnPropertyChanged("EventEditorVisibility");
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

            OnPropertyChanged("IsRemoveEventButtonEnabled");
            OnPropertyChanged("IsMoveUpButtonEnabled");
            OnPropertyChanged("IsMoveDownButtonEnabled");

            Event SelectedEvent = Events[SelectedEventIndex];
            _EventName = SelectedEvent.Name;
            EventDuration = SelectedEvent.Duration.ToString();
            EventDurationUnitsIndex = SelectedEvent.Unit == DurationUnit.Minutes ? 0 : 1;

            switch (SelectedEvent.Type)
            {
                case EventType.Regular:
                    IsEventActivity = true;
                    ActivityParametersVisibility = Visibility.Visible;
                    break;
                case EventType.OfflineBreak:
                    IsEventOfflineBreak = true;
                    ActivityParametersVisibility = Visibility.Collapsed;
                    break;
            }

            SelectedRulePresetIndex = RulePresets.IndexOf(SelectedEvent.Rules);
            EventEditorVisibility = Visibility.Visible;
            OnPropertyChanged("EventEditorVisibility");
            OnPropertyChanged("IsEventActivity");
            OnPropertyChanged("IsEventOfflineBreak");
            OnPropertyChanged("ActivityParametersVisibility");
            OnPropertyChanged("EventName");
            OnPropertyChanged("EventDuration");
            OnPropertyChanged("EventDurationUnitsIndex");
            OnPropertyChanged("SelectedRulePresetIndex");
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

            OnPropertyChanged("ActivityParametersVisibility");
        }

        public void CreatePreset(object? parameter)
        {
            Core.Zarp.DialogReturnValue = null;
            new CreateFocusSessionView().ShowDialog();
            FocusSessionPreset? newPreset = (FocusSessionPreset)Core.Zarp.DialogReturnValue;

            if (newPreset == null)
            {
                return;
            }

            Core.Zarp.FocusSessionPresetManager.Add(newPreset);
            FocusSessionPresets.Add(newPreset);
            SelectedFocusSessionIndex = FocusSessionPresets.Count - 1;
            OnPropertyChanged("SelectedFocusSessionIndex");
            OnPropertyChanged("FocusSessionPresets");
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

        public void AddEvent(object? parameter)
        {
            Event newEvent = new Event();

            if (SelectedEventIndex == -1)
            {
                SelectedFocusSession.NewEvent(newEvent);
                _SelectedEventIndex = Events.Count;
                Events.Add(newEvent);
            }
            else
            {
                SelectedFocusSession.AddEventAtIndex(SelectedEventIndex, newEvent);
                Events.Insert(SelectedEventIndex, newEvent);
            }

            OnEventSelectionChanged();
            OnPropertyChanged("SelectedEventIndex");
            OnPropertyChanged("Events");
        }

        public void RemoveEvent(object? parameter)
        {
            FocusSessionPresets[SelectedFocusSessionIndex].RemoveEvent(SelectedFocusSessionIndex);
            Events.RemoveAt(SelectedFocusSessionIndex);
        }

        public void MoveUp(object? parameter)
        {
            FocusSessionPresets[SelectedFocusSessionIndex].SwapEvents(SelectedEventIndex, SelectedEventIndex - 1);

            int oldIndex = SelectedEventIndex;
            Event temp = Events[SelectedEventIndex];
            Events[SelectedEventIndex] = Events[SelectedEventIndex - 1];
            Events[oldIndex - 1] = temp;

            SelectedEventIndex = oldIndex - 1;
            OnPropertyChanged("SelectedEventIndex");
        }

        public void MoveDown(object? parameter)
        {
            FocusSessionPresets[SelectedFocusSessionIndex].SwapEvents(SelectedEventIndex, SelectedEventIndex + 1);

            int oldIndex = SelectedEventIndex;
            Event temp = Events[SelectedEventIndex];
            Events[SelectedEventIndex] = Events[SelectedEventIndex + 1];
            Events[oldIndex + 1] = temp;

            SelectedEventIndex = oldIndex + 1;
            OnPropertyChanged("SelectedEventIndex");
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
