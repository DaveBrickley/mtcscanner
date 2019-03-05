using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Identity.Client;
using ZXing.Mobile;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MTCScanner
{
    public partial class App : Application
    {

        public static UIParent UiParent = null;

        public static Boolean IsEmuTest = false;

        public App()
        {
            InitializeComponent();

            Properties["UserAccessToken"] = "";
            Properties["displayName"] = "";
            Properties["uniqueUserID"] = "";
            Properties["mail"] = "";
            Properties["StandID"] = "";
            Properties["Company"] = "";

            Properties["LoginName"] = "";
            Properties["StandName"] = "";
            Properties["CompanyName"] = "";




            Logger.LogCallback = Log;
            Logger.Level = LogLevel.Info;
            Logger.PiiLoggingEnabled = true;

            MainPage = new Views.AboutPage();
        }

        private static void Log(LogLevel level, string message, bool containsPii)
        {
            if (containsPii)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            System.Diagnostics.Debug.WriteLine($"{level} {message}");
            Console.ResetColor();
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
