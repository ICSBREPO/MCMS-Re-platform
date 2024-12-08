using mcms.AppLayout;
using mcms.Models;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Attachment
{
    public partial class AddAttachment : ContentPage
    {
        private Workorder workorder;
        private INavigation navigation;
        private double width;
        private double height;
        AttachmentWOViewModel attachmentWOViewModel;

        public AddAttachment(AttachmentWOViewModel attachmentwoviewmodel)
        {
            BindingContext = attachmentwoviewmodel;
            attachmentWOViewModel = attachmentwoviewmodel;
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

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            if (attachmentWOViewModel.m_pdfDocumentStream != null)
            {
                pdfViewer.LoadDocument(attachmentWOViewModel.m_pdfDocumentStream);
            }
        }*/
    }
}
