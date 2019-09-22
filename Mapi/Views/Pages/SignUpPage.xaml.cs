using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mapi.Views
{
    public partial class SignUpPage : ContentPage
    {
        private bool authenticated = false;

        public SignUpPage()
        {
            InitializeComponent();
            Image image = (Image)this.FindByName("LoginIcon");
            image.WidthRequest = DeviceDisplay.MainDisplayInfo.Width;
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (App.Authenticator != null)
                {
                    object[] results = await App.Authenticator.AuthenticateAsync();
                    authenticated = (bool)results[0];
                    App.userId = (string)results[1];
                }

                if (authenticated)
                {
                    App.router.ChangePageTo(new MainMap());
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Authentication was cancelled"))
                {
                    messageLabel.Text = "Authentication cancelled by the user";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                messageLabel.Text = "Authentication failed";
            }
        }
    }
}
