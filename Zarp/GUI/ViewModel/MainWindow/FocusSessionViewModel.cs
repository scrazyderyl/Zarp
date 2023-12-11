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
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class FocusSessionViewModel : ObservableObject
    {
        public ObservableCollection<FocusSessionPreset> FocusSessionPresets { get; set; }
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
                catch
                {

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
                    Events[_SelectedEventIndex].Rules = RulePresets[_SelectedRulePresetIndex];
                }
            }
        }
        public Visibility EventEditorVisibility { get; set; }
        public Visibility ActivityParametersVisibility { get; set; }

        public FocusSessionViewModel()
        {
            FocusSessionPresets = new ObservableCollection<FocusSessionPreset>(Core.Service.Zarp.FocusSessionPresetManager.GetPresets());
            _SelectedFocusSessionIndex = -1;
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);

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

            RulePresets = new ObservableCollection<RulePreset>(Core.Service.Zarp.RulePresetManager.GetPresets());
            _SelectedRulePresetIndex = -1;
            EventEditorVisibility = Visibility.Hidden;
            ActivityParametersVisibility = Visibility.Collapsed;
        }

        void OnFocusSessionSelectionChanged()
        {
            if (SelectedFocusSessionIndex == -1)
            {
                IsAddEventButtonEnabled = false;
                MainEditorVisibility = Visibility.Hidden;
            }
            else
            {
                IsAddEventButtonEnabled = true;
                FocusSessionPreset SelectedFocusSession = FocusSessionPresets[SelectedFocusSessionIndex];
                Events = new ObservableCollection<Event>(SelectedFocusSession.GetEvents());
                LoopCount = SelectedFocusSession.LoopCount.ToString();
                MainEditorVisibility = Visibility.Visible;
                OnPropertyChanged("Events");
                OnPropertyChanged("LoopCount");
            }

            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;

            OnPropertyChanged("IsAddEventButtonEnabled");
            OnPropertyChanged("IsRemoveEventButtonEnabled");
            OnPropertyChanged("IsMoveUpButtonEnabled");
            OnPropertyChanged("IsMoveDownButtonEnabled");
            OnPropertyChanged("MainEditorVisibility");
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
            EventDurationUnitsIndex = SelectedEvent.DurationUnit == TimeUnit.Minutes ? 0 : 1;
            OnPropertyChanged("EventName");
            OnPropertyChanged("EventDuration");
            OnPropertyChanged("EventDurationUnitsIndex");

            switch (SelectedEvent.Type)
            {
                case EventType.Regular:
                    IsEventActivity = true;
                    ActivityParametersVisibility = Visibility.Visible;
                    OnPropertyChanged("IsEventActivity");
                    break;
                case EventType.OfflineBreak:
                    IsEventOfflineBreak = true;
                    ActivityParametersVisibility = Visibility.Collapsed;
                    OnPropertyChanged("IsEventOfflineBreak");
                    break;
            }

            OnPropertyChanged("ActivityParametersVisibility");

            SelectedRulePresetIndex = RulePresets.IndexOf(SelectedEvent.Rules);
            EventEditorVisibility = Visibility.Visible;
            OnPropertyChanged("SelectedRulePresetIndex");
            OnPropertyChanged("EventEditorVisibility");
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
            Core.Service.Zarp.DialogReturnValue = null;
            new CreateFocusSessionView().ShowDialog();
            FocusSessionPreset? newPreset = (FocusSessionPreset?)Core.Service.Zarp.DialogReturnValue;

            if (newPreset == null)
            {
                return;
            }

            Core.Service.Zarp.FocusSessionPresetManager.Add(newPreset);
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
            Event newEvent = new Event(string.Empty, 30, TimeUnit.Minutes, EventType.Regular, null);

            if (SelectedEventIndex == -1)
            {
                FocusSessionPresets[SelectedFocusSessionIndex].NewEvent(newEvent);
                _SelectedEventIndex = Events.Count;
                Events.Add(newEvent);
            }
            else
            {
                int newIndex = SelectedEventIndex + 1;
                FocusSessionPresets[SelectedFocusSessionIndex].AddEventAtIndex(SelectedEventIndex, newEvent);
                Events.Insert(SelectedEventIndex + 1, newEvent);
                _SelectedEventIndex = newIndex;
            }

            OnEventSelectionChanged();
            OnPropertyChanged("SelectedEventIndex");
            OnPropertyChanged("Events");
        }

        public void RemoveEvent(object? parameter)
        {
            int newIndex = SelectedEventIndex - 1;
            FocusSessionPresets[SelectedFocusSessionIndex].RemoveEvent(SelectedEventIndex);
            Events.RemoveAt(SelectedEventIndex);
            SelectedEventIndex = newIndex == -1 && Events.Count > 0 ? 0 : newIndex;
            OnPropertyChanged("SelectedEventIndex");
        }

        public void MoveUp(object? parameter)
        {
            FocusSessionPresets[SelectedFocusSessionIndex].SwapEvents(SelectedEventIndex, SelectedEventIndex - 1);

            int newIndex = SelectedEventIndex;
            Event temp = Events[SelectedEventIndex];
            Events[SelectedEventIndex] = Events[SelectedEventIndex - 1];
            Events[newIndex - 1] = temp;

            SelectedEventIndex = newIndex - 1;
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
