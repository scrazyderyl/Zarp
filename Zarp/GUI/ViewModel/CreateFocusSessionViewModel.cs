using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel
{
    class CreateFocusSessionViewModel : ObservableObject
    {
        public string? Name { get; set; }
        private bool _LoopEnabled;
        public bool LoopEnabled
        {
            get { return _LoopEnabled; }
            set
            {
                _LoopEnabled = value;
                OnLoopToggled();
            }
        }
        private string? _LoopCount;
        public string? LoopCount
        {
            get { return _LoopCount; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _LoopCount = value;
                    return;
                }

                try
                {
                    value = value.TrimStart('0');
                    int loops = int.Parse(value);

                    if (loops > 0)
                    {
                        _LoopCount = value;
                    }
                }
                catch { }
            }
        }
        public Visibility LoopCountFieldVisibility { get; set; }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public CreateFocusSessionViewModel()
        {
            LoopEnabled = false;
            LoopCountFieldVisibility = Visibility.Hidden;

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        void OnLoopToggled()
        {
            if (LoopEnabled)
            {
                LoopCountFieldVisibility = Visibility.Visible;
            }
            else
            {
                LoopCountFieldVisibility = Visibility.Hidden;
            }

            OnPropertyChanged(nameof(LoopCountFieldVisibility));
        }

        void Confirm(object? parameter)
        {
            CreateFocusSessionView window = (CreateFocusSessionView)parameter!;

            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            int loops;

            if (LoopEnabled)
            {
                if (string.IsNullOrWhiteSpace(LoopCount))
                {
                    return;
                }

                loops = int.Parse(LoopCount);
            }
            else
            {
                loops = 1;
            }

            string name = Name.Trim();
            FocusSessionPreset newPreset = new FocusSessionPreset(name, loops);

            if (!Core.App.Service.FocusSessionPresets.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.App.Service.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateFocusSessionView window = (CreateFocusSessionView)parameter!;
            window.Close();
        }
    }
}
