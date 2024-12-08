using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.SQA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SQADetail : ContentPage
    {
        public SQADetail(SqaViewModel sqaviewmodel)
        {
            InitializeComponent();
            BindingContext = sqaviewmodel;
        }
    }
}