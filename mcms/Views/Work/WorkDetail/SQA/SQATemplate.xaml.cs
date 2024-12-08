using mcms.AppLayout;
using mcms.General;
using mcms.Models;
using mcms.Persistence;
using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.SQA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SQATemplate : ContentPage
    {
        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        private SqaViewModel SqaViewModel;
        private double width;
        private double height;

        public SQATemplate(SqaViewModel sqaviewmodel)
        {
            SqaViewModel = sqaviewmodel;
            BindingContext = SqaViewModel;
            InitializeComponent();
           
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if ((Device.RuntimePlatform == "iOS" || Device.RuntimePlatform == "Android") &&
                AppSettings.Instance.IsSafeAreaEnabled)
            {
                if (width != this.width || height != this.height)
                {
                    var safeAreaHeight = AppSettings.Instance.SafeAreaHeight;
                    this.width = width;
                    this.height = height;

                    if (width < height)
                    {
                        iOSSafeArea.Height = safeAreaHeight;
                    }
                    else
                    {
                        iOSSafeArea.Height = 0;
                    }
                }
            }
        }

        protected override void OnAppearing()
        {
            Debug.WriteLine("I am SQA Template.xaml.cs");

            Global global = Global.Instance;
            global.isNewSQA = true;


            base.OnAppearing();

        }


    }
}