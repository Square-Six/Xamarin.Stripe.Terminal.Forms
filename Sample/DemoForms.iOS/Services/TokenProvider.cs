using System;
using StripeTerminal;

namespace DemoForms.iOS.Services
{
    public class TokenProvider : SCPConnectionTokenProvider
    {
        public TokenProvider()
        {
        }

        public override void FetchConnectionToken(SCPConnectionTokenCompletionBlock completion)
        {
            // NOTE: Add your business logic here to retreive your Strip Token

            var token = "{YOUR_STRIPE_TOKEN}";

            completion(token, null);
        }
    }
}
