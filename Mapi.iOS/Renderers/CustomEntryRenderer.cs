using Xamarin.Forms.Platform.iOS; using Xamarin.Forms; using UIKit; using CustomRenderer.iOS; using CoreGraphics; using System; using Mapi;  [assembly: ExportRenderer (typeof(CustomEntry), typeof(CustomEntryRenderer))] namespace CustomRenderer.iOS {
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)         {
            base.OnElementChanged(e);              if (Control != null)
            {
                var view = (CustomEntry)Element;                  Control.Layer.CornerRadius = Convert.ToSingle(30);                 Control.TextColor = new UIColor(red: 0.73f, green: 0.78f, blue: 0.83f, alpha: 1.0f);                  Control.Layer.BorderColor = new CGColor(red: 0.73f, green: 0.78f, blue: 0.83f, alpha: 1.0f);                 Control.Layer.BorderWidth = (nfloat)0.5;
                Control.ClipsToBounds = true;
            }
        }
    } }   