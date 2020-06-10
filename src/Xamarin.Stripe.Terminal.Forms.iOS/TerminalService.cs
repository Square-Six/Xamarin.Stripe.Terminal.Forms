using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using StripeTerminal;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class TerminalService : SCPDiscoveryDelegate, IStripeTerminalService
    {

        private SCPCancelable _cancelable = null;
        private SCPCancelable _discoveryCancelable = null;
        private List<SCPReader> _discoveredReaders = new List<SCPReader>();
        private Action<IList<StripeTerminalReader>> _onReadersDiscoveredAction;
        private Action<System.String> _readerMessageNotificationHandler;
        private Action<System.String> _readerConnectionNotificationHandler;
        private SCPReader _reader;
        private Boolean _isInitialized = false;
        private SCPReaderSoftwareUpdate _softwareUpdate;


        public override void DidUpdateDiscoveredReaders(SCPTerminal terminal, SCPReader[] readers)
        {
            if (readers.Any())
            {
                var stripeReaders = new List<StripeTerminalReader>();

                foreach (var reader in readers)
                {
                    stripeReaders.Add(new StripeTerminalReader
                    {
                        IsSimulated = reader.Simulated,
                        SerialNumber = reader.SerialNumber,
                        SoftwareVersion = reader.DeviceSoftwareVersion
                        //reader.BatteryLevel
                    }); ;
                }

                _discoveredReaders.AddRange(readers);

                // Report Back
                _onReadersDiscoveredAction(stripeReaders);

                _discoveryCancelable = null;
            }
        }


        public bool IsTerminalConnected => _isInitialized == false ? false : SCPTerminal.Shared.ConnectionStatus == SCPConnectionStatus.Connected;


        public String ArePermissionsGranted()
        {
            var failedPermissions = String.Empty;


            // Is Bluetooth Authorized
            if (CoreBluetooth.CBCentralManager.Authorization != CoreBluetooth.CBManagerAuthorization.AllowedAlways && CoreBluetooth.CBCentralManager.Authorization != CoreBluetooth.CBManagerAuthorization.NotDetermined)
            {
                failedPermissions = "Please go to your app settings and enable bluetooth access. Code:" + CoreBluetooth.CBCentralManager.Authorization.ToString();
            }

            // Is Location Athorized
            if (CoreLocation.CLLocationManager.Status == CoreLocation.CLAuthorizationStatus.Denied ||
                CoreLocation.CLLocationManager.Status == CoreLocation.CLAuthorizationStatus.Restricted)
            {
                failedPermissions += "Please go to your app settings and enable location services access. " + CoreLocation.CLLocationManager.Status.ToString();
            }

            return failedPermissions;
        }


        public void CancelDiscover()
        {
            _discoveryCancelable?.Cancel((NSError e) =>
            {
                // Do Nothing
            });
            _discoveryCancelable = null;
            _discoveredReaders.Clear();
        }


        public void ConnectToReader(StripeTerminalReader reader, Action<bool> onReaderConnectionSuccess)
        {
            var discoveredReader = _discoveredReaders.FirstOrDefault(x => x.SerialNumber == reader.SerialNumber);

            if (discoveredReader != null)
            {
                SCPTerminal.Shared.ConnectReader(discoveredReader, (SCPReader scpReader, NSError e) =>
                {
                    if (e == null)
                    {
                        _reader = discoveredReader;

                        onReaderConnectionSuccess(true);
                    }
                    else
                    {
                        onReaderConnectionSuccess(false);
                    }
                });
            }
        }


        public void ReconnectToReader(Action<bool> onReaderConnectionSuccess)
        {
            SCPTerminal.Shared.ConnectReader(_reader, (SCPReader scpReader, NSError e) =>
            {
                if (e == null)
                    onReaderConnectionSuccess(true);
                else
                    onReaderConnectionSuccess(false);
            });
        }


        public void DiscoverReaders(Action<IList<StripeTerminalReader>> readers, Action scanTimeoutCallback)
        {
            var configuration = new SCPDiscoveryConfiguration(SCPDeviceType.Chipper2X, SCPDiscoveryMethod.BluetoothScan, false);
            configuration.Timeout = 15;

            _onReadersDiscoveredAction = readers;
            _discoveryCancelable?.Cancel((NSError e) =>
            {
                // Notify Something
            });
            _discoveryCancelable = SCPTerminal.Shared.DiscoverReaders(configuration, this, (NSError e) =>
            {
                // Do Nothing...
                scanTimeoutCallback();
            });
        }


        public void InitTerminalManager()
        {
            if (_isInitialized == false)
            {
                try
                {
                    _isInitialized = true;
                    SCPTerminal.Shared.Delegate = new TerminalServiceTerminal(ConnectionNotifyHandler);
                }
                catch (Exception ex)
                {
                    throw new Exception("Location services are required in order to initialize the Terminal.", ex);
                }
            }
        }

        public void RetreivePaymentIntent(string clientSecret, Action<string> onSuccess, Action<string> onFailure)
        {
            SCPTerminal.Shared.RetrievePaymentIntent(clientSecret, (SCPPaymentIntent intentToCapture, NSError retreiveError) =>
            {
                if (retreiveError != null)
                {
                    onFailure(retreiveError.LocalizedDescription);
                }
                else
                {
                    _cancelable = SCPTerminal.Shared.CollectPaymentMethod(intentToCapture, new TerminalServiceReaderDisplay(ReaderNotifyHandler), (SCPPaymentIntent capturedIntent, NSError collectError) =>
                    {
                        if (collectError != null)
                        {
                            _cancelable = null;
                            onFailure(collectError.LocalizedDescription);
                        }
                        else
                        {
                            ReaderNotifyHandler("Authorizing Payment");

                            SCPTerminal.Shared.ProcessPayment(capturedIntent, (SCPPaymentIntent approvedIntent, SCPProcessPaymentError paymentError) =>
                            {
                                if (paymentError != null)
                                {
                                    _cancelable = null;
                                    onFailure(paymentError.LocalizedDescription);
                                }
                                else
                                {
                                    if (approvedIntent != null && System.String.IsNullOrEmpty(approvedIntent.StripeId) == false && approvedIntent.Status == SCPPaymentIntentStatus.RequiresCapture)
                                    {
                                        var paymentMethodId = approvedIntent.OriginalJSON["payment_method"]?.ToString() ?? String.Empty;

                                        _cancelable = null;
                                        onSuccess(paymentMethodId);
                                    }
                                    else
                                    {
                                        var errorMessage = System.String.Empty;

                                        if (approvedIntent == null)
                                            errorMessage = "No payment intent was found";

                                        else
                                            errorMessage = "You payment is in an invalid state try the checkout process again: " + approvedIntent.Status.ToString();

                                        _cancelable = null;
                                        onFailure(errorMessage);
                                    }
                                }
                            });
                        }
                    });

                }
            });
        }


        public void RegisterReaderMessageNotifications(Action<string> readerMessageNotificationHandler)
        {
            _readerMessageNotificationHandler = readerMessageNotificationHandler;
        }


        public void TearDownReaderMessageNotifications()
        {
            _readerMessageNotificationHandler = null;
        }


        public void RegisterConnectionMessageNotifications(Action<string> readerConnectionNotificationHandler)
        {
            _readerConnectionNotificationHandler = readerConnectionNotificationHandler;
        }


        public void TearDownConnectionMessageNotifications()
        {
            _readerConnectionNotificationHandler = null;
        }


        public void CancelPayment()
        {
            if (_cancelable != null)
            {
                _cancelable.Cancel((NSError) =>
                {

                });
            }
        }


        public void DisconnectReader()
        {
            SCPTerminal.Shared.DisconnectReader((ex) =>
            {

            });
        }


        public void CheckForSoftwareUpdate(Action<System.String, System.String> hasUpdate)
        {
            SCPTerminal.Shared.CheckForUpdate((readerUpdate, ex) =>
            {
                if (readerUpdate != null)
                {
                    _softwareUpdate = readerUpdate;

                    hasUpdate(readerUpdate.EstimatedUpdateTime.ToString(), null);
                }
                else if (ex != null)
                {
                    hasUpdate(null, ex?.LocalizedDescription);
                }

                hasUpdate(null, null);
            });
        }


        public void UpdateSoftware(Action<float> updateMessage, Action<string> complete)
        {
            SCPTerminal.Shared.InstallUpdate(_softwareUpdate, new ReaderSoftwareUpdate(updateMessage)
            , (ex) =>
            {
                _softwareUpdate = null;

                complete(ex?.LocalizedDescription);
            });
        }


        private void ReaderNotifyHandler(System.String message)
        {
            if (_readerMessageNotificationHandler != null)
                _readerMessageNotificationHandler(message);
        }


        private void ConnectionNotifyHandler(System.String message)
        {
            if (_readerConnectionNotificationHandler != null)
                _readerConnectionNotificationHandler(message);
        }


    }
}


