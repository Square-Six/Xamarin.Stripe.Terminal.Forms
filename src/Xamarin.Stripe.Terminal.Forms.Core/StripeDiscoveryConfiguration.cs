using System;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class StripeDiscoveryConfiguration
    {
        /// <summary>
        /// 15 by default
        /// </summary>
        public int TimeOut { get; set; } = 15;

        /// <summary>
        /// For iOS Only. Chipper2X by default
        /// </summary>
        public string DeviceType { get; set; } = "Chipper2X";

        /// <summary>
        /// For iOS Only. BluetoothScan by default
        /// </summary>
        public string DiscoveyMethod { get; set; } = "BluetoothScan";

        /// <summary>
        /// False by default
        /// </summary>
        public bool IsSimulated { get; set; }
    }
}
