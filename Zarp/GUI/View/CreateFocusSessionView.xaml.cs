using System.Windows;

namespace Zarp.GUI.View
{
    internal partial class CreateFocusSessionView : Window
    {
        public CreateFocusSessionView()
        {
            InitializeComponent();
            NameField.Focus();
        }
    }
}
