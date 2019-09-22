using System;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace Mapi
{
    public class GeometryInteractor: IGeometryInteractor
    {
        //TODO
        // Singleton pattern
        public IGeometryInteractor Interactor => App.geometryInteractor;

        ILineInteractor lineInteractor => App.lineInteractor;

        // Variables for getting random numbers.
        const int SEED = 500;
        static Random random = new Random(SEED);

        // Constants for colors.
        public Color defaultFillColor => Color.FromRgba(211, 211, 211, 150);
        public Color defaultStrokeColor => Color.FromRgba(255, 255, 255, 150);
        /// <summary>
        /// Gets or sets the color for visited polygon and save it in Application.Current.Properties.
        /// </summary>
        /// <value>The color for visited polygon.</value>
        public Color ColorForVisitedPolygon
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("colorForVisitedPolygons"))
                {
                    Application.Current.Properties["colorForVisitedPolygons"] = ConvertColorToString(Color.FromRgba(54, 138, 239, 100));
                    Application.Current.SavePropertiesAsync();
                }

                return ConvertStringToColor((String)Application.Current.Properties["colorForVisitedPolygons"]);
            }

            set
            {
                Application.Current.Properties["colorForVisitedPolygons"] = ConvertColorToString(value);
                Application.Current.SavePropertiesAsync();
            }
        }

        /// <summary>
        /// Converts color into string.
        /// </summary>
        /// <returns>The result color as string.</returns>
        /// <param name="color">Color which is using for getting string.</param>
        static string ConvertColorToString(Color color)
        {
            return $"{color.R}+{color.G}+{color.B}+{color.A}";
        }

        /// <summary>
        /// Converts string data into color.
        /// </summary>
        /// <returns>The result color.</returns>
        /// <param name="color">String that describes color.</param>
        static Color ConvertStringToColor(string color)
        {
            char[] separators = new char[] { '+' };
            var values = color.Split(separators);
            double r = 0, g = 0, b = 0, a = 0;
            double.TryParse(values[0], out r);
            double.TryParse(values[1], out g);
            double.TryParse(values[2], out b);
            double.TryParse(values[3], out a);
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Calculates the random integer between left and right.
        /// </summary>
        /// <returns>The random integer.</returns>
        /// <param name="left">Left border.</param>
        /// <param name="right">Right border.</param>
        static int GetRandom(int left, int right)
        {
            int add = right - left;
            int rnd = random.Next() % add;
            return left + rnd;
        }

        /// <summary>
        /// Converts the point into string.
        /// </summary>
        /// <returns>String from given point.</returns>
        /// <param name="point">Point to convert.</param>
        static string ConvertPointToString(Tuple<int, int> point)
        {
            int x = point.Item1;
            int y = point.Item2;

            string result = "";
            if (x / 100 != 0 || x > 0)
                result += $"{x / 100}.{Math.Abs(x % 100)} ";
            else
                result += $"-0.{Math.Abs(x % 100)} ";

            if (y / 100 != 0 || y > 0)
                result += $"{y / 100}.{Math.Abs(y % 100)} ";
            else
                result += $"-0.{Math.Abs(y % 100)} ";

            return result;
        }

        /// <summary>
        /// Converts each string with coordinates to a polygon.
        /// </summary>
        public void InitializePolygonsArray()
        {
            // Split a rectangle into small (random).
            // -90 -180           -90 180
            //             ...
            //  90 -180            90 180

            Tuple<int, int>[,] grid = new Tuple<int, int>[400, 400];
            // Coordinates are fractional numbers with an accuracy of two decimal places.
            // To simplify, multiply by 100 and work with integers.

            // Fill extremes
            for (int i = 0, LAT = -90; i <= 360; i += 2, LAT++)
            {
                for (int j = 0, LNG = -180; j <= 360; ++j, ++LNG)
                {
                    grid[i, j] = new Tuple<int, int>(LAT * 100, LNG * 100);
                }
            }

            // Filling in intermediate values
            // lu       ru
            //     ...
            // ld       rd
            for (int i = 1; i < 360; i += 2)
                for (int j = 0; j < 360; j++)
                {
                    Tuple<int, int> lu = grid[i - 1, j];
                    Tuple<int, int> ru = grid[i - 1, j + 1];
                    Tuple<int, int> ld = grid[i + 1, j];
                    Tuple<int, int> rd = grid[i + 1, j + 1];

                    int lAdd = (ld.Item1 - lu.Item1) / 3;
                    int rAdd = (ru.Item2 - lu.Item2) / 3;

                    Tuple<int, int> nxt = new Tuple<int, int>(GetRandom(lu.Item1 + lAdd, ld.Item1 - lAdd), GetRandom(lu.Item2 + rAdd, ru.Item2 - rAdd));
                    grid[i, j] = nxt;
                }

            List<string> coordinates = new List<string>();
            int counter = 0;

            for (int i = 2; i < 360; ++i)
            {
                for (int j = 0; j < 359; ++j)
                {
                    string polygon = ConvertPointToString(grid[i, j]);
                    polygon += ConvertPointToString(grid[i, j + 1]);
                    polygon += ConvertPointToString(grid[i - 1, j + 1]);
                    polygon += ConvertPointToString(grid[i - 1, j]);
                    coordinates.Add(polygon);

                    counter++;
                }
            }

            int position = -1;
            int index = 359;

            foreach (string data in coordinates)
            {
                Polygon polygon = GetPolygon(data);
                if (index == 359)
                {
                    position++;
                    index = 0;
                    GeometryEntity.polygons[position] = new Polygon[359];
                }

                GeometryEntity.polygons[position][index] = polygon;
                index++;
            }

            App.finishLoading = true;
        }

        /// <summary>
        /// For converting from automatically generated string into array of double numbers.
        /// </summary>
        /// <returns>Array of double numbers - list with coordinates.</returns>
        /// <param name="StringToConvert">String with coordinates to convert.</param>
        private static double[] ConvertStringToArray(ref string StringToConvert)
        {
            var IntermediateResult = StringToConvert.Split(' ');
            double[] Result = new double[IntermediateResult.Length];

            for (int position = 0; position < IntermediateResult.Length; position++)
                double.TryParse(IntermediateResult[position], out Result[position]);

            return Result;
        }

        /// <summary>
        /// Creates the polygon from array with coordinates.
        /// </summary>
        /// <returns>The polygon from this data.</returns>
        /// <param name="Points">Array with coordinates.</param>
        public Polygon CreatePolygonFromData(double[] Points)
        {
            Polygon NewPolygonAsResult = new Polygon();

            // latitude (N => S), longitude (W => E)

            // 1 degree latitude ~ 111 km
            // 1 degree longitude = EARTH_RADIUS * (Math::PI/180) * cos(lat*Math::PI/180)
            // EARTH_RADIUS = 6371
            for (int position = 0; position + 1 < Points.Length; position += 2)
                NewPolygonAsResult.Positions.Add(new Position(Points[position], Points[position + 1]));


            NewPolygonAsResult.IsClickable = true;
            NewPolygonAsResult.StrokeColor = defaultStrokeColor;

            NewPolygonAsResult.StrokeWidth = 1f;
            NewPolygonAsResult.FillColor = defaultFillColor;

            return NewPolygonAsResult;
        }

        /// <summary>
        /// Gets the polygon from string.
        /// </summary>
        /// <returns>Polygon obtained from the string.</returns>
        /// <param name="StringWithCoordinates">String with coordinates.</param>
        public Polygon GetPolygon(string coordinates)
        {
            double[] ArrayOfPoints = ConvertStringToArray(ref coordinates);

            Polygon polygon = CreatePolygonFromData(ArrayOfPoints);
            return polygon;
        }

        /// <summary>     
        /// Checks if point is in polygon (two checks with two rays).  
        /// </summary>
        /// <returns><c>true</c>, if point is in polygon, <c>false</c> otherwise.</returns>
        /// <param name="polygon">Polygon.</param>
        /// <param name="CurrentPosition">Current position.</param>
        public bool IsInPolygon(Polygon polygon, Location CurrentPosition)
        {
            LineEntity F1 = new LineEntity(polygon.Positions[0], polygon.Positions[1]);
            LineEntity F2 = new LineEntity(polygon.Positions[1], polygon.Positions[2]);
            LineEntity F3 = new LineEntity(polygon.Positions[2], polygon.Positions[3]);
            LineEntity F4 = new LineEntity(polygon.Positions[3], polygon.Positions[0]);

            // The end of first ray
            Position B = new Position(CurrentPosition.Longitude + 34.282828, CurrentPosition.Latitude + 83.181772626);
            // The end of second ray
            Position C = new Position(CurrentPosition.Longitude + 23.2828828, CurrentPosition.Latitude + 12.001827);

            LineEntity AB = new LineEntity(CurrentPosition, B);
            LineEntity AC = new LineEntity(CurrentPosition, C);


            int ABCounter = 0;
            if (lineInteractor.IfIntersect(AB, F1))
                ABCounter++;
            if (lineInteractor.IfIntersect(AB, F2))
                ABCounter++;
            if (lineInteractor.IfIntersect(AB, F3))
                ABCounter++;
            if (lineInteractor.IfIntersect(AB, F4))
                ABCounter++;

            int ACCounter = 0;
            if (lineInteractor.IfIntersect(AC, F1))
                ACCounter++;
            if (lineInteractor.IfIntersect(AC, F2))
                ACCounter++;
            if (lineInteractor.IfIntersect(AC, F3))
                ACCounter++;
            if (lineInteractor.IfIntersect(AC, F4))
                ACCounter++;

            if (ABCounter % 2 == 1 && ACCounter % 2 == 1)
                return true;
            return false;
        }

        /// <summary>
        /// Sets the default color for all polygons.
        /// </summary>
        public void ResetAllPolygons()
        {
            for (int i = 0; i < GeometryEntity.polygons.Length; ++i)
                for (int j = 0; j < GeometryEntity.polygons[i].Length; ++j)
                    GeometryEntity.polygons[i][j].FillColor = defaultFillColor;

        }
    }
}
