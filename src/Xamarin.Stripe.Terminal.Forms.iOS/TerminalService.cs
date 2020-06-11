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
        private Action<string> _readerMessageNotificationHandler;
        private Action<string> _readerConnectionNotificationHandler;
        private SCPReader _reader;
        private bool _isInitialized = false;
        private SCPReaderSoftwareUpdate _softwareUpdate;

        public bool IsTerminalConnected => _isInitialized != false && SCPTerminal.Shared.ConnectionStatus == SCPConnectionStatus.Connected;

        public override void DidUpdateDiscoveredReaders(SCPTerminal terminal, SCPReader[] readers)
        {
            if (readers.Any())
            {
                var stripeReaders = new List<StripeTerminalReader>();

                foreach (var reader in readers)
                {
                    stripeReaders.Add(new StripeTerminalReader
                    {
                        IsSimulated = reader?.Simulated ?? false,
                        SerialNumber = reader?.SerialNumber,
                        SoftwareVersion = reader?.DeviceSoftwareVersion,
                        BatteryLevel = reader?.BatteryLevel?.FloatValue ?? 0
                    });
                }

                _discoveredReaders.AddRange(readers);
                _onReadersDiscoveredAction?.Invoke(stripeReaders);
                _discoveryCancelable = null;
            }
        }

        public string ArePermissionsGranted()
        {
            var failedPermissions = string.Empty;

            // Is Bluetooth Authorized
            if (CoreBluetooth.CBManager.Authorization != CoreBluetooth.CBManagerAuthorization.AllowedAlways && CoreBluetooth.CBManager.Authorization != CoreBluetooth.CBManagerAuthorization.NotDetermined)
            {
                failedPermissions = "Please go to your app settings and enable bluetooth access. Code:" + CoreBluetooth.CBManager.Authorization.ToString();
            }

            // Is Location Athorized
            if (CoreLocation.CLLocationManager.Status == CoreLocation.CLAuthorizationStatus.Denied || CoreLocation.CLLocationManager.Status == CoreLocation.CLAuthorizationStatus.Restricted)
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

        public void ConnectToReader(StripeTerminalReader reader, Action<ReaderConnectionResult> onReaderConnectionSuccess)
        {
            var discoveredReader = _discoveredReaders?.FirstOrDefault(x => x?.SerialNumber == reader?.SerialNumber);
            if (discoveredReader != null)
            {
                SCPTerminal.Shared.ConnectReader(discoveredReader, (SCPReader scpReader, NSError e) =>
                {
                    if (e == null)
                    {
                        _reader = discoveredReader;
                    }

                    var result = new ReaderConnectionResult
                    {
                        ErrorMessage = e?.LocalizedDescription,
                        IsConnected = e == null
                    };

                    onReaderConnectionSuccess?.Invoke(result);
                });
            }
        }

        public void ReconnectToReader(Action<bool> onReaderConnectionSuccess)
        {
            SCPTerminal.Shared.ConnectReader(_reader, (SCPReader scpReader, NSError e) =>
            {
                onReaderConnectionSuccess?.Invoke(e == null);
            });
        }

        public void DiscoverReaders(StripeDiscoveryConfiguration config, Action<IList<StripeTerminalReader>> readers, Action scanTimeoutCallback)
        {
            try
            {
                var deviceType = SCPDeviceType.Chipper2X;
                switch (config.DeviceType)
                {
                    case "Chipper2X":
                        deviceType = SCPDeviceType.Chipper2X;
                        break;
                    case "VerifoneP400":
                        deviceType = SCPDeviceType.VerifoneP400;
                        break;
                    case "WisePad3":
                        deviceType = SCPDeviceType.WisePad3;
                        break;
                }

                var disocveryMethod = SCPDiscoveryMethod.BluetoothScan;
                switch (config.DiscoveyMethod)
                {
                    case "BluetoothScan":
                        disocveryMethod = SCPDiscoveryMethod.BluetoothScan;
                        break;
                    case "BluetoothProximity":
                        disocveryMethod = SCPDiscoveryMethod.BluetoothProximity;
                        break;
                    case "Internet":
                        disocveryMethod = SCPDiscoveryMethod.BluetoothProximity;
                        break;
                }

                var configuration = new SCPDiscoveryConfiguration(deviceType, disocveryMethod, config.IsSimulated)
                {
                    Timeout = (nuint)config.TimeOut,
                };

                _onReadersDiscoveredAction = readers;
                _discoveryCancelable?.Cancel((NSError e) =>
                {
                    // Notify Something
                });
                _discoveryCancelable = SCPTerminal.Shared?.DiscoverReaders(configuration, this, (NSError e) =>
                {
                    // Do Nothing...
                    scanTimeoutCallback?.Invoke();
                });
            }
            catch (Exception e)
            {
                throw e;
            }
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
                    onFailure?.Invoke(retreiveError?.LocalizedDescription);
                }
                else
                {
                    _cancelable = SCPTerminal.Shared?.CollectPaymentMethod(intentToCapture, new TerminalServiceReaderDisplay(ReaderNotifyHandler), (SCPPaymentIntent capturedIntent, NSError collectError) =>
                    {
                        if (collectError != null)
                        {
                            _cancelable = null;
                            onFailure?.Invoke(collectError?.LocalizedDescription);
                        }
                        else
                        {
                            ReaderNotifyHandler("Authorizing Payment");

                            SCPTerminal.Shared?.ProcessPayment(capturedIntent, (SCPPaymentIntent approvedIntent, SCPProcessPaymentError paymentError) =>
                            {
                                if (paymentError != null)
                                {
                                    _cancelable = null;
                                    onFailure?.Invoke(paymentError?.LocalizedDescription);
                                }
                                else
                                {
                                    if (approvedIntent != null && string.IsNullOrEmpty(approvedIntent?.StripeId) == false && approvedIntent?.Status == SCPPaymentIntentStatus.RequiresCapture)
                                    {
                                        var paymentMethodId = approvedIntent?.OriginalJSON["payment_method"]?.ToString() ?? string.Empty;

                                        _cancelable = null;
                                        onSuccess?.Invoke(paymentMethodId);
                                    }
                                    else
                                    {
                                        var errorMessage = approvedIntent == null
                                            ? "No payment intent was found"
                                            : "You payment is in an invalid state try the checkout process again: " + approvedIntent?.Status.ToString();

                                        _cancelable = null;
                                        onFailure?.Invoke(errorMessage);
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

        public void CheckForSoftwareUpdate(Action<string, string> hasUpdate)
        {
            SCPTerminal.Shared.CheckForUpdate((readerUpdate, ex) =>
            {
                if (readerUpdate != null)
                {
                    _softwareUpdate = readerUpdate;
                    hasUpdate(readerUpdate?.EstimatedUpdateTime.ToString(), null);
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
            SCPTerminal.Shared.InstallUpdate(_softwareUpdate, new ReaderSoftwareUpdate(updateMessage), (ex) =>
            {
                _softwareUpdate = null;
                complete(ex?.LocalizedDescription);
            });
        }

        private void ReaderNotifyHandler(string message)
        {
            _readerMessageNotificationHandler?.Invoke(message);
        }

        private void ConnectionNotifyHandler(string message)
        {
            _readerConnectionNotificationHandler?.Invoke(message);
        }
    }
}

public class TerminalServiceReaderDisplay : SCPReaderDisplayDelegate
{
    private Action<string> NotifyHandler;

    public TerminalServiceReaderDisplay(Action<string> notifyHandler)
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
    private Action<string> NotifyHandler;

    public TerminalServiceTerminal(Action<string> notifyHandler)
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