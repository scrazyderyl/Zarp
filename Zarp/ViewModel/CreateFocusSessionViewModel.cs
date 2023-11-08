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
        public string? Loops {  get; set; }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public CreateFocusSessionViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        void Confirm(object? parameter)
        {
            CreateFocusSessionView window = (CreateFocusSessionView?)parameter;

            if (String.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(Loops))
            {
                return;
            }

            int loops;

            try
            {
                loops = int.Parse(Loops);
                
                if (loops < 1)
                {
                    return;
                }
            } catch
            {
                return;
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
            CreateRulePresetView window = (CreateRulePresetView?)parameter;
            window.Close();
        }
    }
}
