using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace mcms.Views.Work
{
    public partial class FilterWo : ContentView
    {
        public static readonly BindableProperty ParentContextProperty =
            BindableProperty.Create("ParentContext", typeof(object), typeof(FilterWo), null, propertyChanged: OnParentContextPropertyChanged);

        public object ParentContext
        {
            get { return GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        private static void OnParentContextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue && newValue != null)
            {
                (bindable as FilterWo).ParentContext = newValue;
            }
        }

        public FilterWo()
        {
            InitializeComponent();
        }

        public void Show()
        {
            IsVisible = true;
            MainContent.FadeTo(1);
            MainContent.TranslateTo(MainContent.TranslationX, 0);
            ShadowView.IsVisible = true;
        }

        public void Hide()
        {
            ShadowView.IsVisible = false;
            var fadeAnimation = new Animation(v => MainContent.Opacity = v, 1, 0);
            var translateAnimation = new Animation(v => MainContent.TranslationY = v, 0, MainContent.Height, null, () => { IsVisible = false; });

            var parentAnimation = new Animation { { 0.5, 1, fadeAnimation }, { 0, 1, translateAnimation } };
            parentAnimation.Commit(this, "HideSettings");
        }

        private void CloseSettings(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}