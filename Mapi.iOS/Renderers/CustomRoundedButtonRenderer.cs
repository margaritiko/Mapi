using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CustomRenderer.iOS;
using CoreGraphics;
using System;
using Mapi;

[assembly: ExportRenderer(typeof(CustomRoundedButton), typeof(CustomRoundedButtonRenderer))]
namespace CustomRenderer.iOS
{
    public class CustomRoundedButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var view = (CustomRoundedButton)Element;

                Control.Layer.CornerRadius = Convert.ToSingle(30);
                Control.TintColor = new UIColor(red: 0.73f, green: 0.78f, blue: 0.83f, alpha: 1.0f);
                Control.SetTitleColor(UIColor.White, UIControlState.Normal);

                Control.Layer.BorderColor = new CGColor(red: 0.73f, green: 0.78f, blue: 0.83f, alpha: 1.0f);
                Control.ClipsToBounds = true;
                Control.BackgroundColor = new UIColor(red: 0.00f, green: 0.54f, blue: 0.97f, alpha: 1.0f);

            }
        }
    }
}
