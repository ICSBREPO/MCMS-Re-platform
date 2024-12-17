using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Login;
using McmsApp.Views.Work;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class AssetViewModel : BaseViewModel
    {
        IAsset sqlasset = new SQLiteAsset();
        //rest api
        IMaximoApi maxrest = new MaximoApi();
        public Workorder workorder { get; set; }
        public int countasset { get; set; }
        INavigation Navigation { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DataLookupCommand { get; set; }
        public AssetViewModel(Workorder wo, INavigation navigation)
        {
            workorder = wo;
            Navigation = navigation;
            SaveCommand = new Command(Save);
            DataLookupCommand = new Command(lookupData);
            Loadasset();
        }

        async void Loadasset()
        {
            workorder.asset = await sqlasset.GetAssetByWonum(workorder.wonum);
            foreach(Asset asset in workorder.asset)
            {
                if (asset.pendingupload)
                {
                    asset.badgeicon = Syncfusion.Maui.Core.BadgeIcon.Away;
                }
                else
                {
                    asset.badgeicon = Syncfusion.Maui.Core.BadgeIcon.None;
                }
            }
            
            countasset = workorder.asset.Count;
        }

        async void lookupData(object selected)
        {
            string select = selected as string;
            await Navigation.PushModalAsync(new LookupUserModal(this, "asset", Navigation, select));
        }

        async void Save()
        {
            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            if (check)
            {
                UserDialogs.Instance.ShowLoading("Uploading Asset...");
                if (workorder.asset[0].tnbmanufdate.ToString() == "0001-01-01T00:00:00")
                {
                    workorder.asset[0].tnbmanufdate = null;
                }
                if (workorder.asset[0].tnbdeliverydate.ToString() == "0001-01-01T00:00:00")
                {
                    workorder.asset[0].tnbdeliverydate = null;
                }
                Asset updateasset = await maxrest.UpdateAsset(workorder.asset[0]);
                UserDialogs.Instance.HideLoading();

                if (updateasset == null)
                {
                    foreach(Asset asset in workorder.asset)
                    {
                        asset.pendingupload = true;
                        await sqlasset.UpdateAsset(asset);
                    }
                    await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                }
                else if (updateasset.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(updateasset.Error.message, "Failed!", "Ok");
                    if (updateasset.Error.reasonCode == "BMXAA9549E")
                    {
                        var hostname = await SecureStorage.GetAsync("Hostname");
                        SecureStorage.RemoveAll();
                        await SecureStorage.SetAsync("Hostname", hostname);
                        await Navigation.PushModalAsync(new LoginPage());
                        return;
                    }
                }
                else
                {
                    workorder.asset[0].pendingupload = false;
                    await sqlasset.UpdateAsset(workorder.asset[0]);
                    await UserDialogs.Instance.AlertAsync("Your Data Successfully Updated", "Success!", "Ok");
                }

            }
            else
            {
                workorder.asset[0].pendingupload = true;
                await sqlasset.UpdateAsset(workorder.asset[0]);
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
            }

        }
    }
}
