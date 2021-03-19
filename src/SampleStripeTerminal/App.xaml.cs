using System;
using SquareSix.Core;
using Xamarin.Forms;

namespace SampleStripeTerminal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SquareSixCore.Init();
            MainPage = new NavigationPage(new MainPage());
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
