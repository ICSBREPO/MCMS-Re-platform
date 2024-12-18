using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work
{
    public partial class WorkorderSync : ContentPage
    {
        WoListViewModel woviewmodel { get; set; }
        public WorkorderSync(WoListViewModel womodel)
        {
            woviewmodel = womodel;
            BindingContext = womodel;
            InitializeComponent();
        }

        void SfCheckBox_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            woviewmodel.checkAll();
        }
    }
}
