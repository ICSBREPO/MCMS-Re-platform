using System;
using Xamarin.Forms;

namespace mcms.AppLayout
{
    public class AppSettings
    {
        static AppSettings()
        {
            Instance = new AppSettings();
        }

        public static AppSettings Instance { get; }

        private bool isDarkTheme;

        public bool IsDarkTheme
        {
            get => this.isDarkTheme;
            set
            {
                if (this.isDarkTheme == value)
                {
                    return;
                }

                this.isDarkTheme = value;
                if (this.isDarkTheme)
                {
                    // Dark Theme
                    Application.Current.Resources.ApplyDarkTheme();
                }
                else
                {
                    // Light Theme
                    Application.Current.Resources.ApplyLightTheme();
                }
            }
        }

        public bool IsSafeAreaEnabled { get; set; } = false;

        public double SafeAreaHeight { get; set; }

        private int selectedThemeColor;
        public int SelectedThemeColor
        {
            get => this.selectedThemeColor;
            set
            {
                if (this.selectedThemeColor == value)
                {
                    return;
                }

                this.selectedThemeColor = value;
                Extensions.ApplyColorSet(this.selectedThemeColor);
            }
        }

    }
}
