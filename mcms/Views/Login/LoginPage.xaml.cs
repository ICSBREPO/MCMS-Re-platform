using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using mcms.ViewModels;
using mcms.Views.PopupPages;
using mcms.Views.Tabbed;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.Views.Login
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel loginmodel { get; set; }
        double height;
        double width;

        public LoginPage()
        {
            loginmodel = new LoginViewModel();
            BindingContext = loginmodel;
            InitializeComponent();
            version.Text = "Version : " + AppInfo.VersionString;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called

            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
            }

            UpdateLayout();
        }

        void UpdateLayout()
        {

            if (width > height)
            {
                SetupLandscapeLayout();
                Debug.WriteLine("Landscape");
            }
            else
            {
                SetupPortraitLayout();
                Debug.WriteLine("Portrait");
            }
        }

        void SetupLandscapeLayout()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                containerForm.Padding = new Thickness(80, 0, 80, 170);
                imageLogin.Margin = new Thickness(0, 10, 0, 0);
                imageLogin.HeightRequest = 60;
                imageLogin.WidthRequest = 60;

                if (Device.RuntimePlatform == Device.Android)
                {
                    usernameEntry.HeightRequest = 50;
                    passwordeEntry.HeightRequest = 50;
                    languageEntry.HeightRequest = 50;
                    buttonLogin.HeightRequest = 40;

                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    usernameEntry.HeightRequest = 20;
                    passwordeEntry.HeightRequest = 20;
                    languageEntry.HeightRequest = 20;
                    buttonLogin.HeightRequest = 40;
                }
            }

            if (Device.Idiom == TargetIdiom.Tablet || Device.Idiom == TargetIdiom.Desktop)
            {

                imageLogin.Margin = new Thickness(0, 0, 0, 0);
                imageLogin.HeightRequest = 139;
                imageLogin.WidthRequest = 139;

                if (Device.RuntimePlatform == Device.Android)
                {

                    containerForm.Padding = new Thickness(174, 191, 174, 428);
                    usernameEntry.HeightRequest = 50;
                    passwordeEntry.HeightRequest = 50;
                    languageEntry.HeightRequest = 50;
                    buttonLogin.HeightRequest = 40;

                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    containerForm.Padding = new Thickness(174, 191, 174, 428);
                    usernameEntry.HeightRequest = 30;
                    passwordeEntry.HeightRequest = 30;
                    languageEntry.HeightRequest = 30;
                    buttonLogin.HeightRequest = 50;
                }
                else if (Device.RuntimePlatform == Device.UWP)
                {

                    containerForm.Padding = new Thickness(300, 10, 300, 0);
                    usernameEntry.HeightRequest = 30;
                    passwordeEntry.HeightRequest = 30;
                    languageEntry.HeightRequest = 30;
                    buttonLogin.HeightRequest = 50;
                }

            }

        }

        void SetupPortraitLayout()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                Debug.WriteLine("Phone");
                containerForm.Padding = new Thickness(35, 70, 35, 70);
                imageLogin.Margin = new Thickness(0, 0, 0, 0);
                imageLogin.HeightRequest = 139;
                imageLogin.WidthRequest = 139;

                if (Device.RuntimePlatform == Device.Android)
                {
                    usernameEntry.HeightRequest = 50;
                    passwordeEntry.HeightRequest = 50;
                    languageEntry.HeightRequest = 50;
                    buttonLogin.HeightRequest = 40;

                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    usernameEntry.HeightRequest = 30;
                    passwordeEntry.HeightRequest = 30;
                    languageEntry.HeightRequest = 30;
                    buttonLogin.HeightRequest = 40;
                }

            }

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                Debug.WriteLine("Tablet");
                containerForm.Padding = new Thickness(174, 191, 174, 428);
                imageLogin.Margin = new Thickness(0, 0, 0, 20);
                imageLogin.HeightRequest = 250;
                imageLogin.WidthRequest = 139;

                if (Device.RuntimePlatform == Device.Android)
                {
                    usernameEntry.HeightRequest = 50;
                    passwordeEntry.HeightRequest = 50;
                    languageEntry.HeightRequest = 50;
                    buttonLogin.HeightRequest = 40;

                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    usernameEntry.HeightRequest = 30;
                    passwordeEntry.HeightRequest = 30;
                    languageEntry.HeightRequest = 30;
                    buttonLogin.HeightRequest = 45;
                }

            }

            if (Device.Idiom == TargetIdiom.Desktop)
            {
                containerForm.Padding = new Thickness(174, 191, 174, 428);
                imageLogin.Margin = new Thickness(0, 0, 0, 20);
                imageLogin.HeightRequest = 250;
                imageLogin.WidthRequest = 139;
                usernameEntry.HeightRequest = 30;
                passwordeEntry.HeightRequest = 30;
                languageEntry.HeightRequest = 30;
                buttonLogin.HeightRequest = 45;


            }
        }

    }
}