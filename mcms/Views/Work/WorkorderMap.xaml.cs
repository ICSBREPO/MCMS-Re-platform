using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work
{
    public partial class WorkorderMap : ContentPage
    {
        public WorkorderMap(WoListViewModel womodel)
        {
            BindingContext = womodel;
            InitializeComponent();
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
        }

        public void FilterView(bool val)
        {
            if (val)
            {
                FilterWoView.Show();
            }
            else
            {
                FilterWoView.Hide();
            }
        }
    }
}
