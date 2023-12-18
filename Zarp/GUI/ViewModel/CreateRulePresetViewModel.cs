using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using Zarp.GUI.View;

namespace Zarp.GUI.ViewModel
{
    class CreateRulePresetViewModel : ObservableObject
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
            RulePreset newPreset = new RulePreset(name, IsWhitelist, IsWhitelist);

            if (!Core.Service.Zarp.RulePresetManager.Add(newPreset))
            {
                MessageBox.Show("A preset with the same name already exists.");
                return;
            }

            Core.Service.Zarp.DialogReturnValue = newPreset;

            window.Close();
        }

        void Cancel(object? parameter)
        {
            CreateRulePresetView window = (CreateRulePresetView)parameter!;
            window.Close();
        }
    }
}
