using System;
using System.Globalization;

namespace mcms
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        CultureInfo GetCurrentCultureInfo(string sLanguageCode);
        void SetLocale();
        void ChangeLocale(string sLanguageCode);
    }
}
