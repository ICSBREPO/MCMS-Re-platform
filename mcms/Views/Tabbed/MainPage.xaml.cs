using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Acr.UserDialogs;
using mcms.Resx;
using System.Threading.Tasks;
using mcms.AppLayout;
using mcms.Models;
using mcms.Persistence;
using mcms.Views.Home;
using mcms.Views.Notification;
using mcms.Views.Setting;
using mcms.Views.Work;
using Syncfusion.XForms.TabView;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace mcms.Views.Tabbed
{
    [DesignTimeVisible(false)]
    public partial class MainPage
    {

        IUserProfile sqlprofile = new SQLiteUserprofile();
        private double width;
        private double height;

        public MainPage()
        {
            InitializeComponent();
            // Set Language
            // setLanguage(1033);
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            this.BackgroundColor = (Color)App.Current.Resources["Background"];
            _ = loadCurrTheme();
            
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
                        if(Device.Idiom == TargetIdiom.Phone)
                        {
                            iOSSafeArea2.Height = safeAreaHeight - 5;
                        }
                        else
                        {
                            iOSSafeArea2.Height = 0;
                        }
                    }
                    else
                    {
                        iOSSafeArea.Height = 0;
                        iOSSafeArea2.Height = 0;
                    }
                }
            }
        }

        async Task loadCurrTheme()
        {
            string username = await SecureStorage.GetAsync("username");
            UserProfile usr = await sqlprofile.GetUserProfileByUserName(username);
            AppSettings.Instance.SelectedThemeColor = usr.themeChoose;

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false); // Set NavBar Hidden

            //Tab Page Config
            string ff = Device.RuntimePlatform == "iOS" ? "EvaIcons" : Device.RuntimePlatform == "Android" ? "EvaIcons.ttf#EvaIcons" : "Assets/EvaIcons.ttf#EvaIcons";
            Color selectedcolor = (Color)App.Current.Resources["PrimaryColor"];
            Color unselectedcolor = (Color)App.Current.Resources["Gray-600"];
            var tabView = new SfTabView();
            tabView.EnableSwiping = false;
            //if (Device.Idiom == TargetIdiom.Phone)
            //{
            //    tabView.TabHeight = 80;
            //}
            tabView.DisplayMode = TabDisplayMode.ImageWithText;
            
            tabView.TabHeaderBackgroundColor = (Color)App.Current.Resources["TabBar"];
            tabView.SelectionChanged += (object sender, Syncfusion.XForms.TabView.SelectionChangedEventArgs e) =>
            {
                SfTabView sfnew = sender as SfTabView;
                sfnew.SelectionIndicatorSettings.Color = (Color)App.Current.Resources["PrimaryColor"];
                foreach (SfTabItem tabitem in (sender as SfTabView).Items)
                {
                    if (tabitem.Title == e.Name)
                    {
                        tabitem.SelectionColor = (Color)App.Current.Resources["PrimaryColor"];
                        if (e.Name == "Home")
                        {
                            tabitem.Content = new HomePage(Navigation, tabView).Content;
                            tabitem.IconFont = "\ue97c";
                        }
                        else if (e.Name == "Workorder List")
                        {
                            tabitem.IconFont = "\ue965";
                        }
                        else if (e.Name == "Notification")
                        {
                            tabitem.IconFont = "\ue922";
                        }
                        else if (e.Name == "Setting")
                        {
                            tabitem.IconFont = "\ue98d";
                        }
                        tabitem.TitleFontAttributes = FontAttributes.Bold;
                    }
                    else
                    {

                        tabitem.SelectionColor = (Color)App.Current.Resources["PrimaryColor"];
                        if (tabitem.Title == "Home")
                        {
                            tabitem.IconFont = "\uea71";
                        }
                        else if (tabitem.Title == "Workorder List")
                        {
                            tabitem.IconFont = "\uea5c";
                        }
                        else if (tabitem.Title == "Notification")
                        {
                            tabitem.IconFont = "\uea18";
                        }
                        else if (tabitem.Title == "Setting")
                        {
                            tabitem.IconFont = "\uea84";
                        }
                        tabitem.TitleFontAttributes = FontAttributes.None;
                    }
                }
            };
            var tabItems = new TabItemCollection
            {
                new SfTabItem()
                {
                    Content = new HomePage(Navigation,tabView).Content,
                    Title = "Home",
                    IconFont = "\ue97c",
                    FontIconFontFamily = ff,
                    FontIconFontColor = unselectedcolor,
                    TitleFontColor = unselectedcolor,
                    FontIconFontSize =  20,
                    SelectionColor = selectedcolor,
                },
                new SfTabItem()
                {
                    Title = "Workorder List",
                    Content = new WorkorderList(Navigation,tabView).Content,
                    IconFont = "\uea5c",
                    FontIconFontFamily = ff,
                    FontIconFontColor = unselectedcolor,
                    TitleFontColor = unselectedcolor,
                    FontIconFontSize =  20,
                    SelectionColor = selectedcolor,
                },
                //new SfTabItem()
                //{
                //    Title = "Notification",
                //    Content = new NotificationList(Navigation).Content,
                //    IconFont = "\uea18",
                //    FontIconFontFamily = ff,
                //    FontIconFontColor = unselectedcolor,
                //    TitleFontColor = unselectedcolor,
                //    FontIconFontSize =  20,
                //    SelectionColor = selectedcolor,
                //},
                new SfTabItem()
                {
                    Title = "Setting",
                    Content = new SettingPage(Navigation).Content,
                    IconFont = "\uea84",
                    FontIconFontFamily = ff,
                    FontIconFontColor = unselectedcolor,
                    TitleFontColor = unselectedcolor,
                    FontIconFontSize =  20,
                    SelectionColor = selectedcolor,
                }
            };
            tabView.TabHeaderPosition = TabHeaderPosition.Bottom;
            tabView.VisibleHeaderCount = 3;
            var selectionIndicatorSettings = new SelectionIndicatorSettings();
            selectionIndicatorSettings.Color = selectedcolor;
            selectionIndicatorSettings.AnimationDuration = 500;
            selectionIndicatorSettings.Position = SelectionIndicatorPosition.Top;
            selectionIndicatorSettings.StrokeThickness = 3;
            tabView.SelectionIndicatorSettings = selectionIndicatorSettings;
            tabView.OverflowMode = OverflowMode.Scroll;
            tabView.Items = tabItems;
            ContentTab.Content = tabView;
            //end tab page config
        }


    }
}
