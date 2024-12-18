using mcms.ApiServices;
using mcms.AppLayout;
using mcms.General;
using mcms.Models;
using mcms.Persistence;
using mcms.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.SQA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SQAForm : ContentPage
    {
        SqaViewModel sqaviewmodel { get; set; }
        public Plusgaudit sqa { get; set; }
        public string status { get; set; }

        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        public IPlusgaudline sqlplusgaud = new SQLitePlusgaudline();
        private double width;
        private double height;

        public SQAForm(SqaViewModel sqamodel)
        {
            sqaviewmodel = sqamodel;
            InitializeComponent();
            BindingContext = sqaviewmodel;

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

        void SfRadioButton_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {

            sqaviewmodel.CalculateScore();
        }

        void Entry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            sqaviewmodel.CalculateScore();
        }

       
        }
}