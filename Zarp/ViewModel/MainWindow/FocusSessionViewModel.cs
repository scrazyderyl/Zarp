using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel.MainWindow
{
    internal class FocusSessionViewModel : ObservableObject
    {
        public ObservableCollection<FocusSessionPreset> FocusSessionPresets { get; set; }
        private int _selectedFocusSessionIndex;
        public int SelectedFocusSessionIndex
        {
            get { return _selectedFocusSessionIndex; }
            set
            {
                _selectedFocusSessionIndex = value;
                OnFocusSessionSelectionChanged();
            }
        }
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }


        public ObservableCollection<Event> Events { get; set; }
        public int _selectedEventIndex;
        public int SelectedEventIndex
        {
            get { return _selectedEventIndex; }
            set
            {
                _selectedEventIndex = value;
                OnEventSelectionChanged();
            }
        }

        public RelayCommand AddEventCommand { get; set; }
        public RelayCommand RemoveEventCommand {  get; set; }
        public RelayCommand MoveUpCommand { get; set; }
        public RelayCommand MoveDownCommand { get; set; }
        public bool IsAddEventButtonEnabled { get; set; }
        public bool IsRemoveEventButtonEnabled { get; set; }
        public bool IsMoveUpButtonEnabled { get; set; }
        public bool IsMoveDownButtonEnabled { get; set; }

        public RelayCommand ChangeEventTypeCommand { get; set; }
        public ObservableCollection<RulePreset> RulePresets { get; set; }
        public RulePreset? SelectedRulePreset { get; set; }
        public Visibility EventEditorVisibility { get; set; }
        public Visibility ActivityParametersVisibility { get; set; }
        public Visibility BreakParametersVisibility { get; set; }

        public FocusSessionViewModel()
        {
            FocusSessionPresets = new ObservableCollection<FocusSessionPreset>(Core.Zarp.FocusSessionPresetManager.GetPresets());
            SelectedFocusSessionIndex = -1;
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);

            Events = new ObservableCollection<Event>();
            SelectedEventIndex = -1;
            IsAddEventButtonEnabled = false;
            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;
            AddEventCommand = new RelayCommand(AddEvent);
            RemoveEventCommand = new RelayCommand(RemoveEvent);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);

            RulePresets = new ObservableCollection<RulePreset>(Core.Zarp.RulePresetManager.GetPresets());
            SelectedRulePreset = null;
            EventEditorVisibility = Visibility.Hidden;
            ActivityParametersVisibility = Visibility.Visible;
            BreakParametersVisibility = Visibility.Collapsed;
            ChangeEventTypeCommand = new RelayCommand(ChangeEventType);
        }


        void OnFocusSessionSelectionChanged()
        {
            if (SelectedFocusSessionIndex == -1)
            {
                IsAddEventButtonEnabled = false;
            } else
            {
                IsAddEventButtonEnabled = true;
            }

            IsRemoveEventButtonEnabled = false;
            IsMoveUpButtonEnabled = false;
            IsMoveDownButtonEnabled = false;

            OnPropertyChanged("IsAddEventButtonEnabled");
            OnPropertyChanged("IsRemoveEventButtonEnabled");
            OnPropertyChanged("IsMoveUpButtonEnabled");
            OnPropertyChanged("IsMoveDownButtonEnabled");
        }

        void OnEventSelectionChanged()
        {
            if (SelectedEventIndex == -1)
            {
                IsRemoveEventButtonEnabled = false;
                EventEditorVisibility = Visibility.Hidden;
                OnPropertyChanged("IsRemoveEventButtonEnabled");
                OnPropertyChanged("EventEditorVisibility");
                return;
            }

            IsRemoveEventButtonEnabled = true;

            if (SelectedEventIndex == 0)
            {
                IsMoveUpButtonEnabled = false;
            } else
            {
                IsMoveUpButtonEnabled = true;
            }

            if (SelectedEventIndex == Events.Count - 1)
            {
                IsMoveDownButtonEnabled = false;
            } else
            {
                IsMoveDownButtonEnabled = true;
            }

            EventEditorVisibility = Visibility.Visible;

            OnPropertyChanged("IsRemoveEventButtonEnabled");
            OnPropertyChanged("IsMoveUpButtonEnabled");
            OnPropertyChanged("IsMoveDownButtonEnabled");
            OnPropertyChanged("EventEditorVisibility");
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
            FocusSessionPresets = new ObservableCollection<FocusSessionPreset>(Core.Zarp.FocusSessionPresetManager.GetPresets());
            SelectedFocusSessionIndex = FocusSessionPresets.Count - 1;
            OnPropertyChanged("SelectedFocusSessionIndex");
            OnPropertyChanged("FocusSessionPresets");
        }

        public void RemovePreset(object? parameter)
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
            
        }

        public void RemoveEvent(object? parameter)
        {

        }

        public void MoveUp(object? parameter)
        {

        }

        public void MoveDown(object? parameter)
        {

        }

        public void ChangeEventType(object? parameter)
        {
            string type = (string?)parameter;

            if (type.Equals("Activity"))
            {
                ActivityParametersVisibility = Visibility.Visible;
                BreakParametersVisibility = Visibility.Collapsed;
            }
            else
            {
                ActivityParametersVisibility = Visibility.Collapsed;
                BreakParametersVisibility = Visibility.Visible;
            }
        }
    }
}
