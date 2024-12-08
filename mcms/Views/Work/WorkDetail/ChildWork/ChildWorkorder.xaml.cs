using System;
using System.Collections.Generic;
using mcms.Models;
using mcms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.ChildWork
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChildWorkorder : ContentPage
    {
        public ChildWorkorder(ChildWOViewModel childwoviewmodel)
        {
            BindingContext = childwoviewmodel;
            InitializeComponent();
        }
    }
}
