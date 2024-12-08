using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.Models;
using mcms.ViewModels;
using mcms.Views.Home;
using Syncfusion.XForms.TabView;
using Xamarin.Forms;
using mcms.General;
using System.Diagnostics;
using System.Threading.Tasks;

namespace mcms.Views.Work.WorkDetail
{
    public partial class TemplateWODetail : ContentPage
    {
        private double height;
        private double width;
        TemplateWODetailViewModel templateviewmodel { get; set; }
        SfTabView sftabview;
        Workorder wo;
        INavigation navigation;


        public TemplateWODetail(Workorder wo, INavigation navigation, SfTabView sfTabView)
        {
            sftabview = sfTabView;
            this.wo = wo;
            this.navigation = navigation;
            templateviewmodel = new TemplateWODetailViewModel(wo, navigation, sftabview);
            BindingContext = templateviewmodel;
            InitializeComponent();
        }

        async void menuButton_Clicked(System.Object sender, System.EventArgs e)
        {
            



            await templateviewmodel.LoadCountingBadge();
            navigationDrawer.ToggleDrawer();
        }

        void SfButton_Clicked(System.Object sender, System.EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
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


            Global global = Global.Instance;
            global.isNewSQA = false;
            if(global.isRefreshWorkDetail)
            {
                _ = updateProgress();
                
                
            }
            
            /*Global global = Global.Instance;
            if (global.isRefreshWorkDetail)
            {
                Navigation.PushAsync(new TemplateWODetail(wo, navigation, sftabview));
                global.isRefreshWorkDetail = false;
            }*/
            //await templateviewmodel.LoadProgress();
            Debug.WriteLine("I am on Appearing Method");
            //int count= _ = templateviewmodel.progressdata.Count;

            //Debug.WriteLine("WoProgressList is : " + count);
           // await templateviewmodel.LoadProgress();
            base.OnAppearing();

            //templateviewmodel.WoProgressList.Clear();
            // _ = templateviewmodel.LoadProgress();
            /*if (global.isRefreshWorkDetail)
            {


                //Navigation.PopToRootAsync();

                Navigation.PushAsync(new TemplateWODetail(wo, navigation, sftabview));
                // Navigation.PopAsync();

            }*/

            // templateviewmodel.WoProgressList = templateviewmodel.progressdata;


            /*var name = this.GetType().Name;
            if(name.Equals("TemplateWODetail"))
            {
                _ = templateviewmodel.LoadProgress();
            }
           
            */
            
                
        }


        private async Task updateProgress()
        {
            Debug.Write("I am called : from updateprogress");

            
             await templateviewmodel.LoadProgress();
         
        }
        

    }

}
