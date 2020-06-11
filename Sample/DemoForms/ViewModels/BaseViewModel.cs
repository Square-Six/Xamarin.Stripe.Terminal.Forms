using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FreshMvvm;
using Mvvm.Async;
using Xamarin.Forms;

namespace DemoForms
{
    public abstract class BaseViewModel : FreshBasePageModel
    {
        public string LeftButtonText { get; set; }
        public Color LeftButtonColor { get; set; }
        public string TitleText { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                if (value)
                {
                    UserDialogs.Instance.ShowLoading();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        }

        private AsyncCommand _leftNavCommand;
        public AsyncCommand LeftNavCommand => _leftNavCommand ?? new AsyncCommand(HandleLeftNavAsync);

        public BaseViewModel()
        {
        }

        protected virtual Task HandleLeftNavAsync()
        {
            return Task.Delay(0);
        }
    }
}
