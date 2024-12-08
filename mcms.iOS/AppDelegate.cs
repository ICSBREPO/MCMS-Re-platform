using FFImageLoading.Forms.Platform;
using Foundation;
using mcms.AppLayout;
using mcms.Helpers;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfPicker.XForms.iOS;
using Syncfusion.SfPullToRefresh.XForms.iOS;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.Core;
using Syncfusion.XForms.iOS.Shimmer;
using Syncfusion.XForms.iOS.EffectsView;
using Syncfusion.XForms.iOS.TabView;
using Syncfusion.XForms.iOS.TextInputLayout;
using UIKit;
using Syncfusion.SfBusyIndicator.XForms.iOS;

namespace mcms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            CachedImageRenderer.Init();
            CachedImageRenderer.Init();
            new SfBusyIndicatorRenderer();
            SfTextInputLayoutRenderer.Init();
            SfPullToRefreshRenderer.Init();
            SfPickerRenderer.Init();
            SfListViewRenderer.Init();
            SfButtonRenderer.Init();
            SfTabViewRenderer.Init();
            SfChipGroupRenderer.Init();
            SfShimmerRenderer.Init();
            SfEffectsViewRenderer.Init();
            Syncfusion.XForms.iOS.SignaturePad.SfSignaturePadRenderer.Init();
            Syncfusion.XForms.iOS.Expander.SfExpanderRenderer.Init();
            Syncfusion.XForms.iOS.PopupLayout.SfPopupLayoutRenderer.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            new Syncfusion.SfNavigationDrawer.XForms.iOS.SfNavigationDrawerRenderer();
            Syncfusion.XForms.iOS.ProgressBar.SfLinearProgressBarRenderer.Init();
            Syncfusion.XForms.iOS.Border.SfBorderRenderer.Init();
            Syncfusion.XForms.iOS.BadgeView.SfBadgeViewRenderer.Init();
            Syncfusion.SfPdfViewer.XForms.iOS.SfPdfDocumentViewRenderer.Init();
            Syncfusion.SfRangeSlider.XForms.iOS.SfRangeSliderRenderer.Init();
            Stormlion.PhotoBrowser.iOS.Platform.Init();
            Core.Init();
            Xamarin.FormsGoogleMaps.Init(GoogleMapsApikey.ApiKey);

            LoadApplication(new App());

            var result = base.FinishedLaunching(app, options);

            var safeAreInset = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
            if (safeAreInset.Top > 0)
            {
                AppSettings.Instance.IsSafeAreaEnabled = true;
                AppSettings.Instance.SafeAreaHeight = safeAreInset.Top;
            }
            return result;


        }
    }
}
