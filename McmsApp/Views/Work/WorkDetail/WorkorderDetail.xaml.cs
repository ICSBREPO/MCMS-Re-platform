using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;
using mcms.Persistence;
using mcms.ViewModels;
using mcms.Views.Tabbed;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using System.Diagnostics;

namespace mcms.Views.Work.WorkDetail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkorderDetail : ContentPage
    {
        public Location loc { get; set; }
        WoDetailViewModel woDetailViewModel { get; set; }
        IWorkorder sqlite = new SQLiteWorkorder();
        //Navigation inherit from view

        public WorkorderDetail(WoDetailViewModel woviewmodel)
        {
            woDetailViewModel = woviewmodel;
            BindingContext = woDetailViewModel;
            InitializeComponent();
          /*  map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.PinDragEnd += async (sender, args) =>
            {
                await UserDialogs.Instance.AlertAsync($"Lat : {args.Pin.Position.Latitude}, Long : {args.Pin.Position.Longitude}", "Info", "Ok");
                loc.Latitude = args.Pin.Position.Latitude;
                loc.Longitude = args.Pin.Position.Longitude;
            };*/
            _ = ShowMapPicker();
        }

        public async Task ShowMapPicker()
        {
            try
            {
              /*  await Task.Yield();
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5000));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Position(location.Latitude, location.Longitude), Distance.FromMeters(100)));
                    Grid grid = new Grid
                    {
                        RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(35) }
                    },
                        ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(35) }
                    }
                    };

                    Image image = new Image() { Source = "map_pin.png", WidthRequest = 35, HeightRequest = 35 };
                    grid.Children.Add(image, 0, 0);
                    loc = location;
                    map.Pins.Clear();
                    Pin pinTNB1 = null;
                    pinTNB1 = new Pin()
                    {
                        Type = PinType.SavedPin,
                        Label = "Current Position",
                        Address = "Current Position",
                        Position = new Position(location.Latitude, location.Longitude),
                        IsDraggable = true,
                        Rotation = 0f,
                        Tag = "curr1",
                        Icon = BitmapDescriptorFactory.FromView(new ContentView
                        {
                            WidthRequest = 35,
                            HeightRequest = 35,
                            Content = grid
                        })
                    };
                    map.Pins.Add(pinTNB1);
                    
                }*/
            }
            catch
            {

            }
        }

        async void Gps_Clicked(System.Object sender, System.EventArgs e)
        {
            int id = woDetailViewModel.workorder.id;
            Workorder wo = await sqlite.GetWorkorder(id);
            wo.tnb_latitude2 = loc.Latitude;
            wo.tnb_longitude2 = loc.Longitude;
            woDetailViewModel.workorder.tnb_latitude2 = Math.Round(loc.Latitude, 8);
            woDetailViewModel.workorder.tnb_longitude2 = Math.Round(loc.Longitude, 8);
            wo.pendingupdate = true;
            await sqlite.UpdateWorkorder(wo);
            await UserDialogs.Instance.AlertAsync($"Location Data Successfully Updated", "Success", "Ok");
        }

       
        }
}
