using System;
using System.Collections.Generic;
using Plugin.Connectivity;
using System.Windows.Input;

using Xamarin.Forms;

namespace MTCScanner.Views
{

    public partial class AboutPage : ContentPage
    {
        async void Handle_Clicked(object sender, System.EventArgs e)
        {

            if (CrossConnectivity.Current.IsConnected)

            {

                Application.Current.MainPage = new Views.GDPR();
            }

            else

            {


                await DisplayAlert("Network", "Please check your network connection", "OK");



            }


        }



        protected void OpenURL(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://microliseconference.com"));
        }

        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
