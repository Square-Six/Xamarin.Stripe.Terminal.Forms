using System;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class StripeTerminalReader
    {
        public Single BatteryLevel { get; set; }

        public Boolean IsSimulated { get; set; }

        public String SerialNumber { get; set; }

        public String SoftwareVersion { get; set; }
    }
}
