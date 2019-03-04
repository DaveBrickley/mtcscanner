using System;
using System.Collections.Generic;
using Plugin.Connectivity;

using Xamarin.Forms;

namespace MTCScanner.Views
{

    public partial class AboutPage : ContentPage
    {
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


        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
