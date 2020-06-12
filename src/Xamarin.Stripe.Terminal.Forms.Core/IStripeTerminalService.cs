using System;
using System.Collections.Generic;

namespace Xamarin.Stripe.Terminal.Forms
{
    public interface IStripeTerminalService
    {
        bool IsTerminalConnected { get; }

        void InitTerminalManager(IConnectionTokenProviderService providerService);
        void DiscoverReaders(StripeDiscoveryConfiguration config, Action<IList<StripeTerminalReader>> readers, Action scanTimeoutCallback);
        void CancelDiscover();
        void ConnectToReader(StripeTerminalReader reader, Action<ReaderConnectionResult> onReaderConnectionSuccess);
        void ReconnectToReader(Action<bool> onReaderConnectionSuccess);
        void RetreivePaymentIntent(string clientSecret, Action<string> onSuccess, Action<string> onFailure);
        void CancelPayment();
        void DisconnectReader();
        string ArePermissionsGranted();
        void CheckForSoftwareUpdate(Action<string, string> hasUpdate);
        void UpdateSoftware(Action<float> updateMessage, Action<string> complete);

        /// <summary>
        /// Register callbacks into Xamarin Forms Screens
        /// </summary>
        void RegisterReaderMessageNotifications(Action<string> readerMessageNotificationHandler);

        /// <summary>
        /// Unregister callbacks into Xamarin Forms Screens
        /// </summary>
        void TearDownReaderMessageNotifications();

        /// <summary>
        /// Register callbacks into Xamarin Forms Screens
        /// </summary>
        void RegisterConnectionMessageNotifications(Action<string> readerConnectionNotificationHandler);

        /// <summary>
        /// Unregister callbacks into Xamarin Forms Screens
        /// </summary>
        void TearDownConnectionMessageNotifications();
    }
}
