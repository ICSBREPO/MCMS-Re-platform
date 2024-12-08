using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using mcms.Views.Home;
using mcms.Views.Login;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.Views.Setting
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage(INavigation navigation)
        {
            BindingContext = new SettingViewModel(navigation);
            InitializeComponent();
            Version.Text = AppInfo.VersionString; 
        }
    }
}
