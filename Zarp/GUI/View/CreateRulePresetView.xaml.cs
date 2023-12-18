using System.Windows;

namespace Zarp.GUI.View
{
    /// <summary>
    /// Interaction logic for TextInputWindow.xaml
    /// </summary>
    public partial class CreateRulePresetView : Window
    {
        public CreateRulePresetView()
        {
            InitializeComponent();
            NameField.Focus();
        }
    }
}
