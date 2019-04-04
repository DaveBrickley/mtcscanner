using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using ZXing;
using HockeyApp.iOS;
using Xamarin.Forms;

using Foundation;
using UIKit;

using MTCScanner;

namespace MTCScanner.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            global::Xamarin.Forms.Forms.Init();

            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("d3d91fb65db345368a90b1eddeec7f94");
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation(); // This line is obsolete in crash only builds
            manager.DebugLogEnabled = true;

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();


            LoadApplication(new App());


            App.UiParent = new UIParent();
            return base.FinishedLaunching(application, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }
    }
}

