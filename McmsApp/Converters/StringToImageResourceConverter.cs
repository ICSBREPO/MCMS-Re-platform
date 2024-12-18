using System.Globalization;
using System.Reflection;
using Microsoft.Maui.Controls.Internals;

namespace McmsApp.Converters
{
    [Preserve(AllMembers = true)]
    public class StringToImageResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageSource.FromResource((string)value, typeof(StringToImageResourceConverter).GetTypeInfo().Assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
