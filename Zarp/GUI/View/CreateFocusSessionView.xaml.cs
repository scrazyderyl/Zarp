using System.Windows;

namespace Zarp.GUI.View
{
    public partial class CreateFocusSessionView : Window
    {
        public CreateFocusSessionView()
        {
            InitializeComponent();
            NameField.Focus();
        }
    }
}
