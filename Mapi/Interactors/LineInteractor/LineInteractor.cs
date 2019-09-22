using System;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;

namespace Mapi
{
    public class LineInteractor: ILineInteractor
    {
        /// <summary>
        /// Swap the specified x and y.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        private void Swap(ref double x, ref double y)
        {
            double tmp = x;
            x = y;
            y = tmp;
        }

        /// <summary>
        /// Returns the area of triangle by three points.
        /// </summary>
        /// <returns>The area.</returns>
        /// <param name="A">A.</param>
        /// <param name="B">B.</param>
        /// <param name="C">C.</param>
        public double ReturnArea(Position A, Position B, Position C)
        {
            return (B.Longitude - A.Longitude) * (C.Latitude - A.Latitude) - (B.Latitude - A.Latitude) * (C.Longitude - A.Longitude);
        }

        /// <summary>
        /// Checks the intersect of two lines, each presents as two points.
        /// </summary>
        /// <returns><c>true</c>, if lines intersect, <c>false</c> otherwise.</returns>
        /// <param name="a">The first line first coordinate.</param>
        /// <param name="b">The first line second coordinate.</param>
        /// <param name="c">The second line first coordinate.</param>
        /// <param name="d">The second line second coordinate.</param>
        public bool CheckIntersect1(double a, double b, double c, double d)
        {
            if (a > b)
                Swap(ref a, ref b);
            if (c > d)
                Swap(ref c, ref d);
            return Math.Max(a, c) <= Math.Min(b, d);
        }

        /// <summary>
        /// Checks the intersect of two lines.
        /// </summary>
        /// <returns><c>true</c>, if intersect was ifed, <c>false</c> otherwise.</returns>
        /// <param name="Left">First line to check.</param>
        /// <param name="Right">Second line to check.</param>
        public bool IfIntersect(LineEntity Left, LineEntity Right)
        {

            Position A = Left.X;
            Position B = Left.Y;
            Position C = Right.X;
            Position D = Right.Y;

            return CheckIntersect1(A.Longitude, B.Longitude, C.Longitude, D.Longitude) &&
                CheckIntersect1(A.Latitude, B.Latitude, C.Latitude, D.Latitude) &&
                ReturnArea(A, B, C) * ReturnArea(A, B, D) <= 0 &&
                ReturnArea(C, D, A) * ReturnArea(C, D, B) <= 0;
           
        }
    }
}
