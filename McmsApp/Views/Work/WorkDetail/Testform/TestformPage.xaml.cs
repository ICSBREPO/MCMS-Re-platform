using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.Testform
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestformPage : ContentPage
    {
        public TestformPage(TestFormViewModel testformviewmodel)
        {
            InitializeComponent();
            BindingContext = testformviewmodel;
        }

    }
}