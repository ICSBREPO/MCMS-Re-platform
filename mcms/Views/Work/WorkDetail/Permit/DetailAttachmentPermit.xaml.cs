using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Permit
{
    public partial class DetailAttachmentPermit : ContentPage
    {
        private double width;
        private double height;
        PermitViewModel permitViewModel;

        public DetailAttachmentPermit(PermitViewModel permitviewmodel)
        {
            BindingContext = permitviewmodel;
            permitViewModel = permitviewmodel;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (permitViewModel.m_pdfDocumentStream!=null)
            {
                pdfViewer.LoadDocument(permitViewModel.m_pdfDocumentStream);
            }
        }
    }
}
