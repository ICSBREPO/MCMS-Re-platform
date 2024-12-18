using System;
using System.Collections.Generic;
using System.Diagnostics;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.SQA
{
    public partial class DigitalSignatureList : ContentPage
    {
        private double width;
        private double height;

        SqaViewModel sqamodel { get; set; }

        public DigitalSignatureList(SqaViewModel sqaviewmodel)
        {

            Debug.WriteLine("abc");
            sqamodel = sqaviewmodel;
            InitializeComponent();
            BindingContext = sqaviewmodel;
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
