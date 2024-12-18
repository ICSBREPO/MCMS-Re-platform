using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.Persistence;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.SQA
{
    public partial class SQAAttachment : ContentPage
    {
        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        private SqaViewModel SqaViewModel;
        private double width;
        private double height;

        public SQAAttachment(SqaViewModel sqaviewmodel)
        {
            SqaViewModel = sqaviewmodel;
            BindingContext = SqaViewModel;
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
