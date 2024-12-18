using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using mcms.Models;
using mcms.ViewModels;
using mcms.Views.Work.WorkDetail;
using Syncfusion.XForms.TabView;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkorderList : ContentPage
    {

        public WoListViewModel woListViewModel;
        public SfTabView TabView;
        public INavigation nav;
        public WorkorderList(INavigation navigation, SfTabView tabView)
        {
            nav = navigation;
            TabView = tabView;
            woListViewModel = new WoListViewModel(navigation, this);
            BindingContext = woListViewModel;
            InitializeComponent();
            //map.MyLocationEnabled = true;
            //map.UiSettings.MyLocationButtonEnabled = true;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height)
            {
                if (Search.IsVisible)
                {
                    Search.WidthRequest = width;
                }
            }
        }

        /// <summary>
        /// Invoked when search button is clicked.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event Args</param>
        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            this.SearchButton.IsVisible = false;
            this.DownloadButton.IsVisible = false;
            this.FilterButton.IsVisible = false;
            this.MapButton.IsVisible = false;
            this.Search.IsVisible = true;
            this.ProfileView.IsVisible = false;

            if (this.TitleBar != null)
            {
                double opacity;

                // Animating Width of the search box, from 0 to full width when it added to the view.
                var expandAnimation = new Animation(
                    property =>
                    {
                        Search.WidthRequest = property;
                        opacity = property / TitleBar.Width;
                        Search.Opacity = opacity;
                    }, 0, TitleBar.Width, Easing.Linear);
                expandAnimation.Commit(Search, "Expand", 16, 250, Easing.Linear, (p, q) => this.SearchExpandAnimationCompleted());
            }
        }


        /// <summary>
        /// Invoked when back to title button is clicked.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Event Args</param>
        private void BackToTitle_Clicked(object sender, EventArgs e)
        {
            this.SearchButton.IsVisible = true;
            this.DownloadButton.IsVisible = true;
            this.FilterButton.IsVisible = true;
            this.MapButton.IsVisible = true;
            if (this.TitleBar != null)
            {
                double opacity;

                // Animating Width of the search box, from full width to 0 before it removed from view.
                var shrinkAnimation = new Animation(property =>
                {
                    Search.WidthRequest = property;
                    opacity = property / TitleBar.Width;
                    Search.Opacity = opacity;
                },
                TitleBar.Width, 0, Easing.Linear);
                shrinkAnimation.Commit(Search, "Shrink", 16, 250, Easing.Linear, (p, q) => this.SearchBoxAnimationCompleted());
            }

            SearchEntry.Text = string.Empty;
        }

        /// <summary>
        /// Invokes when search box Animation completed.
        /// </summary>
        private void SearchBoxAnimationCompleted()
        {
            this.Search.IsVisible = false;
            this.ProfileView.IsVisible = true;
        }

        protected override bool OnBackButtonPressed()
        {
            if (!this.FilterWoView.IsVisible)
            {
                return base.OnBackButtonPressed();
            }

            this.FilterWoView.Hide();
            return true;
        }

        public void FilterView(bool val)
        {
            if (val)
            {
                FilterWoView.Show();
            }
            else
            {
                FilterWoView.Hide();
            }
        }


        /// <summary>
        /// Invokes when search expand Animation completed.
        /// </summary>
        private void SearchExpandAnimationCompleted()
        {
            this.SearchEntry.Focus();
        }
    }
}
