using System;
using System.Collections.Generic;
using MTCScanner.Authenticate;
using Microsoft.Identity.Client;
using System.Web;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MTCScanner.Scanner
{
    public partial class Scanner : ContentPage
    {

        public Scanner()
        {
            InitializeComponent();





        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            var ReloadProfile = new CloudData();

            await ReloadProfile.GetDisplayName();

            TextName.Text = "Logged in as: " + App.Current.Properties["displayName"].ToString();
            TextStand.Text = "Stand ID: " + App.Current.Properties["StandID"].ToString();
            TextCompany.Text = "Company: " + App.Current.Properties["Company"].ToString();

            System.Diagnostics.Debug.WriteLine("Debug: Scanner page is loaded again");


        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {

            var checkPasswordIsValid = new CloudData();
            AuthenticationResult token = await checkPasswordIsValid.GetAccessToken("");


            string UserConferencePassword = App.Current.Properties["Password"].ToString();

            string PasswordFromAPI = checkPasswordIsValid.GetAccessID().Result;

            System.Diagnostics.Debug.WriteLine("Debug: Password must be " + PasswordFromAPI);

            if (PasswordFromAPI != "disable_conference_scanner_password")

            {
                if (UserConferencePassword != PasswordFromAPI)

                {

                    await DisplayAlert("Alert", "The password in your profile does not match the current Conference Password. Please contact Microlise to obtain the latest one, edit your profile, then try again", "OK");

                    return;
                }

            }

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

            var action = await DisplayAlert("Edit Profile", "The following page will enable you to edit profile information like Stand ID or Company Name", "PROCEED", "GO BACK");

            if (action)

            {


                var x = new CloudData();
                AuthenticationResult token = await x.GetAccessToken("edit"); // Edit Profile



                await DisplayAlert("Message", "If you have made changes to your profile, please note it may take up to 30 seconds for them to be reflected in the app.\n\nIf you have changed conference password, please log out and in again.", "OK");


                TextName.Text = "";
                TextStand.Text = "";
                TextCompany.Text = "";

                var ReloadPage = new LoginPage();

                App.Current.MainPage = ReloadPage;




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
