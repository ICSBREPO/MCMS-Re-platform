using System.Globalization;
using System.Linq;
using System.Threading;
using System;
using mcms.ApiServices;
using mcms.AppLayout;
using mcms.Helpers;
using mcms.Resources;
using mcms.Resx;
using mcms.Models;
using mcms.Persistence;
using mcms.Views.Login;
using mcms.Views.Tabbed;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using DLToolkit.Forms.Controls;
using System.Threading.Tasks;

namespace mcms
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {

        public App()
        {
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDc2MDY1QDMxMzkyZTMyMmUzMGZqUmxvTVRQOVhjREUrTG1FUXpJMi82WXpIeWt6TnlnVEtVSEEzSUVkN3M9");
            InitializeComponent();
            GoogleMapsApi.Initialize(GoogleMapsApikey.ApiKey);
            _ = checkHostname();

            //Add to ad-bugfixing
            //Add to feature/rashid
        }

        async Task checkHostname()
        {
           
            string check = await SecureStorage.GetAsync("Hostname");
            if(check == null)
            {
                await SecureStorage.SetAsync("Hostname", "http://10.215.73.60");
            }
            string isLoogged = await SecureStorage.GetAsync("apikey");
            if (isLoogged != null)
            {
                this.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                this.MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
