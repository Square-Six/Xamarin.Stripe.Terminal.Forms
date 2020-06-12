using System;
using Foundation;
using StripeTerminal;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class StripeConnectionTokenProvider : SCPConnectionTokenProvider
    {
        private readonly IConnectionTokenProviderService _connectionTokenProviderService;

        private int _retries = 1;
        private const int MaxRetries = 3;

        public StripeConnectionTokenProvider(IConnectionTokenProviderService connectionTokenProviderService)
        {
            _connectionTokenProviderService = connectionTokenProviderService;
        }

        public override async void FetchConnectionToken(SCPConnectionTokenCompletionBlock completion)
        {
            var isSuccess = false;

            do
            {
                try
                {
                    var token = await _connectionTokenProviderService.FetchConnectionToken();
                    isSuccess = true;
                    completion(token, null);
                }
                catch (Exception ex)
                {
                    if (_retries >= MaxRetries)
                    {
                        completion(null, new NSError(new NSString(ex.Message), 0));
                    }
                }

                _retries++;

            } while (_retries < MaxRetries && isSuccess == false);
        }
    }
}
