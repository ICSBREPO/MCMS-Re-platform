using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Login;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class WorkorderspecViewModel : BaseViewModel
    {
        IWorkorderspec sqlwospec = new SQLiteWorkorderspec();
        IAssetSpec sqlaspec = new SQLiteAssetSpec();
        ILocationSpec sqllspec = new SQLiteLocationSpec();
        IMxDomain sqldomain = new SQLiteMxDomain();
        IAsset sqlasset = new SQLiteAsset();
        //rest api
        IMaximoApi maxrest = new MaximoApi();
        public Workorder workorder { get; set; }
        public int countwospec { get; set; }
        INavigation Navigation { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand OpenClosePickerAssetCommand { get; set; }
        public ICommand OpenClosePickerLocationCommand { get; set; }

        public ObservableCollection<string> ListDomain { get; set; }

        public bool pickerIsOpen { get; set; }
        public bool isloading { get; set; }
        public bool isasset { get; set; }
        public string selecteddomain { get; set; }
        public int heightassetspec { get; set; }
        public int heightlocationspec { get; set; }
        public int assetspeccount { get; set; }
        public int locationspeccount { get; set; }
        public string assettitle { get; set; }
        public string locationtitle { get; set; }

        public AssetSpec selectedassetspec { get; set; }
        public LocationSpec selectedlocationspec { get; set; }

        public WorkorderspecViewModel(Workorder wo, INavigation navigation)
        {
            workorder = wo;
            Navigation = navigation;
            SaveCommand = new Command(Save);
            _ = LoadWospec();
            OpenClosePickerAssetCommand = new Command(OpenClosePickerAsset);
            OpenClosePickerLocationCommand = new Command(OpenClosePickerLocation);
        }

        public async void OpenClosePickerAsset(object param)
        {
            pickerIsOpen = !pickerIsOpen;
            if (pickerIsOpen)
            {
                ListDomain = new ObservableCollection<string>();
                isasset = true;
                selectedassetspec = param as AssetSpec;
                List<MxDomain> domains = await sqldomain.GetMxDomainAsync(selectedassetspec.domainid);
                foreach (MxDomain domain in domains)
                {
                    ListDomain.Add(domain.value);
                }
            }
        }

        public async void OpenClosePickerLocation(object param)
        {
            pickerIsOpen = !pickerIsOpen;
            if (pickerIsOpen)
            {
                ListDomain = new ObservableCollection<string>();
                isasset = false;
                selectedlocationspec = param as LocationSpec;
                List<MxDomain> domains = await sqldomain.GetMxDomainAsync(selectedlocationspec.domainid);
                foreach (MxDomain domain in domains)
                {
                    ListDomain.Add(domain.value);
                }

            }
        }

        async Task LoadWospec()
        {
            isloading = false;
            UserDialogs.Instance.ShowLoading("Loading Data");
            //Get Data asset
            workorder.asset = await sqlasset.GetAssetByWonum(workorder.wonum);

            //Get Data asset spec from sqlite
            workorder.assetspec = await sqlaspec.GetAssetSpecByWO(workorder.workorderid);
            foreach(AssetSpec aspec in workorder.assetspec)
            {
                if (aspec.pendingupload)
                {
                    aspec.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    aspec.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
                if (aspec.datatype == "NUMERIC")
                {
                    aspec.alnvalue = aspec.numvalue.ToString();
                }
            }
            assetspeccount = workorder.assetspec.Count;
            heightassetspec = assetspeccount * 80;
            assettitle = $"Asset : {workorder.assetnum} ({assetspeccount})";
            //Get Data location spec from sqlite
            workorder.locationspec = await sqllspec.GetLocationSpecByWO(workorder.workorderid);
            foreach (LocationSpec lspec in workorder.locationspec)
            {
                if (lspec.pendingupload)
                {
                    lspec.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    lspec.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
                if (lspec.datatype == "NUMERIC")
                {
                    lspec.alnvalue = lspec.numvalue.ToString();
                }
            }
            locationspeccount = workorder.locationspec.Count;
            heightlocationspec = locationspeccount * 80;
            locationtitle = $"Location : {workorder.location} ({locationspeccount})";
            UserDialogs.Instance.HideLoading();
            isloading = true;
        }

        async void Save()
        {
            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            if (check)
            {
                UserDialogs.Instance.ShowLoading("Uploading Workorder Spec...");

                if(workorder.asset.Count>0)
                await funUpdateAssets();
                await funUpdateLocation();
                UserDialogs.Instance.HideLoading();
            }
            else
            {
                foreach (AssetSpec aspec in workorder.assetspec)
                {
                    if (aspec.datatype == "NUMERIC")
                    {
                        if (String.IsNullOrEmpty(aspec.alnvalue))
                        {
                            aspec.numvalue = null;
                        }
                        else
                        {
                            aspec.numvalue = Convert.ToInt32(aspec.alnvalue);
                        }
                        aspec.alnvalue = null;
                    }
                    aspec.pendingupload = true;
                    await sqlaspec.UpdateAssetSpec(aspec);
                }
                

                foreach (LocationSpec lspec in workorder.locationspec)
                {
                    if (lspec.datatype == "NUMERIC")
                    {
                        if (String.IsNullOrEmpty(lspec.alnvalue))
                        {
                            lspec.numvalue = null;
                        }
                        else
                        {
                            lspec.numvalue = Convert.ToInt32(lspec.alnvalue);
                        }
                        lspec.alnvalue = null;
                    }
                    lspec.pendingupload = true;
                    await sqllspec.UpdateLocationSpec(lspec);
                }
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
            }
            await LoadWospec();
        }

        private async Task funUpdateAssets()
        {
            Asset upasset = workorder.asset[0];
            foreach (AssetSpec aspec in workorder.assetspec)
            {
                if (aspec.datatype == "NUMERIC")
                {
                    if (String.IsNullOrEmpty(aspec.alnvalue))
                    {
                        aspec.numvalue = null;
                    }
                    else
                    {
                        aspec.numvalue = Convert.ToInt32(aspec.alnvalue);
                    }
                    aspec.alnvalue = null;
                }
            }

            upasset.assetspec = workorder.assetspec;
            Asset updateasset = await maxrest.UpdateAsset(upasset);

            if (updateasset == null)
            {
                foreach (AssetSpec aspec in workorder.assetspec)
                {
                    aspec.pendingupload = true;
                    await sqlaspec.UpdateAssetSpec(aspec);
                }
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
                foreach (AssetSpec aspec in workorder.assetspec)
                {
                    aspec.pendingupload = false;
                    await sqlaspec.UpdateAssetSpec(aspec);
                }
                await UserDialogs.Instance.AlertAsync("Asset Data Successfully Updated", "Success!", "Ok");
            }
        }

        private async Task funUpdateLocation()
        {
            Locations uplocations = new Locations();
            foreach (LocationSpec lspec in workorder.locationspec)
            {
                uplocations.locationsid = lspec.locationsid;
                if (lspec.datatype == "NUMERIC")
                {
                    if (String.IsNullOrEmpty(lspec.alnvalue))
                    {
                        lspec.numvalue = null;
                    }
                    else
                    {
                        lspec.numvalue = Convert.ToInt32(lspec.alnvalue);
                    }
                    lspec.alnvalue = null;
                }
            }

            uplocations.locationspec = workorder.locationspec;
            Locations updatelocations = await maxrest.UpdateLocation(uplocations);
            if (updatelocations == null)
            {
                
                foreach (LocationSpec lspec in workorder.locationspec)
                {
                    lspec.pendingupload = true;
                    await sqllspec.UpdateLocationSpec(lspec);
                }
                await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
            }
            else if (updatelocations.Error != null)
            {
                await UserDialogs.Instance.AlertAsync(updatelocations.Error.message, "Failed!", "Ok");
                if (updatelocations.Error.reasonCode == "BMXAA9549E")
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
                foreach (LocationSpec lspec in workorder.locationspec)
                {
                    lspec.pendingupload = false;
                    await sqllspec.UpdateLocationSpec(lspec);
                }
                await UserDialogs.Instance.AlertAsync("Location Data Successfully Updated", "Success!", "Ok");
            }
        }
    }
}
