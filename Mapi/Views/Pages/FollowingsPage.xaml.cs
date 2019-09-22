using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace Mapi.Views
{
    #pragma warning disable CS4014
    public partial class FollowingsPage : ContentPage
    {
        public List<ListItemEntity> users;

        public event EventHandler ItemTapped;

        private bool isAnimating = false;
        private uint animationDelay = 300;

        public FollowingsPage()
        {
            InitializeComponent();

            InnerButtonClose.IsVisible = false;
            InnerButtonMenu.IsVisible = true;

            ConnectOpenWithClick();
            ConnectCloseWithClick();
            ConnectWithClick();

            ShowFollowers(new object(), new EventArgs());
            this.BindingContext = this;

            PhotoContent.Source = App.mainUser.Img;
        }

        /// <summary>
        /// Shows the followers.
        /// </summary>
        /// <param name="sender">Sender button.</param>
        /// <param name="e">Some arguments.</param>
        async void ShowFollowers(object sender, EventArgs e)
        {
            FollowersButton.BackgroundColor = new Color(0.00f, 0.54f, 0.97f);
            FollowingsButton.BackgroundColor = new Color(0.78125f, 0.78125f, 0.78125f);
            await App.dataInteractor.GetUser();
            var followersFromCDB = App.dataInteractor.GetFollowers();
            users = new List<ListItemEntity>();
            if (followersFromCDB != null)
            {
                for (int i = 0; i < followersFromCDB.Length; ++i)
                {
                    var follower = App.dataInteractor.FindFollower(followersFromCDB[i]);
                    if (follower != null)
                        users.Add(follower);
                }
            }

            UsersList.ItemsSource = users;
        }

        /// <summary>
        /// Shows the followings.
        /// </summary>
        /// <param name="sender">Sender button.</param>
        /// <param name="e">Some arguments.</param>
        async void ShowFollowings(object sender, EventArgs e)
        {
            FollowingsButton.BackgroundColor = new Color(0.00f, 0.54f, 0.97f);
            FollowersButton.BackgroundColor = new Color(0.78125f, 0.78125f, 0.78125f);

            await App.dataInteractor.GetUser();
            var followingsFromCDB = App.dataInteractor.GetFollowings();
            users = new List<ListItemEntity>();
            if (followingsFromCDB != null)
            {
                for (int i = 0; i < followingsFromCDB.Length; ++i)
                {
                    var following = App.dataInteractor.FindFollowing(followingsFromCDB[i]);
                    if (following != null)
                        users.Add(following);
                }
            }

            UsersList.ItemsSource = users;
        }

        private void SearchForUsers(object sender, EventArgs args)
        {
            FollowersButton.BackgroundColor = new Color(0.78125f, 0.78125f, 0.78125f);
            FollowingsButton.BackgroundColor = new Color(0.78125f, 0.78125f, 0.78125f);

            if (SearchEntry.Text == "")
                return;

            users = new List<ListItemEntity>();
            var allUsersWithSameName = App.manager?.SearchUsersByName(SearchEntry.Text);

            if (allUsersWithSameName != null)
            {
                for (int i = 0; i < allUsersWithSameName.Count; ++i)
                {
                    users.Add(App.dataInteractor.ConvertUser(allUsersWithSameName[i]));
                }
            }

            UsersList.ItemsSource = users;
        }

        async void AddButtonClicked(object sender, System.EventArgs e)
        {
            Button button = (Button)sender;
            string anotherUserName = button.CommandParameter as String;

            button.IsEnabled = false;
            if (button.Text == "+")
            {
                button.Text = "-";
                await App.dataInteractor.AddFollowing(anotherUserName);
                await App.dataInteractor.AddFollower(anotherUserName, App.mainUser.Name);
            }
            else
            {
                button.Text = "+";
                await App.dataInteractor.DeleteFollowing(anotherUserName);
                await App.dataInteractor.DeleteFollower(anotherUserName, App.mainUser.Name);
            }
            ShowFollowings(null, null);
            button.IsEnabled = true;
        }

        private void Item_Clicked(object sender, System.EventArgs e)
        {
            ViewCell cell = (ViewCell)sender;
            App.router.ChangePageTo(new UsersMap(cell.BindingContext.ToString()));
        }

        /// <summary>
        /// Adds tap gesture recognizers for each item in menu.
        /// </summary>
        private void ConnectWithClick()
        {
            ConnectItemWithCLick(Settings, "Settings");
            ConnectItemWithCLick(Followings, "Followings");
            ConnectItemWithCLick(UserMap, "Map");
            // ConnectUserPhotoWithCLick(PhotoContent, "PhotoContent");
        }

        /// <summary>
        /// Adds tap gesture recognizer to an image - specific menu item.
        /// </summary>
        /// <param name="image">Image - menu item.</param>
        /// <param name="value">Menu item description.</param>
        private void ConnectItemWithCLick(Image image, string value)
        {
            image.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    ItemTapped?.Invoke(this, new SelectedItemChangedEventArgs(value));

                    await CloseMenu();

                    if (value == "Settings")
                        App.router.ChangePageTo(new Views.SettingsPage());
                    else if (value == "Followings")
                        App.router.ChangePageTo(new Views.FollowingsPage());
                    else if (value == "Map")
                        App.router.ChangePageTo(new Views.MainMap());
                }),
                NumberOfTapsRequired = 1
            });
        }

        /// <summary>
        /// Adds tap gesture recognizer to close button in the middle of round menu.
        /// </summary>
        private void ConnectCloseWithClick()
        {
            InnerButtonClose.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await CloseMenu();
                }),
                NumberOfTapsRequired = 1
            });
        }

        /// <summary>
        /// Adds tap gesture recognizer to open button in the middle of round menu.
        /// </summary>
        private void ConnectOpenWithClick()
        {
            InnerButtonMenu.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (!isAnimating)
                    {
                        isAnimating = true;

                        InnerButtonClose.IsVisible = true;
                        InnerButtonMenu.IsVisible = true;

                        InnerButtonMenu.RotateTo(360, animationDelay);
                        InnerButtonMenu.FadeTo(0, animationDelay);
                        InnerButtonClose.RotateTo(360, animationDelay);
                        InnerButtonClose.FadeTo(1, animationDelay);

                        await OuterCircle.ScaleTo(3.3, 1000, Easing.BounceIn);
                        await ShowMenuItems();
                        InnerButtonMenu.IsVisible = false;

                        isAnimating = false;

                    }
                }),
                NumberOfTapsRequired = 1
            });
        }

        /// <summary>
        /// Closes the menu.
        /// </summary>
        private async Task CloseMenu()
        {
            if (!isAnimating)
            {

                isAnimating = true;

                InnerButtonMenu.IsVisible = true;
                InnerButtonClose.IsVisible = true;
                await HideMenuItems();

                InnerButtonClose.RotateTo(0, animationDelay);
                InnerButtonClose.FadeTo(0, animationDelay);
                InnerButtonMenu.RotateTo(0, animationDelay);
                InnerButtonMenu.FadeTo(1, animationDelay);
                await OuterCircle.ScaleTo(1, 400, Easing.BounceOut);
                InnerButtonClose.IsVisible = false;

                isAnimating = false;
            }
        }

        /// <summary>
        /// Hides the menu items.
        /// </summary>
        private async Task HideMenuItems()
        {
            var speed = 25U;
            await Settings.FadeTo(0, speed);
            await Followings.FadeTo(0, speed);
            await UserMap.FadeTo(0, speed);
        }

        /// <summary>
        /// Shows the menu items.
        /// </summary>
        private async Task ShowMenuItems()
        {
            var speed = 25U;
            await Settings.FadeTo(1, speed);
            await Followings.FadeTo(1, speed);
            await UserMap.FadeTo(1, speed);
        }
    }
} 