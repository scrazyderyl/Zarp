using System.Windows.Controls;
using Zarp.Core.Datatypes;
using Zarp.GUI.UserControls;
using Zarp.GUI.ViewModel.MainWindow;

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

        private void EventsSequence_ItemRemoved(object sender, ItemAddedRemovedEventArgs e)
        {
            ((FocusSessionViewModel)this.DataContext).SelectedFocusSession!.SetEventRuleSet((FocusSessionEvent)e.Item!, null);
        }
    }
}
