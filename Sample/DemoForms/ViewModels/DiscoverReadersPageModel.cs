using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Mvvm.Async;
using Xamarin.Forms;
using Xamarin.Stripe.Terminal.Forms;

namespace DemoForms
{
    public class DiscoverReadersPageModel : BaseListViewModel<StripeTerminalReader>
    {
        private readonly IStripeTerminalService _stripeTerminalService;

        private StripeDiscoveryConfiguration _configuration;

        public bool IsConnectionStateVisible { get; set; }
        public bool IsUpdateAvailable { get; set; }
        public bool IsResultsVisible { get; set; } = true;

        private AsyncCommand _updateCommand;
        public AsyncCommand UpdateCommand => _updateCommand ?? new AsyncCommand(UpdateReaderAsync);

        private AsyncCommand _closeCommand;
        public AsyncCommand CloseCommand => _closeCommand ?? new AsyncCommand(HandleLeftNavAsync);

        private AsyncCommand _disconnectCommand;
        public AsyncCommand DisconnectCommand => _disconnectCommand ?? new AsyncCommand(DisconnectAsync);

        public DiscoverReadersPageModel(IStripeTerminalService stripeTerminalService)
        {
            _stripeTerminalService = stripeTerminalService;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is StripeDiscoveryConfiguration config)
            {
                _configuration = config;
            }

            LeftButtonText = "Cancel";
            LeftButtonColor = Color.FromHex("#007AFF");
            TitleText = "Discovery";
        }

        protected override void ViewIsAppearing(object sender, EventArgs args)
        {
            base.ViewIsAppearing(sender, args);

            var _ = DiscoverReadersAsync();
        }

        private async Task DiscoverReadersAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(1000);

                var isConnected = _stripeTerminalService.IsTerminalConnected;
                if (!isConnected)
                {
                    var failedPermissions = _stripeTerminalService.ArePermissionsGranted();
                    if (string.IsNullOrEmpty(failedPermissions))
                    {
                        _stripeTerminalService.DiscoverReaders(_configuration, OnReadersDiscovered, ScanTimeout);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await CoreMethods.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReadersDiscovered(IList<StripeTerminalReader> readers)
        {
            ListItems = new ObservableCollection<StripeTerminalReader>(readers);
            IsBusy = false;
        }

        private void ScanTimeout()
        {
            if (!ListItems?.Any() ?? false)
            {
                CoreMethods.DisplayAlert("", "No Payment Readers Found", "OK");
            }

            IsBusy = false;
        }

        protected override async Task OnItemSelected(object item)
        {
            if (item is StripeTerminalReader reader)
            {
                try
                {
                    IsBusy = true;
                    _stripeTerminalService.ConnectToReader(reader, ReaderConnectionResponse);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await base.OnItemSelected(item);
        }

        private void ReaderConnectionResponse(ReaderConnectionResult connectionResult)
        {
            if (connectionResult.IsConnected)
            {
                _stripeTerminalService.CancelDiscover();
                _stripeTerminalService.CheckForSoftwareUpdate((description, ex) =>
                {
                    IsBusy = false;

                    if (string.IsNullOrEmpty(description) == false)
                    {
                        CoreMethods.DisplayAlert("Update", "A software update is available for your reader. Estimated update time is " + description, "OK");
                        IsUpdateAvailable = true;
                    }
                    else if (string.IsNullOrEmpty(ex) == false)
                    {
                        Console.WriteLine(ex);
                        CoreMethods.DisplayAlert("Error", ex, "OK");
                    }
                });
            }
            else
            {
                CoreMethods.DisplayAlert("Error", connectionResult.ErrorMessage, "OK");
            }

            IsResultsVisible = !connectionResult.IsConnected;
            IsConnectionStateVisible = connectionResult.IsConnected;
            IsBusy = false;
        }

        private async Task UpdateReaderAsync()
        {
            try
            {
                IsBusy = true;

                _stripeTerminalService.UpdateSoftware((progress) =>
                {
                    Console.WriteLine($"Update Progress " + progress.ToString("P0"));
                }, async (ex) =>
                {
                    if (string.IsNullOrEmpty(ex))
                    {
                        await CoreMethods.DisplayAlert("Success", "Reader Software is Updated! You may start using the reader.", "OK");
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Error", "Software update failed. " + ex, "OK");
                    }
                    IsBusy = false;
                });
            }
            catch(Exception e)
            {
                await CoreMethods.DisplayAlert("Error", e.Message, "OK");
                IsBusy = false;
            }
        }

        private Task DisconnectAsync()
        {
            _stripeTerminalService.DisconnectReader();
            return CoreMethods.PopPageModel(true);
        }

        protected override Task HandleLeftNavAsync()
        {
            return CoreMethods.PopPageModel(true);
        }
    }
}
