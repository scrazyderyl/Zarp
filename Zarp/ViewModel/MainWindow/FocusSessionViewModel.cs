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
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }
        public RelayCommand ChangeEventTypeCommand { get; set; }

        public ObservableCollection<RulePreset> RulePresets { get; set; }

        private Visibility _activityParametersVisibility;
        private Visibility _breakParametersVisibility;

        public Visibility ActivityParametersVisibility
        {
            get { return _activityParametersVisibility; }
            set
            {
                _activityParametersVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility BreakParametersVisibility
        {
            get { return _breakParametersVisibility; }
            set
            {
                _breakParametersVisibility = value;
                OnPropertyChanged();
            }
        }

        public FocusSessionViewModel()
        {
            RulePresets = new ObservableCollection<RulePreset>();
            ActivityParametersVisibility = Visibility.Visible;
            BreakParametersVisibility = Visibility.Collapsed;

            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
            ChangeEventTypeCommand = new RelayCommand(ChangeEventType);
        }

        public void CreatePreset(object? parameter)
        {
            new TextInputView().ShowDialog();
        }

        public void RemovePreset(object? parameter)
        {

        }

        public void DuplicatePreset(object? parameter)
        {
            new TextInputView().ShowDialog();
        }

        public void ImportPreset(object? parameter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();
        }

        public void ExportPreset(object? parameter)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = ".json";
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.ShowDialog();
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
