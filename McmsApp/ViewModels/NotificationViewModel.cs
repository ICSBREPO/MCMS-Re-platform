using System;
using System.Collections.ObjectModel;
using McmsApp.Models;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {

        public ObservableCollection<NotifDummy> NotifList { get; set; }

        private Command<object> itemTappedCommand;

        /// <summary>
        /// Gets the command that will be executed when an item is selected.
        /// </summary>
        public Command<object> ItemTappedCommand
        {
            get
            {
                return this.itemTappedCommand ?? (this.itemTappedCommand = new Command<object>(this.ItemSelected));
            }
        }

        INavigation Navigation;

        public NotificationViewModel(INavigation navigation)
        {
            Navigation = navigation;
            LoadNotif();
        }

        public void LoadNotif()
        {
            NotifList = new ObservableCollection<NotifDummy>
            {
                new NotifDummy
                {
                    Name = "WO 001 Has been assigned to you!!",
                    IsRead = false,
                    ReceivedTime = DateTime.Now.AddDays(-2)
                },
                new NotifDummy
                {
                    Name = "WO 002 Has been assigned to you!!",
                    IsRead = true,
                    ReceivedTime = DateTime.Now.AddDays(-3)
                },
                new NotifDummy
                {
                    Name = "WO 003 Has been assigned to you!!",
                    IsRead = false,
                    ReceivedTime = DateTime.Now.AddDays(-3)
                },
            };
        }

        /// <summary>
        /// Invoked when an item is selected from the navigation list.
        /// </summary>
        /// <param name="selectedItem">Selected item from the list view.</param>
        private void ItemSelected(object selectedItem)
        {
            try
            {
                ((selectedItem as Syncfusion.ListView.XForms.ItemTappedEventArgs)?.ItemData as NotifDummy).IsRead = true;
            }
            catch (Exception)
            {

            }
            // Do something
        }

    }
}
