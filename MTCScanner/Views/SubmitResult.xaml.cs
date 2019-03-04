using System;
using System.Collections.Generic;
using MTCScanner.Authenticate;
using MTCScanner.Scanner;
using Plugin.Connectivity;
using System.Web;

using Xamarin.Forms;

namespace MTCScanner.Views
{
    public partial class SubmitResult : ContentPage
    {
        public SubmitResult()
        {
            InitializeComponent();

        }


        async public void SubmitCodeToCloud(string code)

        {


            //string encodedString = Base64Encode(code);

            System.Diagnostics.Debug.WriteLine("Debug: user is attempting to submit code of length " + code.Length + ":" + code);

            var submit = new CloudData();

            string userAddedNotes = TextNotes.Text;

            var response = await submit.PostBarcode(code, userAddedNotes);

            if (response == "OK")

            {

                var action = await DisplayAlert("Success", "Your unique code was successfully saved", "TRY AGAIN", "QUIT SCANNING");

                if (action)

                {
                    var scanPage = new FullScreenScanning();
                    // Navigate to our scanner page

                    App.Current.MainPage = scanPage;

                }

                else

                {

                    var About = new Views.AboutPage();

                    // Navigate to our scanner page

                    App.Current.MainPage = About;

                }

            }

            else

            {

                await DisplayAlert("Alert", "An error occurred, please scan and submit your QR code again", "OK");



            }


        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {

                await DisplayAlert("Network", "Please check you are connected to the network", "Ok");
            }

            else

            {

                SubmitCodeToCloud(TextID.Text);

            }

        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            var scanPage = new FullScreenScanning();
            // Navigate to our scanner page

            App.Current.MainPage = scanPage;
        }
    }
}
