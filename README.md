# Xamarin.Stripe.Terminal.Forms

[![NuGet](https://img.shields.io/nuget/v/Xamarin.Stripe.Terminal.Forms.svg)](https://www.nuget.org/packages/Xamarin.Stripe.Terminal.Forms/)

[Stripe][stripe] Terminal SDK Bindings for Xamarin.Forms

## Installation

Using the [.NET Core command-line interface (CLI) tools][dotnet-core-cli-tools]:

```sh
dotnet add package Xamarin.Stripe.Terminal.Forms
```

Using the [NuGet Command Line Interface (CLI)][nuget-cli]:

```sh
nuget install Xamarin.Stripe.Terminal.Forms
```

Using the [Package Manager Console][package-manager-console]:

```powershell
Install-Package Xamarin.Stripe.Terminal.Forms
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "Xamarin.Stripe.Terminal.Forms".
5. Click on the Xamarin.Stripe.Terminal.Forms package, select the appropriate version in the
   right-tab and click *Install*.


## Support platforms

- [x] Android
- [x] iOS


## Initialization before use

iOS:

 - Create a TokenProvider class that derives from `SCPConnectionTokenProvider` and override the `FetchConnectionToken`       method. That method is where you need to retrieve and return your Stripe Configuration Key.
 
 - Set the TokenProvder by calling the `SCPTerminal.SetTokenProvider(new TokenProvider())` method

 - Call the `StripeTerminal.InitTerminalManager()` method
 
 Android:
 
  - Create a TokenProvider class that derives from `IConnectionTokenProvider` and override the `FetchConnectionToken`       method. That method is where you need to retrieve and return your Stripe Configuration Key.
  
   ```
   public class TokenProvider : Java.Lang.Object, IConnectionTokenProvider
   {
   }
   ```
 
 - Set the TokenProvder by calling the `StripeTerminal.InitTerminal(Application.Context, tokenProvider, terminalService)` method

 - NOTE: Make sure that location permissions are enabled before calling `InitTemrinal`
  
 ```
 if (ContextCompat.CheckSelfPermission(MainActivity.Instance, Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
 if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted)
 {
    ActivityCompat.RequestPermissions(this, new[] { Android.Manifest.Permission.AccessFineLocation }, 10);
 }

 # listener is typeof(ITerminalListener)
 StripeTerminal.InitTerminal(Android.App.Application.Context, new TokenProvider(dataService), listener);
 ```


## IStripeTerminalService Interafce methods

```
void InitTerminalManager();
void DiscoverReaders(StripeDiscoveryConfiguration config, Action<IList<StripeTerminalReader>> readers, Action scanTimeoutCallback);;
void CancelDiscover();
void ConnectToReader(StripeTerminalReader reader, Action<ReaderConnectionResult> onReaderConnectionSuccess);
void ReconnectToReader(Action<Boolean> onReaderConnectionSuccess);
void RetreivePaymentIntent(String clientSecret, Action<String> onSuccess, Action<String> onFailure);
void RegisterReaderMessageNotifications(Action<String> readerMessageNotificationHandler);
void TearDownReaderMessageNotifications();
void RegisterConnectionMessageNotifications(Action<String> readerConnectionNotificationHandler);
void TearDownConnectionMessageNotifications();
void CancelPayment();
void DisconnectReader();
Boolean IsTerminalConnected { get; }
String ArePermissionsGranted();
void CheckForSoftwareUpdate(Action<String, String> hasUpdate);
void UpdateSoftware(Action<float> updateMessage, Action<string> complete);
```

## Native SDK Documentation

iOS:

- [Getting Started](https://stripe.com/docs/terminal/ios)
- [API Reference](https://stripe.github.io/stripe-terminal-ios/docs/index.html)

Android:

- [Getting Started](https://stripe.com/docs/terminal/sdk/android)
- [API Reference](https://stripe.dev/stripe-terminal-android)

For any requests, bug or comments, please [open an issue][issues] or [submit a
pull request][pulls].

[dotnet-core-cli-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/
[issues]: https://github.com/Square-Six/Xamarin.Stripe.Terminal.Forms/issues/new
[nuget-cli]: https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference
[package-manager-console]: https://docs.microsoft.com/en-us/nuget/tools/package-manager-console
[pulls]: https://github.com/Square-Six/Xamarin.Stripe.Terminal.Forms/issues/pulls
[stripe]: https://stripe.com
