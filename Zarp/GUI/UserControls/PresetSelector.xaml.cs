using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zarp.Core.Datatypes;
using Zarp.GUI.View;

namespace Zarp.GUI.UserControls
{
    internal partial class PresetSelector : UserControl
    {
        public static readonly DependencyProperty PresetCollectionProperty = DependencyProperty.Register(nameof(PresetCollection), typeof(IPresetCollection), typeof(PresetSelector), new FrameworkPropertyMetadata(OnPresetCollectionChanged));

        public IPresetCollection? PresetCollection
        {
            get => (IPresetCollection)GetValue(PresetCollectionProperty);
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
                PresetList = new ObservableCollection<Preset>();
            }
            else
            {
                PresetList = new ObservableCollection<Preset>(PresetCollection);
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
            if (_SelectedItemChangedExternally && PresetList != null && SelectedPreset != null)
            {
                Selector.SelectedIndex = PresetList.IndexOf(SelectedPreset);
            }
        }

        private bool _SelectedItemChangedExternally = true;
        public ObservableCollection<Preset>? PresetList;

        public PresetSelector()
        {
            InitializeComponent();
        }

        private void Selector_SelectionChanged(object Sender, RoutedEventArgs e)
        {
            _SelectedItemChangedExternally = false;

            if (Selector.SelectedIndex == -1)
            {
                Options.IsEnabled = false;
                SelectedPreset = null;
            }
            else
            {
                Preset selected = (Preset)Selector.SelectedItem;
                Options.IsEnabled = true;
                SelectedPreset = selected;

                Delete.IsEnabled = selected.IsDeletable;
            }

            _SelectedItemChangedExternally = true;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Preset? result = CreateFunction();

            if (result == null)
            {
                return;
            }

            PresetList?.Add(result);
            Selector.SelectedIndex = Selector.Items.Count - 1;
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            RenamePresetView renameView = new RenamePresetView(PresetCollection!);
            renameView.ShowDialog();

            if (!renameView.Confirmed)
            {
                return;
            }

            Preset selected = (Preset)Selector.SelectedItem!;

            if (renameView.Confirmed && PresetCollection!.Rename(selected.Guid, renameView.ChosenName))
            {
                PresetList?.RemoveAt(Selector.SelectedIndex);
                PresetList?.Add(selected);
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

            Preset newPreset = SelectedPreset!.Duplicate(renameView.ChosenName);

            PresetCollection!.Add(newPreset);
            PresetList?.Add(newPreset);
            Selector.SelectedIndex = Selector.Items.Count - 1;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            PresetCollection!.Remove(SelectedPreset!.Guid);
            int selectedIndex = Selector.SelectedIndex;
            PresetList?.RemoveAt(selectedIndex);
            Selector.SelectedIndex = Selector.Items.Count == selectedIndex ? Selector.Items.Count - 1 : selectedIndex;
        }
    }
}