using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mapi.Views
{
#pragma warning disable CS4014
    public partial class SettingsPage : ContentPage
    {
        public event EventHandler ItemTapped;

        private bool isAnimating = false;
        private uint animationDelay = 300;

        public SettingsPage()
        {
            InitializeComponent();
            InnerButtonClose.IsVisible = false;
            InnerButtonMenu.IsVisible = true;

            ConnectOpenWithClick();
            ConnectCloseWithClick();
            ConnectWithClick();

            if (App.geometryInteractor.ColorForVisitedPolygon == Color.FromRgba(54, 138, 239, 100))
            {
                Blue.ScaleTo(1.3, 1);
            }
            else if (App.geometryInteractor.ColorForVisitedPolygon == Color.FromRgba(255, 214, 0, 100))
            {
                Yellow.ScaleTo(1.3, 1);
            }
            else if (App.geometryInteractor.ColorForVisitedPolygon == Color.FromRgba(255, 23, 68, 100))
            {
                Red.ScaleTo(1.3, 1);
            }
            else if (App.geometryInteractor.ColorForVisitedPolygon == Color.FromRgba(0, 200, 83, 100))
            {
                Green.ScaleTo(1.3, 1);
            }
        }
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

        async void ColorButtonClicked(object sender, System.EventArgs e)
        {
            Blue.ScaleTo(1, 1);
            Yellow.ScaleTo(1, 1);
            Red.ScaleTo(1, 1);
            Green.ScaleTo(1, 1);

            Blue.WidthRequest = 80;
            Blue.HeightRequest = 80;
            Yellow.WidthRequest = 80;
            Yellow.HeightRequest = 80;
            Red.WidthRequest = 80;
            Red.HeightRequest = 80;
            Green.WidthRequest = 80;
            Green.HeightRequest = 80;

            Button colorButton = (Button)sender;
            await colorButton.ScaleTo(1.3, 500);

            if (colorButton.Text == "Blue")
            {
                App.geometryInteractor.ColorForVisitedPolygon = Color.FromRgba(54, 138, 239, 100);
            }
            else if (colorButton.Text == "Yellow")
            {
                App.geometryInteractor.ColorForVisitedPolygon = Color.FromRgba(255, 214, 0, 100);
            }
            else if (colorButton.Text == "Red")
            {
                App.geometryInteractor.ColorForVisitedPolygon = Color.FromRgba(255, 23, 68, 100);
            }
            else if (colorButton.Text == "Green")
            {
                App.geometryInteractor.ColorForVisitedPolygon = Color.FromRgba(0, 200, 83, 100);
            }
        }
    }
}
