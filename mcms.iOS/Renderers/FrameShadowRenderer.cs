using System;
using CoreGraphics;
using mcms.Controls;
using mcms.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomShadowFrame), typeof(FrameShadowRenderer))]
namespace mcms.iOS.Renderers
{
    public class FrameShadowRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> element)
        {
            base.OnElementChanged(element);
            var customShadowFrame = (CustomShadowFrame)this.Element;
            if (customShadowFrame != null)
            {
                this.Layer.CornerRadius = customShadowFrame.Radius;
                this.Layer.ShadowOpacity = customShadowFrame.ShadowOpacity;
                this.Layer.ShadowOffset = new CGSize(customShadowFrame.ShadowOffsetWidth, customShadowFrame.ShadowOffSetHeight);
                this.Layer.Bounds.Inset(customShadowFrame.BorderWidth, customShadowFrame.BorderWidth);
                Layer.BorderColor = customShadowFrame.CustomBorderColor.ToCGColor();
                Layer.BorderWidth = (float)customShadowFrame.BorderWidth;
            }
        }
    }
}
