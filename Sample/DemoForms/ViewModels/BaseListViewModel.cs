using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DemoForms
{
    public abstract class BaseListViewModel<T> : BaseViewModel
    {
        protected virtual bool AutoDeselectItem => true;

        public ObservableCollection<T> ListItems { get; set; }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnItemSelected(value);
            }
        }

        protected virtual Task OnItemSelected(object item)
        {
            if (AutoDeselectItem)
            {
                SelectedItem = null;
            }

            return Task.Delay(0);
        }
    }
}
