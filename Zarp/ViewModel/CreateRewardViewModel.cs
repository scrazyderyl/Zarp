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
    internal class CreateRewardViewModel
    {
        public string? Name { get; set; }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public CreateRewardViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        void Confirm(object? parameter)
        {
            CreateRewardView window = (CreateRewardView?)parameter;

            if (String.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            string name = Name.Trim();
            RewardPreset newPreset = new RewardPreset(name);

            if (!Core.Zarp.RewardPresetManager.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.Zarp.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateRewardView window = (CreateRewardView?)parameter;
            window.Close();
        }
    }
}
