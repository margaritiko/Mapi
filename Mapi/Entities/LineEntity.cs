﻿using Xamarin.Forms.GoogleMaps; using Xamarin.Essentials;  namespace Mapi {     public class LineEntity     {         public Position X { get; set; }         public Position Y { get; set; }          public LineEntity(Position X, Position Y)         {             this.X = X;             this.Y = Y;         }          public LineEntity(Location X, Position Y)         {             Position _X = new Position(X.Latitude, X.Longitude);             this.X = _X;             this.Y = Y;         }     } }  