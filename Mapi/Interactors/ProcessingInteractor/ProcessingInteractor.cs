using System;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;

namespace Mapi
{
    public class ProcessingInteractor: IProcessingInteractor
    {
        const double zoomMinimumValue = 6.8;
        public static Location currentLocation = new Location();
        public int resultOfGettingPosition { get; set; } = 0;

        IGeometryInteractor geometryInteractor => App.geometryInteractor;

        /// <summary>
        /// Compares two position by Y coordinate.
        /// </summary>
        /// <returns><c>true</c>, if left position smaller than right by y, <c>false</c> otherwise.</returns>
        /// <param name="left">First position.</param>
        /// <param name="right">Second position.</param>
        bool CompareByY(Position left, Position right)
        {
            if (right.Latitude > left.Latitude)
                return true;
            return false;
        }

        /// <summary>
        /// Compares two position by X coordinate.
        /// </summary>
        /// <returns><c>true</c>, if left position smaller than right by x, <c>false</c> otherwise.</returns>
        /// <param name="left">First position.</param>
        /// <param name="right">Second position.</param>
        bool CompareByX(Position left, Position right)
        {
            if (right.Longitude > left.Longitude)
                return true;
            return false;
        }

        /// <summary>
        /// Updates the current location.
        /// </summary>
        public async void UpdateCurrentLocation()
        {
            try
            {
                currentLocation = await GetCurrentLocation();
                resultOfGettingPosition = 0;
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                resultOfGettingPosition = 1;
            }
            catch (PermissionException)
            {
                // Handle permission exception
                resultOfGettingPosition = 2;
            }
            catch (Exception)
            {
                // Unable to get location
                resultOfGettingPosition = 3;
            }
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>The current location.</returns>
        async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var request1 = new GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Lowest, TimeSpan.FromSeconds(10));

                var location = await Geolocation.GetLastKnownLocationAsync();
                var location1 = await Geolocation.GetLocationAsync(request);
                var location2 = await Geolocation.GetLocationAsync(request1);

