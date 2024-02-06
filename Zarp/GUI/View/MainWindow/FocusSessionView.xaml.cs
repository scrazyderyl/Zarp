using System.Windows.Controls;

namespace Zarp.GUI.View.MainWindow
{
    internal partial class FocusSessionView : UserControl
    {
        public FocusSessionView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EventsSequence.NameChanged();
        }
    }
}
