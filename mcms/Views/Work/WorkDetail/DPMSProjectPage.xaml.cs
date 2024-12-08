using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail
{
    public partial class DPMSProjectPage : ContentPage
    {
        public DPMSProjectPage(ProjectDPMSViewModel projectDPMSViewModel)
        {
            BindingContext = projectDPMSViewModel;
            InitializeComponent();
        }

    }
}
