﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zarp.View.MainWindow
{
    /// <summary>
    /// Interaction logic for FocusSessionView.xaml
    /// </summary>
    public partial class FocusSessionView : UserControl
    {
        public FocusSessionView()
        {
            InitializeComponent();
        }

        private void PresetChanged(object sender, RoutedEventArgs e)
        {
            if (PresetSelector.SelectedItem == null)
            {
                RemoveButton.IsEnabled = false;
                DuplicateButton.IsEnabled = false;
                ExportButton.IsEnabled = false;
            }
            else
            {
                RemoveButton.IsEnabled = true;
                DuplicateButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
            }
        }
    }
}
