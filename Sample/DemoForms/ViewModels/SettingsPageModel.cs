using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using DemoForms.Enums;
using DemoForms.Models;
using Xamarin.Forms;
using Xamarin.Stripe.Terminal.Forms;

namespace DemoForms
{
    public class SettingsPageModel : BaseListViewModel<BaseModel>
    {
        private readonly IStripeTerminalService _stripeTerminalService;

        public string DeviceType { get; set; } = "Chipper2X";
        public string DiscoveryMethod { get; set; } = "BluetoothScan";

        public SettingsPageModel(IStripeTerminalService stripeTerminalService)
        {
            _stripeTerminalService = stripeTerminalService;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            SetListItems();
        }

        private void SetListItems()
        {
            var items = new List<BaseModel>();
            items.Add(new StatusModel("No Reader Connected"));
            items.Add(new SettingsItemModel("Discover Readers", "READER CONNECTION", Color.FromHex("#007AFF"), SettingsType.Discover, false));
            items.Add(new SettingsItemModel(DeviceType, "DEVICE TYPE", Color.Black, SettingsType.DeviceType));
            items.Add(new SettingsItemModel(DiscoveryMethod, "DISCOVERY METHOD", Color.Black, SettingsType.DiscoveryMethod));

            ListItems = new ObservableCollection<BaseModel>(items);
        }

        protected override async Task OnItemSelected(object item)
        {
            if (item is SettingsItemModel itemModel)
            {
                switch(itemModel.SettingsType)
                {
                    case SettingsType.DeviceType:
                        await HandleDeviceTypesSelection();
                        break;
                    case SettingsType.Discover:
                        var config = new StripeDiscoveryConfiguration
                        {
                            TimeOut = 15,
                            DeviceType = DeviceType,
                            DiscoveyMethod = DiscoveryMethod,
                            IsSimulated = false
                        };
                        await CoreMethods.PushPageModel<DiscoverReadersPageModel>(config, true);
                        break;
                    case SettingsType.DiscoveryMethod:
                        await HandleDiscoverSelection();
                        break;
                }
            }

            await base.OnItemSelected(item);
        }

        private async Task HandleDiscoverSelection()
        {
            var cancel = "Cancel";
            var deviceTypes = new string[] { "BluetoothScan", "BluetoothProximity", "Internet" };
            var result = await UserDialogs.Instance.ActionSheetAsync("Select Discovery Method", cancel, null, buttons: deviceTypes);
            if (result != cancel)
            {
                DiscoveryMethod = result;
                SetListItems();
            }
        }

        private async Task HandleDeviceTypesSelection()
        {
            var cancel = "Cancel";
            var deviceTypes = new string[] { "Chipper2X", "VerifoneP400", "WisePad3" };
            var result = await UserDialogs.Instance.ActionSheetAsync("Select Device Type", cancel, null, buttons: deviceTypes);
            if (result != cancel)
            {
                DeviceType = result;
                SetListItems();
            }
        }
    }
}
