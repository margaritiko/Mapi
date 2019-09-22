using System;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;

namespace Mapi
{
    public interface ILineInteractor
    {
        double ReturnArea(Position A, Position B, Position C);
        bool CheckIntersect1(double a, double b, double c, double d);
        bool IfIntersect(LineEntity Left, LineEntity Right);
    }
}
