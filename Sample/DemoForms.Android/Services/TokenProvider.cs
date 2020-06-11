using System;
using Com.Stripe.Stripeterminal.Callable;

namespace DemoForms.Droid.Services
{
    public class TokenProvider : Java.Lang.Object, IConnectionTokenProvider
    {
        public TokenProvider()
        {

        }

        void IConnectionTokenProvider.FetchConnectionToken(IConnectionTokenCallback callback)
        {
                        // NOTE: Add your business logic here to retreive your Strip Token

            var token = "{YOUR_STRIPE_TOKEN}";

            callback.OnSuccess(token);
        }
    }
}
