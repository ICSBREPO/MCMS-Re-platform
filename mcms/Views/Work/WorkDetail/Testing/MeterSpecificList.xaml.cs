using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.Models;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Testing
{
    public partial class MeterSpecificList : ContentPage
    {

        TestingMeterViewModel meterViewModel { get; set; }
        private double width;
        private double height;

        public MeterSpecificList(TestingMeterViewModel metermodel)
        {
            meterViewModel = metermodel;
            InitializeComponent();

            BindingContext = metermodel;
        }

        void SfPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            meterViewModel.selectedMeter.tnbnewreading = meterViewModel.selecteddomain;
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
