using System;
using Zarp.Core;
using Zarp.View;
using System.Windows;

namespace Zarp.ViewModel
{
    class CreateRulePresetViewModel : ObservableObject
    {
        public string? Name { get; set; }
        public bool IsWhitelist {  get; set; }
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
            CreateRulePresetView window = (CreateRulePresetView?)parameter;

            if (String.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            string name = Name.Trim();
            RulePreset newPreset = new RulePreset(name, IsWhitelist, IsWhitelist);

            if (!Core.Zarp.RulePresetManager.Add(newPreset))
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
