using System;
using Xamarin.Forms;

namespace Mapi
{
    public class Router: IRouter
    {
        public void ChangePageTo(ContentPage page)
        {
            try
            {
                App.geometryInteractor.ResetAllPolygons();
            }
            catch
            {
                // Polygons are not ready
            }
            Application.Current.MainPage = page;
        }
    }
}
