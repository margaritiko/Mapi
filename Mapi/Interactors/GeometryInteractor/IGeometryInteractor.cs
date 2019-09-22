using System;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace Mapi
{
    public interface IGeometryInteractor
    {
        // Property
        Color ColorForVisitedPolygon { get; set; }
        Color defaultFillColor { get; }
        Color defaultStrokeColor { get; }

        // Methods
        void InitializePolygonsArray();
        Polygon CreatePolygonFromData(double[] Points);
        Polygon GetPolygon(string coordinates);
        bool IsInPolygon(Polygon polygon, Location CurrentPosition);
        void ResetAllPolygons();
    }
}
