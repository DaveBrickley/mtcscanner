using System;
using System.Collections.Generic;
using Plugin.Connectivity;
using Xamarin.Forms;
using System.Threading;

namespace MTCScanner.Views
{
    public partial class GDPR : ContentPage
    {
        public GDPR()
        {
            InitializeComponent();
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)

            {

                Application.Current.MainPage = new Authenticate.LoginPage();
            }

            else

            {


                await DisplayAlert("Network", "Please check your network connection", "OK");



            }
        }
    }
}
