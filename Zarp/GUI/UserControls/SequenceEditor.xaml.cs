using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Zarp.GUI.UserControls
{
    internal partial class SequenceEditor : UserControl
    {
        private static readonly object[] Empty = new object[0];

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
            object? deselected = Selector.SelectedIndex == -1 ? null : Items![Selector.SelectedIndex]!;

            if (ItemList == null)
            {
                Items = new ObservableCollection<object>();
                Add.IsEnabled = false;
            }
            else
            {
                Items = new ObservableCollection<object>();

                foreach (object item in ItemList)
                {
                    Items.Add(item);
                }

                Add.IsEnabled = true;
            }

            _SuppressPropertyChangedCallback = true;
            _SuppressSelectionChangedEvent = true;

            Selector.SelectedIndex = -1;
            Selector.ItemsSource = Items;
            Remove.IsEnabled = false;
            MoveUp.IsEnabled = false;
            MoveDown.IsEnabled = false;

            SelectedIndex = -1;
            SelectedItem = null;

            if (deselected != null)
            {
                RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, new object[] { deselected }, Empty));
            }

            _SuppressSelectionChangedEvent = false;
            _SuppressPropertyChangedCallback = false;
        }

        public static readonly DependencyProperty CreateFunctionProperty = DependencyProperty.Register(nameof(CreateFunction), typeof(Func<object>), typeof(SequenceEditor));

        public Func<object?> CreateFunction
        {
            get => (Func<object?>)GetValue(CreateFunctionProperty);
            set => SetValue(CreateFunctionProperty, value);
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(object), typeof(SequenceEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SequenceEditor)d).OnSelectedIndexChanged();
        }

        private void OnSelectedIndexChanged()
        {
            if (!_SuppressPropertyChangedCallback && ItemList != null)
            {
                Selector.SelectedIndex = SelectedIndex;
            }
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
            if (!_SuppressPropertyChangedCallback && ItemList != null && SelectedItem != null)
            {
                Selector.SelectedIndex = ItemList.IndexOf(SelectedItem);
            }
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(SequenceEditor));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public static readonly RoutedEvent ItemAddedEvent = EventManager.RegisterRoutedEvent(nameof(ItemAdded), RoutingStrategy.Bubble, typeof(ItemAddedRemovedEventHandler), typeof(SequenceEditor));

        public event ItemAddedRemovedEventHandler ItemAdded
        {
            add => AddHandler(ItemAddedEvent, value);
            remove => RemoveHandler(ItemAddedEvent, value);
        }

        public static readonly RoutedEvent ItemRemovedEvent = EventManager.RegisterRoutedEvent(nameof(ItemRemoved), RoutingStrategy.Bubble, typeof(ItemAddedRemovedEventHandler), typeof(SequenceEditor));

        public event ItemAddedRemovedEventHandler ItemRemoved
        {
            add => AddHandler(ItemRemovedEvent, value);
            remove => RemoveHandler(ItemRemovedEvent, value);
        }

        private bool _SuppressPropertyChangedCallback;
        private bool _SuppressSelectionChangedEvent;
        public ObservableCollection<object>? Items;

        public SequenceEditor()
        {
            InitializeComponent();
        }

        public void NameChanged()
        {
            _SuppressSelectionChangedEvent = true;

            int index = Selector.SelectedIndex;
            Items![index] = string.Empty;
            Items![index] = ItemList![index]!;
            Selector.SelectedIndex = index;

            _SuppressSelectionChangedEvent = false;
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_SuppressSelectionChangedEvent)
            {
                return;
            }

            _SuppressPropertyChangedCallback = true;

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

                if (Selector.SelectedIndex == Items!.Count - 1)
                {
                    MoveDown.IsEnabled = false;
                }
                else
                {
                    MoveDown.IsEnabled = true;
                }
            }

            SelectedIndex = Selector.SelectedIndex;

            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.RemovedItems, e.AddedItems));

            _SuppressPropertyChangedCallback = false;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            object? newItem = CreateFunction();

            if (newItem == null)
            {
                return;
            }

            _SuppressPropertyChangedCallback = true;
            _SuppressSelectionChangedEvent = true;

            if (Selector.SelectedIndex == -1)
            {
                ItemList!.Add(newItem);
                Items!.Add(newItem);
                Selector.SelectedIndex = Items!.Count - 1;
                Remove.IsEnabled = true;
            }
            else
            {
                int newIndex = Selector.SelectedIndex + 1;
                ItemList!.Insert(newIndex, newItem);
                Items!.Insert(newIndex, newItem);
                Selector.SelectedIndex = newIndex;
            }

            if (Selector.SelectedIndex != 0)
            {
                MoveUp.IsEnabled = true;
            }

            SelectedIndex = Selector.SelectedIndex;
            SelectedItem = newItem;

            _SuppressSelectionChangedEvent = false;
            _SuppressPropertyChangedCallback = false;

            RaiseEvent(new ItemAddedRemovedEventArgs(ItemAddedEvent, newItem));
            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, Empty, new object[] { newItem }));

            Selector.Focus();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            _SuppressPropertyChangedCallback = true;
            _SuppressSelectionChangedEvent = true;

            object? removed = ItemList![Selector.SelectedIndex];
            ItemList!.RemoveAt(Selector.SelectedIndex);
            Items!.RemoveAt(Selector.SelectedIndex);
            Remove.IsEnabled = false;
            MoveUp.IsEnabled = false;
            MoveDown.IsEnabled = false;

            SelectedIndex = -1;
            SelectedItem = null;

            _SuppressSelectionChangedEvent = false;
            _SuppressPropertyChangedCallback = false;

            RaiseEvent(new ItemAddedRemovedEventArgs(ItemRemovedEvent, removed));
            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, new object?[] { removed }, Empty));

            Selector.Focus();
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            object? temp = ItemList![Selector.SelectedIndex];
            ItemList[Selector.SelectedIndex] = ItemList[Selector.SelectedIndex - 1];
            ItemList[Selector.SelectedIndex - 1] = temp;

            _SuppressPropertyChangedCallback = true;
            _SuppressSelectionChangedEvent = true;

            temp = Items![Selector.SelectedIndex];
            int newIndex = Selector.SelectedIndex - 1;
            Items[Selector.SelectedIndex] = Items[Selector.SelectedIndex - 1];
            Items[newIndex] = temp;
            Selector.SelectedIndex = newIndex;

            if (Selector.SelectedIndex == 0)
            {
                MoveUp.IsEnabled = false;
            }

            MoveDown.IsEnabled = true;

            SelectedIndex = Selector.SelectedIndex;

            _SuppressSelectionChangedEvent = false;
            _SuppressPropertyChangedCallback = false;

            Selector.Focus();
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            object? temp = ItemList![Selector.SelectedIndex];
            ItemList[Selector.SelectedIndex] = ItemList[Selector.SelectedIndex + 1];
            ItemList[Selector.SelectedIndex + 1] = temp;

            _SuppressPropertyChangedCallback = true;
            _SuppressSelectionChangedEvent = true;

            temp = Items![Selector.SelectedIndex];
            int newIndex = Selector.SelectedIndex + 1;
            Items[Selector.SelectedIndex] = Items[Selector.SelectedIndex + 1];
            Items[newIndex] = temp;
            Selector.SelectedIndex = newIndex;

            if (Selector.SelectedIndex == Items!.Count - 1)
            {
                MoveDown.IsEnabled = false;
            }

            MoveUp.IsEnabled = true;

            SelectedIndex = Selector.SelectedIndex;

            _SuppressSelectionChangedEvent = false;
            _SuppressPropertyChangedCallback = false;

            Selector.Focus();
        }
    }
}
