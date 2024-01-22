using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;
using Zarp.GUI.View;

namespace Zarp.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for PresetSelector.xaml
    /// </summary>
    public partial class PresetSelector : UserControl
    {
        public static readonly DependencyProperty PresetCollectionProperty = DependencyProperty.Register(nameof(PresetCollection), typeof(PresetCollection), typeof(PresetSelector), new FrameworkPropertyMetadata(OnPresetCollectionChanged));

        public PresetCollection? PresetCollection
        {
            get => (PresetCollection)GetValue(PresetCollectionProperty);
            set => SetValue(PresetCollectionProperty, value);
        }

        private static void OnPresetCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PresetSelector)d).OnPresetCollectionChanged();
        }

        protected void OnPresetCollectionChanged()
        {
            if (PresetCollection == null)
            {
                PresetList = new ObservableCollection<string>();
            }
            else
            {
                PresetList = new ObservableCollection<string>(PresetCollection);
            }

            Selector.ItemsSource = PresetList;
        }

        public static readonly DependencyProperty CreateFunctionProperty = DependencyProperty.Register(nameof(CreateFunction), typeof(Func<Preset>), typeof(PresetSelector));

        public Func<Preset?> CreateFunction
        {
            get => (Func<Preset>)GetValue(CreateFunctionProperty);
            set => SetValue(CreateFunctionProperty, value);
        }

        public static readonly DependencyProperty SelectedPresetProperty = DependencyProperty.Register(nameof(SelectedPreset), typeof(Preset), typeof(PresetSelector), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPresetChanged));

        public Preset? SelectedPreset
        {
            get => (Preset?)GetValue(SelectedPresetProperty);
            set => SetValue(SelectedPresetProperty, value);
        }

        private static void OnSelectedPresetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PresetSelector)d).OnSelectedPresetChanged();
        }

        private void OnSelectedPresetChanged()
        {
            if (SelectedItemChangedExternally && PresetList != null && SelectedPreset != null)
            {
                Selector.SelectedIndex = PresetList.IndexOf(SelectedPreset.Name);
            }
        }

        private bool SelectedItemChangedExternally = true;
        public ObservableCollection<string>? PresetList;

        public PresetSelector()
        {
            InitializeComponent();
        }

        private void Selector_SelectionChanged(object Sender, RoutedEventArgs e)
        {
            SelectedItemChangedExternally = false;

            if (Selector.SelectedIndex == -1)
            {
                Options.IsEnabled = false;
                SelectedPreset = null;
            }
            else
            {
                Options.IsEnabled = true;
                SelectedPreset = PresetCollection![(string)Selector.SelectedItem];
            }

            SelectedItemChangedExternally = true;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Preset? result = CreateFunction();

            if (result != null)
            {
                PresetCollection!.Add(result);
                PresetList?.Add(result.Name);
                Selector.SelectedIndex = Selector.Items.Count - 1;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            string[] fileNames = FileDialogs.OpenJSONMulti();

            foreach (string fileName in fileNames)
            {

            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            RenamePresetView renameView = new RenamePresetView(PresetCollection!);
            renameView.ShowDialog();

            if (!renameView.Confirmed)
            {
                return;
            }

            if (renameView.Confirmed && PresetCollection!.Rename(SelectedPreset!.Name, renameView.ChosenName))
            {
                PresetList?.RemoveAt(Selector.SelectedIndex);
                PresetList?.Add(renameView.ChosenName);
                Selector.SelectedIndex = Selector.Items.Count - 1;
            }
        }

        private void Duplicate_Click(object sender, RoutedEventArgs e)
        {
            RenamePresetView renameView = new RenamePresetView(PresetCollection!);
            renameView.ShowDialog();

            if (!renameView.Confirmed)
            {
                return;
            }

            PresetCollection!.Add(SelectedPreset!.Duplicate(renameView.ChosenName));
            PresetList?.Add(renameView.ChosenName);
            Selector.SelectedIndex = Selector.Items.Count - 1;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileDialogs.SaveJSON();

            if (!fileName.Equals(string.Empty))
            {

            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            PresetCollection!.Remove(SelectedPreset!.Name);
            int selectedIndex = Selector.SelectedIndex;
            PresetList?.RemoveAt(selectedIndex);
            Selector.SelectedIndex = Selector.Items.Count == selectedIndex ? Selector.Items.Count - 1 : selectedIndex;
        }
    }
}