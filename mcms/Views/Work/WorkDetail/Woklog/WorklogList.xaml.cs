using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Woklog
{
    public partial class WorklogList : ContentPage
    {
        public WorklogList(WorklogWOViewModel worklogWOViewModel)
        {
            BindingContext = worklogWOViewModel;
            InitializeComponent();
        }
    }
}
