using System;
using Xamarin.Forms;
using FreshMvvm;
using DemoForms.Services;
using Xamarin.Stripe.Terminal.Forms;

namespace DemoForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Registers the service that retrevies your stripe token for Dependency injection
            FreshIOC.Container.Register<IConnectionTokenProviderService, MyStripeTokenFetchService>();

            // Set the main page
            var settingsPage = FreshPageModelResolver.ResolvePageModel<SettingsPageModel>();
            MainPage = new FreshNavigationContainer(settingsPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
