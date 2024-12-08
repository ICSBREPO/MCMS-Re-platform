using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Permit
{
    public partial class PermitList : ContentPage
    {
        public PermitList(PermitViewModel permitViewModel)
        {
            InitializeComponent();
            BindingContext = permitViewModel;
        }
    }
}
