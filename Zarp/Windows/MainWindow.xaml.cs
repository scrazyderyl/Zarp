using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Zarp.Core;
using Zarp.ViewModels;
using static Zarp.Core.PInvoke;

namespace Zarp
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

        }

        void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnClose(object sender, CancelEventArgs e)
        {
            //e.Cancel = true;
            //Hide();
        }

    }
}
