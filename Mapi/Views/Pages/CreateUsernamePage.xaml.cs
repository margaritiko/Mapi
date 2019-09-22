using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Plugin.Permissions;
#pragma warning disable
namespace Mapi.Views
{
    public partial class CreateUsernamePage : ContentPage
    {
        public CreateUsernamePage()
        {
            InitializeComponent();
        }

        async void OnOKButtonClicked(object sender, EventArgs e)
        {
            if (Username.Text == null)
                return;
            if (Username.Text.Length < 2)
            {
                DisplayAlert("Error", "Username is too short :c\n Must contain more than one character", "Try again");
                return;
            }
            else if (App.manager.SearchForCollision(Username.Text))
            {
                DisplayAlert("Error", "This username already exists :c", "Try again");
                return;
            }
            else if (Username.Text.Contains(" "))
            {
                DisplayAlert("Error", "Username cannot contain whitespaces :c", "Try again");
                return;
            }
            // Adding new user if there is no one with current token in table
            await App.dataInteractor.AddUserToDBWithName(Username.Text);
            App.router.ChangePageTo(new MainMap());
        }
    }
}
