using System;
using System.Collections.Generic;
using MTCScanner.Authenticate;
using Microsoft.Identity.Client;


using Xamarin.Forms;

namespace MTCScanner.Scanner
{
    public partial class Scanner : ContentPage
    {

        public Scanner()
        {
            InitializeComponent();
        }


        void Handle_Clicked(object sender, System.EventArgs e)
        {


            var scanPage = new FullScreenScanning();
            var resultsPage = new Views.SubmitResult();
            // Navigate to our scanner page
            //await Navigation.PushModalAsync(scanPage);

            if (App.IsEmuTest == false)

            {

                Application.Current.MainPage = scanPage;

            }

            else

            {
                var barCode = new Models.Item
                {

                    Id = "12345678910",
                    Text = "Please click the button below to submit your scanned code to the Microlise database"


                };

                resultsPage.BindingContext = barCode;

                Application.Current.MainPage = resultsPage;

            }

            //NavigationPage.SetHasBackButton(this, false);

        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            var logmeout = new Authenticate.CloudData();

            logmeout.Logout();

            var home = new Views.AboutPage();

            App.Current.MainPage = home;



        }

        async void Handle_Clicked_2(object sender, System.EventArgs e)
        {



            var x = new CloudData();
            AuthenticationResult token = await x.GetAccessToken("edit"); // Edit Profile

            //Application.Current.MainPage = Scanner;





        }



        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Debug.WriteLine("Debug: back button pressed");

            return true;
        }
    }
}
