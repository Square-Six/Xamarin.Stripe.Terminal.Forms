using System;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class StripeTerminalReader
    {
        public float BatteryLevel { get; set; }
        public bool IsSimulated { get; set; }
        public string SerialNumber { get; set; }
        public string SoftwareVersion { get; set; }

        //TODO: Add mapping
        // public DeviceType DeviceType { get; set; }
    }
}
