using System.Windows;

namespace Zarp.GUI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
            Show();
            new ApplicationSelectorView().Show();
        }
    }
}
