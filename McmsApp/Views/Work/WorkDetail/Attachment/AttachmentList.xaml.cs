using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Attachment
{
    public partial class AttachmentList : ContentPage
    {
        public AttachmentList(AttachmentWOViewModel attachmentwoviewmodel)
        {
            BindingContext = attachmentwoviewmodel;
            InitializeComponent();
        }
    }
}
