using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail.Testing
{
    public partial class MeterList : ContentPage
    {

        public MeterList(TestingMeterViewModel metertestingmodel)
        {
            InitializeComponent();
            BindingContext = metertestingmodel;
        }
    }
}
