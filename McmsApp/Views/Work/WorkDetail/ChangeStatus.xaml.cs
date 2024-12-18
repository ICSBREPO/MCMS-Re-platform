using System;
using System.Collections.Generic;
using mcms.Models;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work.WorkDetail
{
    public partial class ChangeStatus : ContentPage
    {

        public ChangeStatus(WoDetailViewModel woDetailViewModel)
        {
            BindingContext = woDetailViewModel;
            InitializeComponent();
        }        
    }
}
