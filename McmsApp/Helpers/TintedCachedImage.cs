using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.Helpers
{
    public class TintedCachedImage : Image
    {
        public static BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(TintedCachedImage), Colors.Transparent, propertyChanged: UpdateColor);

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        private static void UpdateColor(BindableObject bindable, object oldColor, object newColor)
        {
            var view = (TintedCachedImage)bindable;
            var oldcolor = (Color)oldColor;
            var newcolor = (Color)newColor;

            if (!oldcolor.Equals(newcolor))
            {
                view.TintColor = newcolor;
            }
        }
    }
}
