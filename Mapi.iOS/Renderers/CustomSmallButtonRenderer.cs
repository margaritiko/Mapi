using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CustomRenderer.iOS;
using CoreGraphics;
using System;
using Mapi;

[assembly: ExportRenderer(typeof(CustomSmallButton), typeof(CustomSmallButtonRenderer))]
namespace CustomRenderer.iOS
{
    public class CustomSmallButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                var view = (CustomSmallButton)Element;

                Control.Layer.CornerRadius = Convert.ToSingle(5);

                Control.Layer.BorderColor = new CGColor(red: 0.73f, green: 0.78f, blue: 0.83f, alpha: 1.0f);
                Control.ClipsToBounds = true;
            }
        }
    }
}
