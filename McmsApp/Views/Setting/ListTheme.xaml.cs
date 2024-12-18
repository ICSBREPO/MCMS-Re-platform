using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FFImageLoading;
using mcms.AppLayout;
using mcms.ViewModels;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.Border;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Setting
{
    [Preserve(AllMembers = true)]
    public partial class ListTheme : ContentPage
    {
        private double width;
        private double height;
        public ListTheme()
        {
            InitializeComponent();
            BindingContext = new SettingViewModel(Navigation);
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if ((Device.RuntimePlatform == "iOS" || Device.RuntimePlatform == "Android") &&
                AppSettings.Instance.IsSafeAreaEnabled)
            {
                if (width != this.width || height != this.height)
                {
                    var safeAreaHeight = AppSettings.Instance.SafeAreaHeight;
                    this.width = width;
                    this.height = height;

                    if (width < height)
                    {
                        iOSSafeArea.Height = safeAreaHeight;
                    }
                    else
                    {
                        iOSSafeArea.Height = 0;
                    }
                }
            }
        }
    }
}
