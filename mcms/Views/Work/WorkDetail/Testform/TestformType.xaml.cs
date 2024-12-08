using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;
using mcms.General;

namespace mcms.Views.Work.WorkDetail.Testform
{
    public partial class TestformType : ContentPage
    {
        private double width;
        private double height;
      
      

         TestFormViewModel testmodel { get; set; }
        public TestformType(TestFormViewModel testformviewmodel)
        {
           
           
            testmodel = testformviewmodel;
            BindingContext = testmodel;
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

        void SfRadioButton_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            testmodel.calculatePercentage();
        }

        protected void OnAppearing()
        {
            base.OnAppearing();
          
           // BindingContext = testmodel;
        }



    }
}
