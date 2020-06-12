using System;
using System.Threading.Tasks;

namespace Xamarin.Stripe.Terminal.Forms
{
    public interface IConnectionTokenProviderService
    {
        Task<string> FetchConnectionToken();
    }
}
