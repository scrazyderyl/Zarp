using System.Windows;
using System.Windows.Controls;

namespace Zarp.GUI.View.MainWindow
{
    /// <summary>
    /// Interaction logic for FocusSessionView.xaml
    /// </summary>
    public partial class FocusSessionView : UserControl
    {
        public FocusSessionView()
        {
            InitializeComponent();
        }

        private void PresetChanged(object sender, RoutedEventArgs e)
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
