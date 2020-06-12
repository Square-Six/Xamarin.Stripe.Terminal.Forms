using System;
using Com.Stripe.Stripeterminal.Callable;
using Com.Stripe.Stripeterminal.Model.External;
using Java.Lang;

namespace Xamarin.Stripe.Terminal.Forms
{
    public class StripeConnectionTokenProvider : Java.Lang.Object, IConnectionTokenProvider
    {
        private readonly IConnectionTokenProviderService _connectionTokenProviderService;

        private int _retries = 1;
        private const int MaxRetries = 3;

        public StripeConnectionTokenProvider(IConnectionTokenProviderService connectionTokenProviderService)
        {
            _connectionTokenProviderService = connectionTokenProviderService;
        }

        public async void FetchConnectionToken(IConnectionTokenCallback callback)
        {
            var isSuccess = false;

            do
            {
                try
                {
                    var token = await _connectionTokenProviderService.FetchConnectionToken();
                    isSuccess = true;
                    callback.OnSuccess(token);
                }
                catch (Throwable ex)
                {
                    if (_retries >= MaxRetries)
                    {
                        callback.OnFailure(new ConnectionTokenException("Failed to fetch connection token", ex));
                    }
                }
                catch (System.Exception ex)
                {
                    if (_retries >= MaxRetries)
                    {
                        callback.OnFailure(new ConnectionTokenException("Failed to fetch connection token", new Throwable(ex.Message)));
                    }
                }

                _retries++;

            } while (_retries < MaxRetries && isSuccess == false);
        }
    }
}
