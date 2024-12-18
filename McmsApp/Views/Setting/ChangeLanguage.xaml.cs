using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Setting
{
    public partial class ChangeLanguage : ContentPage
    {
        private double width;
        private double height;

        public ChangeLanguage()
        {
            InitializeComponent();
            BindingContext = new SettingViewModel(Navigation);
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
