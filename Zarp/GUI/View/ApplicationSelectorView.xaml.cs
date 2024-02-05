using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zarp.Core.Datatypes;
using Zarp.GUI.DataTypes;

namespace Zarp.GUI.View
{
    public partial class ApplicationSelectorView : Window
    {
        public bool Confirmed = false;
        public IEnumerable<ApplicationInfo> Selected => InstalledApplicationsList.SelectedItems.Cast<ItemWithIcon<ApplicationInfo>>().Select(item => item.Data)
            .Concat(OpenApplicationsList.SelectedItems.Cast<ItemWithIcon<ApplicationInfo>>().Select(item => item.Data))
            .Concat(OtherApplicationsList.Items.Cast<ItemWithIcon<ApplicationInfo>>().Select(item => item.Data));

        public ApplicationSelectorView()
        {
            InitializeComponent();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }
    }
}
