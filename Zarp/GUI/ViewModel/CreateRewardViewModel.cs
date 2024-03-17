using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel
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
            CreateRewardView window = (CreateRewardView)parameter!;

            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            string name = Name.Trim();
            Reward newPreset = new Reward(name, 30, RewardRequirement.FocusSessionCompletion, null, 60);

            if (!Core.App.Service.RewardPresets.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.App.Service.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateRewardView window = (CreateRewardView)parameter!;
            window.Close();
        }
    }
}
