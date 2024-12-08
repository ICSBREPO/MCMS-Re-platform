using System;
using System.Collections.Generic;
using mcms.Resources;
using mcms.ViewModels;
using Syncfusion.XForms.TabView;
using Xamarin.Forms;

namespace mcms.Views.Home
{
    public partial class HomePage : ContentPage
    {
        HomeViewModel homeviewmodel;
        public SfTabView tabview;

        public HomePage(INavigation navigation, SfTabView tabView)
        {
            tabview = tabView;
            homeviewmodel = new HomeViewModel(navigation, this);
            BindingContext = homeviewmodel;
            InitializeComponent();
        }

        private async void SfButton_Clicked(object sender, EventArgs e)
        {
            tabview.SelectedIndex = 1;
        }

        void SfPicker_SelectionChanged(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            _ = homeviewmodel.LoadChart();
        }

    }
          
}
