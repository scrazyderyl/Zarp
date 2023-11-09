using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zarp.Core;
using Zarp.View;

namespace Zarp.ViewModel
{
    class CreateFocusSessionViewModel : ObservableObject
    {
        public string? Name { get; set; }
        public string? Loops { get; set; }
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

            OnPropertyChanged("LoopCountFieldVisibility");
        }

        void Confirm(object? parameter)
        {
            CreateFocusSessionView window = (CreateFocusSessionView?)parameter;

            if (String.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            int loops;

            if (LoopEnabled)
            {
                if (String.IsNullOrWhiteSpace(Loops))
                {
                    return;
                }

                try
                {
                    loops = int.Parse(Loops);

                    if (loops < 1)
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }
            }
            else
            {
                loops = 1;
            }

            string name = Name.Trim();
            FocusSessionPreset newPreset = new FocusSessionPreset(name, loops);

            if (!Core.Zarp.FocusSessionPresetManager.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.Zarp.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateFocusSessionView window = (CreateFocusSessionView?)parameter;
            window.Close();
        }
    }
}
