using System;
using System.Threading.Tasks;
using Xamarin.Stripe.Terminal.Forms;

namespace DemoForms.Services
{
    public class MyStripeTokenFetchService : IConnectionTokenProviderService
    {
        public MyStripeTokenFetchService()
        {
        }

        public async Task<string> FetchConnectionToken()
        {
            await Task.Delay(1000);

            var token = "{YOUR_STRIPE_TOKEN}";

            return token;
        }
    }
}
