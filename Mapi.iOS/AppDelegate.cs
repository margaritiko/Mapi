using Foundation;
using UIKit;
using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Permissions;

namespace Mapi.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        private MobileServiceUser user;

        static async void CheckForPermission()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            CheckForPermission();
            global::Xamarin.Forms.Forms.Init();
            // Mapi account key 
            Xamarin.FormsGoogleMaps.Init("YOUR_GOOGLE_MAPS_KEY");

            CurrentPlatform.Init();
            App.Init(this);
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public async Task<object[]> AuthenticateAsync()
        {
            bool success = false;
            try
            {
                if (user == null)
                {
                    // The authentication provider could also be Facebook, Twitter, or Microsoft
                    user = await App.authenticateInteractor.DefaultManager.CurrentClient.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.Google, Constants.URLScheme);
                }
                success = true;
            }
            catch (Exception ex)
            {
                var authAlert = UIAlertController.Create("Authentication failed", ex.Message, UIAlertControllerStyle.Alert);
                authAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(authAlert, true, null);
            }
            object[] array = { success, user.UserId ?? "" };
            return array;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return App.authenticateInteractor.DefaultManager.CurrentClient.ResumeWithURL(url);
        }
    }
}
