using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zarp.GUI.ViewModel;
using System.Drawing;
using System.IO;
using static Zarp.GUI.Model.ApplicationSelectorModel;
using System.Windows.Controls;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;

namespace Zarp.GUI.View
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

            list.AddRange((InstalledApplicationsList.SelectedItems.Cast<ItemWithIcon<ApplicationInfo>>()).Select(item => item.Data));
            list.AddRange(OpenApplicationsList.SelectedItems.Cast<ApplicationInfo>());
            list.AddRange(OtherApplicationsList.SelectedItems.Cast<ApplicationInfo>());

            Core.Service.Zarp.DialogReturnValue = list;
        }
    }
}
