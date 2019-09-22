using System;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace Mapi
{
    public interface IProcessingInteractor
    {
        int resultOfGettingPosition { get; set; }
        void UpdateCurrentLocation();
        void UpdateVisiblePolygons(Polygon MainCameraPolygon, Xamarin.Forms.GoogleMaps.Map map);
        void UpdateVisibleForUser(Polygon MainCameraPolygon, Xamarin.Forms.GoogleMaps.Map map, string username);
    }
}
