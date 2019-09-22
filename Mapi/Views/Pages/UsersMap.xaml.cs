using System;
using Xamarin.Forms.GoogleMaps;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mapi.Views
{
    #pragma warning disable CS4014
    public partial class UsersMap : ContentPage
    {
        static string CurrentUser;
        static Polygon CameraPolygon = new Polygon();

        public event EventHandler ItemTapped;

        private bool isAnimating = false;
        private const uint animationDelay = 300;

        public UsersMap(string username)
        {
            InitializeComponent();

            CurrentUser = username;
            Username.Text = username;

            InnerButtonClose.IsVisible = false;
            InnerButtonMenu.IsVisible = true;

            ConnectOpenWithClick();
            ConnectCloseWithClick();
            ConnectWithClick();

            // Choose style for map from list:
            //               styleGreyStandard
            //                  styleLightGrey
            //              styleLightColorful

            map.MapStyle = MapStyle.FromJson(DifferentStylesForMap.styleLightColorful);
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;

            map.CameraIdled += (sender, args) =>
            {
                try
                {
                    CheckForUpdates();
                }
                catch
                {
                    // Problems with network
                    // DisplayAlert("Error", "Problems with network", "CLose");
                }
            };

            try
            {
                CheckForUpdates();
            }
            catch
            {
                // Problems with network
            }
        }

        // MARK: - Work with geometry

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <returns><c>true</c>, if updates was checked, <c>false</c> otherwise.</returns>
        bool CheckForUpdates()
        {
            if (!App.finishLoading) {
                return false; 
                }
            // Checks if new polygons were visited
            try
            {
                App.processingInteractor.UpdateVisibleForUser(CameraPolygon, map, Username.Text);
            }
            catch 
            {
                // Cannot get location
            }
            return true;
        }

        // MARK: - Methods for work with menu

        /// <summary>
        /// Adds tap gesture recognizers for each item in menu.
        /// </summary>
        private void ConnectWithClick()
        {
            ConnectItemWithClick(Settings, "Settings");
            ConnectItemWithClick(Followings, "Followings");
            ConnectItemWithClick(UserMap, "Map");
        }

        /// <summary>
        /// Adds tap gesture recognizer to an image - specific menu item.
        /// </summary>
        /// <param name="image">Image - menu item.</param>
        /// <param name="value">Menu item description.</param>
        private void ConnectItemWithClick(Image image, string value)
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

