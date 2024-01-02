using System.Windows;

namespace Zarp.GUI.View
{
    /// <summary>
    /// Interaction logic for CreateRewardView.xaml
    /// </summary>
    public partial class CreateRewardView : Window
    {
        public CreateRewardView()
        {
            InitializeComponent();
            NameField.Focus();
        }
    }
}
