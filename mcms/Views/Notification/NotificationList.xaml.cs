using System;
using System.Collections.Generic;
using mcms.AppLayout;
using mcms.ViewModels;
using Xamarin.Forms;

namespace mcms.Views.Notification
{
    public partial class NotificationList : ContentPage
    {
        public NotificationList(INavigation navigation)
        {
            BindingContext = new NotificationViewModel(navigation);
            InitializeComponent();
        }
    }
}
