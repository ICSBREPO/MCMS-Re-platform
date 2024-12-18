using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.Models;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Attachment
{
    public partial class AttachmentChecklist : ContentPage
    {
        private double width;
        private double height;
        public AttachmentChecklist(Workorder wo, INavigation navigation, Tnbwochecklist chcktype, int tnbwochecklisttypeid)
        {

            //change chlklisttypeid to checklistid
            BindingContext = new AttachmentWOViewModel(wo, navigation,null,chcktype, chcktype.tnbwochecklistid);
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
