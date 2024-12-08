using System;
using System.Collections.Generic;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Work
{
    public partial class LookupUserModal : ContentPage
    {
        public LookupUserModal(object viewModels,string page,INavigation navigation,string type="person")
        {
            BindingContext = new LookupViewModel(viewModels,page,navigation,type);
            InitializeComponent();
        }
    }
}
