# Xamarin.Stripe.Terminal.Forms


## Support platforms

- [x] Android
- [x] iOS


## Initialization before use

iOS:

 - No initialization needed
 
 Android:
 
  - In the MainActivity.cs class OnCreate method add the following line of code.
  
  ```
  TerminalService.Init(this);
  ```
  
  
  ## Usage
  
   - In your shared code, create a class that implements the `IConnectionTokenProviderService` interface and overide the `FetchConnectionToken` method. This is where you will handle the logic to retreive your stripe connection token.
   
   ```
   public class MyStripeTokenFetchService : IConnectionTokenProviderService
   {
        public MyStripeTokenFetchService()
        {
        }

        public async Task<string> FetchConnectionToken()
        {
            var token = "{YOUR_STRIPE_TOKEN}";
            return token;
        }
    }
   ```

   - Create an instance of the TerminalService and call the `InitTerminalManager` passing in an instance of the class that implents the IConnectionTokenProviderService.
   
   ```
   stripeTerminalService.InitTerminalManager(myStripeTokenFetchService);
   ```
   
   * NOTE: Make sure that location permissions are enabled before calling `InitTemrinal`.


## IStripeTerminalService Interafce methods

```
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
void RegisterReaderMessageNotifications(Action<string> readerMessageNotificationHandler);
void TearDownReaderMessageNotifications();
void RegisterConnectionMessageNotifications(Action<string> readerConnectionNotificationHandler);
void TearDownConnectionMessageNotifications();
```



NOTE: If you run into any compiler issues with Java exited with Code 1 or DX8, on the Android side, try adding the folliwing nuget package to resolve the issue.

```sh
<PackageReference Include="Xamarin.Google.Guava.ListenableFuture" ExcludeAssets="build;buildTransitive">
   <Version>1.0.0.2</Version>
</PackageReference>
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
