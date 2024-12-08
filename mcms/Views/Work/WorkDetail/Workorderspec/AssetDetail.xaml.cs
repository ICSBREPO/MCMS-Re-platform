using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.Workorderspec
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssetDetail : ContentPage
    {
        public AssetDetail(AssetViewModel assetviewmodel)
        {
            InitializeComponent();
            BindingContext = assetviewmodel;
        }
    }
}