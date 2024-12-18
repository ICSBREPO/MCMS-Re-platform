using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.ImagePreview
{
    public partial class PreviewImage : ContentPage
    {
        private double width;
        private double height;
        PreviewimageViewModel previewImageViewModel { get; set; }

        public PreviewImage(INavigation navigation, string base64image)
        {
            previewImageViewModel = new PreviewimageViewModel(navigation, base64image);
            BindingContext = previewImageViewModel;
            InitializeComponent();
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
