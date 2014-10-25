using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Sledge.Gui.Structures
{
    public class ItemList<T> : ObservableCollection<T>
    {
        // http://blogs.msdn.com/b/nathannesbit/archive/2009/04/20/addrange-and-observablecollection.aspx
        public void AddRange(IEnumerable<T> dataToAdd)
        {
            CheckReentrancy();
            var startingIndex = Count;
            var add = dataToAdd.ToList();
            foreach (var data in add)
            {
                Items.Add(data);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, add, startingIndex));
        }
    }
}