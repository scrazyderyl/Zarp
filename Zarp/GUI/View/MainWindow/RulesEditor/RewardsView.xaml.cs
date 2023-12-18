using System.Windows;
using System.Windows.Controls;

namespace Zarp.GUI.View.MainWindow.RulesEditor
{
    /// <summary>
    /// Interaction logic for RewardsView.xaml
    /// </summary>
    public partial class RewardsView : UserControl
    {
        public RewardsView()
        {
            InitializeComponent();
        }
        private void PresetChanged(object Sender, RoutedEventArgs e)
        {
            if (PresetSelector.SelectedItem == null)
            {
                RemoveButton.IsEnabled = false;
                RenameButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
                ExportButton.IsEnabled = false;
            }
            else
            {
                RemoveButton.IsEnabled = true;
                RenameButton.IsEnabled = true;
                DuplicateButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
            }
        }
    }
}
