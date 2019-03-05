using System;
using System.Collections.Generic;
using MTCScanner.Authenticate;
using Microsoft.Identity.Client;
using System.Web;

using Xamarin.Forms;

namespace MTCScanner.Scanner
{
    public partial class Scanner : ContentPage
    {

        public Scanner()
        {
            InitializeComponent();

            TextName.Text = App.Current.Properties["LoginName"].ToString();
            TextStand.Text = App.Current.Properties["StandName"].ToString();
            TextCompany.Text = App.Current.Properties["CompanyName"].ToString();


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

        async public void EditProfile()

        {

            var action = await DisplayAlert("Edit Profile", "Complete the fields on the next page with your registered Email address and Password to edit profile information like Stand ID or Company Name", "PROCEED", "GO BACK");

            if (action)

            {


                var x = new CloudData();
                AuthenticationResult token = await x.GetAccessToken("edit"); // Edit Profile

            }


        }

        public void Handle_Clicked_2(object sender, System.EventArgs e)
        {



            EditProfile();


        }



        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Debug.WriteLine("Debug: back button pressed");

            return true;
        }
    }
}
