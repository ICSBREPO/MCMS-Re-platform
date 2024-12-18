using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcms.Models;
using mcms.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using mcms.Persistence;
using Xamarin.Essentials;

namespace mcms.Views.Work.WorkDetail.SQA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SQAList : ContentPage
    {

        SqaViewModel SqaViewModel { get; set; }

        public SQAList(SqaViewModel sqamodel)
        {
            SqaViewModel = sqamodel;
            BindingContext = SqaViewModel;
            InitializeComponent();
            
        }

    }
}