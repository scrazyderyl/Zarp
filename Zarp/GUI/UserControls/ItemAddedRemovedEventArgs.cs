using System.Windows;

namespace Zarp.GUI.UserControls
{
    internal class ItemAddedRemovedEventArgs : RoutedEventArgs
    {
        private readonly object? _Item;

        public object? Item
        {
            get => _Item;
        }

        public ItemAddedRemovedEventArgs(RoutedEvent routedEvent, object? item) : base(routedEvent)
        {
            _Item = item;
        }
    }
}
