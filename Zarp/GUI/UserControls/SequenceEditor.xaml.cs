using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for SequenceEditor.xaml
    /// </summary>
    public partial class SequenceEditor : UserControl
    {
        public static readonly DependencyProperty ItemListProperty = DependencyProperty.Register(nameof(ItemList), typeof(IList), typeof(SequenceEditor), new FrameworkPropertyMetadata(OnItemListChanged));

        public IList? ItemList
        {
            get => (IList)GetValue(ItemListProperty);
            set => SetValue(ItemListProperty, value);
        }

        private static void OnItemListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SequenceEditor)d).OnItemListChanged();
        }

        private void OnItemListChanged()
        {
            ItemNames = new ObservableCollection<string>();

            if (ItemList == null)
            {
                Add.IsEnabled = false;
            }
            else
            {
                foreach (object item in ItemList)
                {
                    ItemNames.Add(item.ToString()!);
                }

                Add.IsEnabled = true;
            }

            Selector.ItemsSource = ItemNames;
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SequenceEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public object? SelectedItem
        {
            get => (object?)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SequenceEditor)d).OnSelectedItemChanged();
        }

        private void OnSelectedItemChanged()
        {
            if (SelectedItemChangedExternally && ItemList != null && SelectedItem != null)
            {
                Selector.SelectedIndex = ItemList.IndexOf(SelectedItem);
            }
        }

        private bool SelectedItemChangedExternally = true;
        public ObservableCollection<string>? ItemNames;

        public SequenceEditor()
        {
            InitializeComponent();
        }

        public void NameChanged()
        {
            SelectedItemChangedExternally = false;
            int index = Selector.SelectedIndex;
            ItemNames![index] = ItemList![index]!.ToString()!;
            Selector.SelectedIndex = index;
            SelectedItemChangedExternally = true;
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItemChangedExternally = false;

            if (Selector.SelectedIndex == -1)
            {
                SelectedItem = null;
                Remove.IsEnabled = false;
                MoveUp.IsEnabled = false;
                MoveDown.IsEnabled = false;
            }
            else
            {
                SelectedItem = ItemList![Selector.SelectedIndex];
                Remove.IsEnabled = true;

                if (Selector.SelectedIndex == 0)
                {
                    MoveUp.IsEnabled = false;
                }
                else
                {
                    MoveUp.IsEnabled = true;
                }

                if (Selector.SelectedIndex == ItemNames!.Count - 1)
                {
                    MoveDown.IsEnabled = false;
                }
                else
                {
                    MoveDown.IsEnabled = true;
                }
            }

            SelectedItemChangedExternally = true;
        }

        private static int count = 0;

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            object newItem = new Event(count++.ToString(), 30, TimeUnit.Minutes, EventType.Regular, null);

            if (Selector.SelectedIndex == -1)
            {
                ItemList!.Add(newItem);
                ItemNames!.Add(newItem.ToString()!);
                Selector.SelectedIndex = ItemNames!.Count - 1;
            }
            else
            {
                int newIndex = Selector.SelectedIndex + 1;
                ItemList!.Insert(newIndex, newItem);
                ItemNames!.Insert(newIndex, newItem.ToString()!);
                Selector.SelectedIndex = newIndex;
            }

            Selector.Focus();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = Selector.SelectedIndex - 1;
            ItemList!.RemoveAt(Selector.SelectedIndex);
            ItemNames!.RemoveAt(Selector.SelectedIndex);
            Selector.SelectedIndex = newIndex == -1 && ItemNames.Count > 0 ? 0 : newIndex;

            Selector.Focus();
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            object? temp = ItemList![Selector.SelectedIndex];
            ItemList[Selector.SelectedIndex] = ItemList[Selector.SelectedIndex - 1];
            ItemList[Selector.SelectedIndex - 1] = temp;

            string tempName = ItemNames![Selector.SelectedIndex];
            int newIndex = Selector.SelectedIndex - 1;
            ItemNames[Selector.SelectedIndex] = ItemNames[Selector.SelectedIndex - 1];
            ItemNames[newIndex] = tempName;
            Selector.SelectedIndex = newIndex;

            Selector.Focus();
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            object? temp = ItemList![Selector.SelectedIndex];
            ItemList[Selector.SelectedIndex] = ItemList[Selector.SelectedIndex + 1];
            ItemList[Selector.SelectedIndex + 1] = temp;

            string tempName = ItemNames![Selector.SelectedIndex];
            int newIndex = Selector.SelectedIndex + 1;
            ItemNames[Selector.SelectedIndex] = ItemNames[Selector.SelectedIndex + 1];
            ItemNames[newIndex] = tempName;
            Selector.SelectedIndex = newIndex;

            Selector.Focus();
        }
    }
}