public class TerminalServiceReaderDisplay : SCPReaderDisplayDelegate
{

    private Action<String> NotifyHandler;


    public TerminalServiceReaderDisplay(Action<String> notifyHandler)
    {
        NotifyHandler = notifyHandler;
    }


    public override void DidRequestReaderDisplayMessage(SCPTerminal terminal, SCPReaderDisplayMessage displayMessage)
    {
        var message = SCPTerminal.StringFromReaderDisplayMessage(displayMessage);

        NotifyHandler(message);
    }


    public override void DidRequestReaderInput(SCPTerminal terminal, SCPReaderInputOptions inputOptions)
    {
        var message = SCPTerminal.StringFromReaderInputOptions(inputOptions);

        NotifyHandler(message);
    }

}


public class TerminalServiceTerminal : SCPTerminalDelegate
{

    private Action<String> NotifyHandler;


    public TerminalServiceTerminal(Action<String> notifyHandler)
    {
        NotifyHandler = notifyHandler;
    }


    public override void Terminal(SCPTerminal terminal, SCPReader reader)
    {
        NotifyHandler("Disconnected");
    }


}


public class ReaderSoftwareUpdate : SCPReaderSoftwareUpdateDelegate
{

    private Action<float> NotifyHandler;


    public ReaderSoftwareUpdate(Action<float> notifyHandler)
    {
        NotifyHandler = notifyHandler;
    }


    public override void DidReportReaderSoftwareUpdateProgress(SCPTerminal terminal, float progress)
    {
        NotifyHandler(progress);
    }
}
