using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel
{
    internal class CreateRulePresetViewModel : ObservableObject
    {
        public string? Name { get; set; }
        public bool IsWhitelist { get; set; }
        public bool IsBlacklist { get; set; }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public CreateRulePresetViewModel()
        {
            IsWhitelist = true;

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        void Confirm(object? parameter)
        {
            CreateRulePresetView window = (CreateRulePresetView)parameter!;

            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            string name = Name.Trim();
            RulePreset newPreset = new RulePreset(name, IsWhitelist);

            if (!Core.App.Service.RulePresets.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.App.Service.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateRulePresetView window = (CreateRulePresetView)parameter!;
            window.Close();
        }
    }
}
