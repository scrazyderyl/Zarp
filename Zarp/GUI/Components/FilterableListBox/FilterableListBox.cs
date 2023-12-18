using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Zarp.GUI.Components
{
    public class FilterableListBox : ListBox
    {
        private List<object> Selected;

        public FilterableListBox()
        {
            Selected = new List<object>();
            SelectionChanged += OnSelectionChanged;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            SelectionChanged -= OnSelectionChanged;
            base.OnItemsChanged(e);
            SetSelectedItems(Selected);
            SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object obj in e.AddedItems)
            {
                Selected.Add(obj);
            }

            foreach (object obj in e.RemovedItems)
            {
                Selected.Remove(obj);
            }
        }
    }
}
