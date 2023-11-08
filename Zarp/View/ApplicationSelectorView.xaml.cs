using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zarp.Core;

namespace Zarp.View
{
    /// <summary>
    /// Interaction logic for ApplicationSelectorView.xaml
    /// </summary>
    public partial class ApplicationSelectorView : Window
    {
        public ApplicationSelectorView()
        {
            InitializeComponent();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            List<ApplicationInfo> list = new List<ApplicationInfo>();

            foreach (ApplicationInfo item in OpenApplicationsListBox.SelectedItems)
            {
                list.Add(item);
            }

            foreach (ApplicationInfo item in InstalledApplicationsListBox.SelectedItems)
            {
                list.Add(item);
            }

            foreach (ApplicationInfo item in UserSpecifiedApplicationsListBox.Items)
            {
                list.Add(item);
            }

            Zarp.Core.Zarp.DialogReturnValue = list;
        }
    }
}
