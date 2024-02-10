using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Zarp.GUI.Components
{
    internal class FilterableListBox : ListBox
    {
        private List<object> _Selected;

        public FilterableListBox()
        {
            _Selected = new List<object>();
            SelectionChanged += OnSelectionChanged;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            SelectionChanged -= OnSelectionChanged;
            base.OnItemsChanged(e);
            SetSelectedItems(_Selected);
            SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object obj in e.AddedItems)
            {
                _Selected.Add(obj);
            }

            foreach (object obj in e.RemovedItems)
            {
                _Selected.Remove(obj);
            }
        }
    }
}
