using System;
using Xamarin.Forms;
using FreshMvvm;

namespace DemoForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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
