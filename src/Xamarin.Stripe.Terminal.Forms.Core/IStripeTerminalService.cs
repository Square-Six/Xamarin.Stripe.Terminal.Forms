using System;
using System.Collections.Generic;

namespace Xamarin.Stripe.Terminal.Forms
{
    public interface IStripeTerminalService
    {
        void InitTerminalManager();

        void DiscoverReaders(Action<IList<StripeTerminalReader>> readers, Action scanTimeoutCallback);

        void CancelDiscover();

        void ConnectToReader(StripeTerminalReader reader, Action<Boolean> onReaderConnectionSuccess);

        void ReconnectToReader(Action<Boolean> onReaderConnectionSuccess);

        void RetreivePaymentIntent(String clientSecret, Action<String> onSuccess, Action<String> onFailure);

        /// <summary>
        /// Register callbacks into Xamarin Forms Screens
        /// </summary>
        void RegisterReaderMessageNotifications(Action<String> readerMessageNotificationHandler);

        /// <summary>
        /// Unregister callbacks into Xamarin Forms Screens
        /// </summary>
        void TearDownReaderMessageNotifications();

        /// <summary>
        /// Register callbacks into Xamarin Forms Screens
        /// </summary>
        void RegisterConnectionMessageNotifications(Action<String> readerConnectionNotificationHandler);

        /// <summary>
        /// Unregister callbacks into Xamarin Forms Screens
        /// </summary>
        void TearDownConnectionMessageNotifications();

        void CancelPayment();

        void DisconnectReader();

        Boolean IsTerminalConnected { get; }

        String ArePermissionsGranted();

        void CheckForSoftwareUpdate(Action<System.String, System.String> hasUpdate);

        void UpdateSoftware(Action<float> updateMessage, Action<string> complete);

    }
}
