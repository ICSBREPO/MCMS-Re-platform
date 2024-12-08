using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Acr.UserDialogs;
using Plugin.Media;
using mcms.AppLayout;

namespace mcms.Droid
{
    [Activity(Label = "MCMS SIT/STG", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            CachedImageRenderer.Init(enableFastRenderer: true);
            CachedImageRenderer.InitImageViewHandler();
            //Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
            //int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            //if (resourceId > 0)
            //{
            //    AppSettings.Instance.IsSafeAreaEnabled = true;
            //    AppSettings.Instance.SafeAreaHeight = (int)(Resources.GetDimensionPixelSize(resourceId) / Resources.DisplayMetrics.Density);
            //}

            base.OnCreate(savedInstanceState);
            await CrossMedia.Current.Initialize();
            UserDialogs.Init(this);
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init(this);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState); // initialize for
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            LoadApplication(new App());

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            // Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}