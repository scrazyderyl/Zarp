﻿using System.Windows;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.View
{
    internal partial class RenamePresetView : Window
    {
        public bool Confirmed = false;
        public string ChosenName => NameField.Text;
        private IPresetCollection _PresetCollection;

        public RenamePresetView(IPresetCollection collection)
        {
            _PresetCollection = collection;
            InitializeComponent();
            NameField.Focus();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            if (ChosenName.Equals(string.Empty))
            {
                MessageBox.Show("Invalid name");
                return;
            }

            if (_PresetCollection.Contains(ChosenName))
            {
                MessageBox.Show("A preset with the same name already exists");
                return;
            }

            Confirmed = true;
            Close();
        }
    }
}
