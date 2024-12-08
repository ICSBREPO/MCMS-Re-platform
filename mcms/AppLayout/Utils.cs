using System.Linq;
using mcms.Themes;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace mcms.AppLayout
{
    [Preserve(AllMembers = true)]
    public static class Extensions
    {
        public static void ApplyDarkTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;
                var lightTheme = mergedDictionaries.OfType<LightTheme>().FirstOrDefault();
                if (lightTheme != null)
                {
                    mergedDictionaries.Remove(lightTheme);
                }

                // mergedDictionaries.Add(new DarkTheme());
                AppSettings.Instance.IsDarkTheme = true;
            }
        }

        public static void ApplyLightTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;

                // var darkTheme = mergedDictionaries.OfType<DarkTheme>().FirstOrDefault();
                // if (darkTheme != null)
                // {
                //     mergedDictionaries.Remove(darkTheme);
                // }
                mergedDictionaries.Add(new LightTheme());
                AppSettings.Instance.IsDarkTheme = false;
            }
        }

        public static void ApplyColorSet(int index)
        {
            switch (index)
            {
                case 1:
                    ApplyColorSet1();
                    break;
                case 2:
                    ApplyColorSet2();
                    break;
                case 3:
                    ApplyColorSet3();
                    break;
                case 4:
                    ApplyColorSet4();
                    break;
                default:
                    ApplyColorSet1();
                    break;
            }
        }

        public static void ApplyColorSet1()
        {
            Application.Current.Resources["PrimaryColor"] = Color.FromHex("#F54E5E");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#d0424f");
            Application.Current.Resources["PrimaryDarkenColor"] = Color.FromHex("#ab3641");
            Application.Current.Resources["PrimaryLighterColor"] = Color.FromHex("#edcacd");
            Application.Current.Resources["PrimaryLight"] = Color.FromHex("#ffe8f4");
            Application.Current.Resources["PrimaryGradient"] = Color.FromHex("#e83f94");
            Application.Current.Resources["Secondary"] = Color.FromHex("#2F80ED");
            Application.Current.Resources["Background"] = Color.FromHex("#f6f7f8");
            Application.Current.Resources["Card"] = Color.FromHex("#FFFFFF");
            Application.Current.Resources["TabBar"] = Color.FromHex("#FBFBFB");
            Application.Current.Resources["Label"] = Color.FromHex("#333942");
        }

        public static void ApplyColorSet2()
        {
            Application.Current.Resources["PrimaryColor"] = Color.FromHex("#56CCF2");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#1a5ac6");
            Application.Current.Resources["PrimaryDarkenColor"] = Color.FromHex("#174fb0");
            Application.Current.Resources["PrimaryLighterColor"] = Color.FromHex("#73a0ed");
            Application.Current.Resources["PrimaryLight"] = Color.FromHex("#cdddf9");
            Application.Current.Resources["PrimaryGradient"] = Color.FromHex("#00acff");
            Application.Current.Resources["Secondary"] = Color.FromHex("#7EE5F2");
            Application.Current.Resources["Background"] = Color.FromHex("#2e333b");
            Application.Current.Resources["Card"] = Color.FromHex("#23262e");
            Application.Current.Resources["TabBar"] = Color.FromHex("#21242b");
            Application.Current.Resources["Label"] = Color.FromHex("#cdd4e8");
        }

        public static void ApplyColorSet3()
        {
            Application.Current.Resources["PrimaryColor"] = Color.FromHex("#4592D0");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#4b3ae1");
            Application.Current.Resources["PrimaryDarkenColor"] = Color.FromHex("#3829ba");
            Application.Current.Resources["PrimaryLighterColor"] = Color.FromHex("#b5aefb");
            Application.Current.Resources["PrimaryLight"] = Color.FromHex("#eae8fe");
            Application.Current.Resources["PrimaryGradient"] = Color.FromHex("#aa9cfc");
            Application.Current.Resources["Secondary"] = Color.FromHex("#C4C4C4");
            Application.Current.Resources["Background"] = Color.FromHex("#2e333b");
            Application.Current.Resources["Card"] = Color.FromHex("#23262e");
            Application.Current.Resources["TabBar"] = Color.FromHex("#21242b");
            Application.Current.Resources["Label"] = Color.FromHex("#FFFFFF");
        }

        public static void ApplyColorSet4()
        {
            Application.Current.Resources["PrimaryColor"] = Color.FromHex("#44C868");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#056c56");
            Application.Current.Resources["PrimaryDarkenColor"] = Color.FromHex("#045343");
            Application.Current.Resources["PrimaryLighterColor"] = Color.FromHex("#98f0de");
            Application.Current.Resources["PrimaryLight"] = Color.FromHex("#ebf9f7");
            Application.Current.Resources["PrimaryGradient"] = Color.FromHex("#0ed342");
            Application.Current.Resources["Secondary"] = Color.FromHex("#FFDB56");
            Application.Current.Resources["Background"] = Color.FromHex("#2e333b");
            Application.Current.Resources["Card"] = Color.FromHex("#23262e");
            Application.Current.Resources["TabBar"] = Color.FromHex("#21242b");
            Application.Current.Resources["Label"] = Color.FromHex("#E1E1E1");
        }
    }
}
