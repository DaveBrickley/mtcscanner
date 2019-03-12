using System;
using System.Collections.Generic;
using MTCScanner.Authenticate;
using MTCScanner.Models;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MTCScanner.Views
{
    public partial class Password : ContentPage
    {
        public Password()
        {
            InitializeComponent();
        }


        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            var x = new CloudData();

            GraphUser y = await x.GetDisplayName();


            Task<string> access_id = x.GetAccessID();

            System.Diagnostics.Debug.WriteLine("Debug: get access ID " + access_id.Result);

            //await DisplayAlert("Alert", "Welcome, " + y, "OK");

            var displayName = new Item
            {

                Name = "Logged in as: " + y.displayName,
                Company = "Company: " + y.Company,
                Stand = "Stand number: " + y.StandID



            };

            App.Current.Properties["LoginName"] = "Logged in as: " + y.displayName;
            App.Current.Properties["StandName"] = "Stand ID: " + y.StandID;
            App.Current.Properties["CompanyName"] = "Company: " + y.Company;



            await App.Current.SavePropertiesAsync();

            var Scanner = new Scanner.Scanner();

            Scanner.BindingContext = displayName;


            Application.Current.MainPage = Scanner;


        }
    }
}