                if (location1 != null)
                    return location1;
                else if (location2 != null)
                    return location2;
                return location;
            }
            catch (FeatureNotSupportedException featureNotSupportedException)
            {
                // Console.WriteLine ("throw1");
                // Handle not supported on device exception
                throw featureNotSupportedException;
            }
            catch (PermissionException permissionException)
            {
                // Console.WriteLine ("throw2");
                // Handle permission exception
                throw permissionException;
            }
            catch (Exception exception)
            {
                // Console.WriteLine ("throw3");
                // Unable to get location
                throw exception;
            }
        }

        /// <summary>
        /// Updates the visible polygons.
        /// </summary>
        /// <param name="MainCameraPolygon">Main camera polygon.</param>
        /// <param name="map">Map.</param>
        public void UpdateVisiblePolygons(Polygon MainCameraPolygon, Xamarin.Forms.GoogleMaps.Map map)
        {
            // Updating MainCameraPolygon positions
            MainCameraPolygon.Positions.Clear();
            MainCameraPolygon.Positions.Add(map.Region.FarLeft);
            MainCameraPolygon.Positions.Add(map.Region.FarRight);
            MainCameraPolygon.Positions.Add(map.Region.NearRight);
            MainCameraPolygon.Positions.Add(map.Region.NearLeft);

            // Clearing map before adding new
            map.Polygons.Clear();

            // Binary search for lower position (Y)
            int left = 0, right = GeometryEntity.polygons.Length - 1;

            Position MainCameraPosition;
            if (MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[1].Latitude
            && MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[1].Latitude &&
                MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByY(GeometryEntity.polygons[middle][0].Positions[3], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int TopBorder = left;

            // Binary search for upper position (Y)
            left = 0;
            right = GeometryEntity.polygons.Length - 1;

            if (MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[1].Latitude
                && MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[2].Latitude &&
                    MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[1].Latitude &&
                MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByY(GeometryEntity.polygons[middle][0].Positions[0], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int BottomBorder = left;

            // Binary search for lower position (X)
            left = 0;
            right = GeometryEntity.polygons[0].Length - 1;

            if (MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[1].Longitude
                && MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[2].Longitude &&
                    MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[1].Longitude &&
                MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByX(GeometryEntity.polygons[0][middle].Positions[1], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int LeftBorder = left;

            // Binary search for upper position (X)
            left = 0;
            right = GeometryEntity.polygons[0].Length - 1;

            if (MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[1].Longitude
            && MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[1].Longitude &&
                MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByX(GeometryEntity.polygons[0][middle].Positions[0], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int RightBorder = left;

            // Feature for big zoom
            if (map.CameraPosition.Zoom < zoomMinimumValue)
            {
                MainCameraPolygon.StrokeColor = geometryInteractor.defaultStrokeColor;
                MainCameraPolygon.StrokeWidth = 1f;
                MainCameraPolygon.FillColor = geometryInteractor.defaultFillColor;
                map.Polygons.Add(MainCameraPolygon);
            }

            const int delta = 1;
            for (int i = Math.Min(TopBorder, BottomBorder) - delta; i <= Math.Min(Math.Max(TopBorder, BottomBorder) + delta, GeometryEntity.polygons.Length - 1); ++i)
                for (int j = Math.Min(LeftBorder, RightBorder) - delta; j <= Math.Min(delta + Math.Max(LeftBorder, RightBorder), GeometryEntity.polygons[i].Length - 1); ++j)
                {
                    try
                    {
                        if (map.CameraPosition.Zoom >= zoomMinimumValue)
                        {
                            map.Polygons.Add(GeometryEntity.polygons[i][j]);
                            // Geometry.polygons[i][j].FillColor = Geometry.DefaultFillColor;
                        }

                        string infoAboutCurrentPolygon = $"{i}+{j}";
                        if (App.dataInteractor.CheckPolygon(infoAboutCurrentPolygon))
                        {
                            GeometryEntity.polygons[i][j].FillColor = geometryInteractor.ColorForVisitedPolygon;
                            if (map.CameraPosition.Zoom < zoomMinimumValue)
                                map.Polygons.Add(GeometryEntity.polygons[i][j]);
                        }
                        else if (geometryInteractor.IsInPolygon(GeometryEntity.polygons[i][j], currentLocation))
                        {
                            App.dataInteractor.AddNewPolygon(infoAboutCurrentPolygon);
                            GeometryEntity.polygons[i][j].FillColor = geometryInteractor.ColorForVisitedPolygon;
                            if (map.CameraPosition.Zoom < zoomMinimumValue)
                                map.Polygons.Add(GeometryEntity.polygons[i][j]);
                        }
                    }
                    catch 
                    {
                        // Google map not available
                    }
                }
        }

        /// <summary>
        /// Updates the visible polygons for user.
        /// </summary>
        /// <param name="MainCameraPolygon">Main camera polygon.</param>
        /// <param name="map">Map.</param>
        /// <param name="username">Username.</param>
        public void UpdateVisibleForUser(Polygon MainCameraPolygon, Xamarin.Forms.GoogleMaps.Map map, string username)
        {
            // Updating MainCameraPolygon positions
            MainCameraPolygon.Positions.Clear();
            MainCameraPolygon.Positions.Add(map.Region.FarLeft);
            MainCameraPolygon.Positions.Add(map.Region.FarRight);
            MainCameraPolygon.Positions.Add(map.Region.NearRight);
            MainCameraPolygon.Positions.Add(map.Region.NearLeft);

            // Clearing map before adding new
            map.Polygons.Clear();
            try
            {
                geometryInteractor.ResetAllPolygons();
            }
            catch
            {
                // Polygons are not ready
            }

            // Binary search for lower position (Y)
            int left = 0, right = GeometryEntity.polygons.Length - 1;

            Position MainCameraPosition;
            if (MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[1].Latitude
            && MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[0].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[1].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[1].Latitude &&
                MainCameraPolygon.Positions[2].Latitude > MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByY(GeometryEntity.polygons[middle][0].Positions[3], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int TopBorder = left;

            // Binary search for upper position (Y)
            left = 0;
            right = GeometryEntity.polygons.Length - 1;

            if (MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[1].Latitude
                && MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[2].Latitude &&
                    MainCameraPolygon.Positions[0].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[2].Latitude &&
                MainCameraPolygon.Positions[1].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[0].Latitude
            && MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[1].Latitude &&
                MainCameraPolygon.Positions[2].Latitude < MainCameraPolygon.Positions[3].Latitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByY(GeometryEntity.polygons[middle][0].Positions[0], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int BottomBorder = left;

            // Binary search for lower position (X)
            left = 0;
            right = GeometryEntity.polygons[0].Length - 1;

            if (MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[1].Longitude
                && MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[2].Longitude &&
                    MainCameraPolygon.Positions[0].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[1].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[1].Longitude &&
                MainCameraPolygon.Positions[2].Longitude > MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByX(GeometryEntity.polygons[0][middle].Positions[1], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int LeftBorder = left;

            // Binary search for upper position (X)
            left = 0;
            right = GeometryEntity.polygons[0].Length - 1;

            if (MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[1].Longitude
            && MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[0].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[0];
            }
            else if (MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[2].Longitude &&
                MainCameraPolygon.Positions[1].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[1];
            }
            else if (MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[0].Longitude
            && MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[1].Longitude &&
                MainCameraPolygon.Positions[2].Longitude < MainCameraPolygon.Positions[3].Longitude)
            {
                MainCameraPosition = MainCameraPolygon.Positions[2];
            }
            else
            {
                MainCameraPosition = MainCameraPolygon.Positions[3];
            }

            while (right - left > 1)
            {
                int middle = (right + left) / 2;

                if (CompareByX(GeometryEntity.polygons[0][middle].Positions[0], MainCameraPosition))
                    left = middle;
                else
                    right = middle;
            }
            int RightBorder = left;

            // Feature for big zoom
            if (map.CameraPosition.Zoom < zoomMinimumValue)
            {
                MainCameraPolygon.StrokeColor = geometryInteractor.defaultStrokeColor;
                MainCameraPolygon.StrokeWidth = 1f;
                MainCameraPolygon.FillColor = geometryInteractor.defaultFillColor;
                map.Polygons.Add(MainCameraPolygon);
            }

            const int delta = 1;
            for (int i = Math.Min(TopBorder, BottomBorder) - delta; i <= Math.Min(Math.Max(TopBorder, BottomBorder) + delta, GeometryEntity.polygons.Length - 1); ++i)
                for (int j = Math.Min(LeftBorder, RightBorder) - delta; j <= Math.Min(delta + Math.Max(LeftBorder, RightBorder), GeometryEntity.polygons[i].Length - 1); ++j)
                {
                    try
                    {
                        if (map.CameraPosition.Zoom >= zoomMinimumValue)
                            map.Polygons.Add(GeometryEntity.polygons[i][j]);

                        string infoAboutCurrentPolygon = $"{i}+{j}";
                        if (App.dataInteractor.CheckPolygonForUser(username, infoAboutCurrentPolygon))
                        {
                            GeometryEntity.polygons[i][j].FillColor = geometryInteractor.ColorForVisitedPolygon;
                            if (map.CameraPosition.Zoom < zoomMinimumValue)
                                map.Polygons.Add(GeometryEntity.polygons[i][j]);
                        }
                        else
                        {
                            GeometryEntity.polygons[i][j].FillColor = geometryInteractor.defaultFillColor;
                        }
                    }
                    catch
                    {
                       // Google map not available
                    }
                }
        }
    }
}
