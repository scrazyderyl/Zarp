using Zarp.GUI.DataTypes;
using Zarp.GUI.ViewModel.MainWindow.RulesEditor;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class RulesViewModel : ObservableObject
    {
        public RuleSetsViewModel PresetsVM { get; set; }
        public GlobalRulesViewModel GlobalRulesVM { get; set; }
        public TimeLimitsViewModel TimeLimitsVM { get; set; }
        public RewardsViewModel RewardsVM { get; set; }

        public RelayCommand ChangeViewCommand { get; set; }

        public object CurrentView { get; set; }

        public RulesViewModel()
        {
            PresetsVM = new RuleSetsViewModel();
            GlobalRulesVM = new GlobalRulesViewModel();
            TimeLimitsVM = new TimeLimitsViewModel();
            RewardsVM = new RewardsViewModel();

            CurrentView = PresetsVM;

            ChangeViewCommand = new RelayCommand(ChangeView);
        }

        public void ChangeView(object? parameter)
        {
            CurrentView = parameter!;
            OnPropertyChanged(nameof(CurrentView));
        }
    }
}
