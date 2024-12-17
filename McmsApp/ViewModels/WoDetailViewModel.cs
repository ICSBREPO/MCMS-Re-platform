using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Login;
using McmsApp.Views.Work.WorkDetail;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using McmsApp.General;
using McmsApp.Views.Work;
using System.Diagnostics;

namespace McmsApp.ViewModels
{
    public class WoDetailViewModel : BaseViewModel
    {
        IMaximoApi maxrest = new MaximoApi();
        public bool IsBusy { get; set; }

        public IWorkorder sqlwo = new SQLiteWorkorder();
        public IAsset sqlasset = new SQLiteAsset();
        //Command
        public ICommand LoadDetailWOCommand { get; set; }
        public ICommand BackToWOListCommand { get; set; }
        public ICommand StatusCommand { get; set; }
        //public ICommand NewStatusCommand { get; set; }
        public ICommand NewStatusLookupCommand { get; set; }
        public ICommand SaveStatusCommand { get; set; }

        //Binding View
        public Workorder workorder { get; set; }
        public WoProgress woProgress { get; set; }
        public View WOContent { get; set; }
        public bool pickerIsOpen { get; set; }
        public string woasset { get; set; }
        public ProgressBarUpdate progressBarUpdate;

        //public ObservableCollection<string> StatusType { get; set; }
        public string SelectedNewStatus { get; set; }
        public string SelectedNewStatusDescription { get; set; }

        // pages
        INavigation Navigation { get; set; }

        public WoDetailViewModel(Workorder wo, INavigation navigation, ProgressBarUpdate progressBarUpdate) 
        {
            Navigation = navigation;
            workorder = wo;
            this.progressBarUpdate = progressBarUpdate;
            LoadDetailWOCommand = new Command(async () => await LoadWoData());
            _ = LoadWoData();
            //Status List
            /*StatusType = new ObservableCollection<string>();
            StatusType.Add("ASGN");
            StatusType.Add("INPRGS");
            StatusType.Add("WATINST");
            StatusType.Add("APINST");
            StatusType.Add("WTEST");
            StatusType.Add("TESTCOMP");
            StatusType.Add("WMPIAT");
            StatusType.Add("PIATFAIL");
            StatusType.Add("TECO");
            StatusType.Add("MPIATCOMP");
            StatusType.Add("COMP");*/
            //End Status List
            SelectedNewStatus = workorder.status;
            SelectedNewStatusDescription = workorder.status_description;

            BackToWOListCommand = new Command(BackToWOList);
            StatusCommand = new Command(ChangesStatus);
            //NewStatusCommand = new Command(StatusView);
            NewStatusLookupCommand = new Command(lookupStatus);
            SaveStatusCommand = new Command(SaveStatus);
        }

        /*void StatusView()
        {
            pickerIsOpen = true;
        }*/

        async void lookupStatus(object selected)
        {
            string select = selected as string;
            await Navigation.PushModalAsync(new LookupUserModal(this, "status", Navigation, select));
        }

        async void ChangesStatus()
        {
            await Navigation.PushAsync(new ChangeStatus(this));
        }

        async Task LoadWoData()
        {
            
                workorder = await sqlwo.GetWorkorder(workorder.id);
                workorder.asset = await sqlasset.GetAssetByWonum(workorder.wonum);
                if (workorder.asset.Count > 0)
                {
                    woasset = workorder.asset[0].assetnum + " " + workorder.asset[0].description;
                }
        }

        void BackToWOList()
        {
            Navigation.PopAsync();
        }

        async void SaveStatus()
        {
            

            UserDialogs.Instance.ShowLoading("Uploading Status...");
            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            if (check)
            {
                //start save action
                workorder.status = SelectedNewStatus;
                workorder.status_description = SelectedNewStatusDescription;
                Workorder updwo = await maxrest.UpdateWO(workorder, "workorderid,wonum,status");
                UserDialogs.Instance.HideLoading();
                if (updwo == null)
                {
                    workorder.pendingupdate = true;
                    await sqlwo.UpdateWorkorder(workorder);
                    await UserDialogs.Instance.AlertAsync("Success change status but not upload to maximo", "Success", "Ok");
                    await LoadWoData();
                    await Navigation.PopAsync();
                }
                else if (updwo.Error != null)
                {
                    workorder = await sqlwo.GetWorkorder(workorder.id);
                    SelectedNewStatus = workorder.status;
                    SelectedNewStatusDescription = workorder.status_description;
                    await UserDialogs.Instance.AlertAsync(updwo.Error.message, "Failed", "Ok");
                    if (updwo.Error.reasonCode == "BMXAA9549E")
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
                    await sqlwo.UpdateWorkorder(workorder);
                    await UserDialogs.Instance.AlertAsync("Success change status", "Success", "Ok");
                    await LoadWoData();
                    await Navigation.PopAsync();
                }
            }
            else
            {
                //start save action
                workorder.status = SelectedNewStatus;
                workorder.status_description = SelectedNewStatusDescription;
                if (workorder.id == null || workorder.id == 0)
                {
                    UserDialogs.Instance.HideLoading();
                    workorder.pendingupdate = true;
                    await sqlwo.UpdateWorkorder(workorder);
                    await UserDialogs.Instance.AlertAsync("Success change status but not upload to maximo", "Success", "Ok");
                    await LoadWoData();
                    await Navigation.PopAsync();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    workorder.pendingupdate = true;
                    await sqlwo.UpdateWorkorder(workorder);
                    await UserDialogs.Instance.AlertAsync("Success change status but not upload to maximo", "Success", "Ok");
                    await LoadWoData();
                    await Navigation.PopAsync();
                }
            }

            /*Global global = Global.Instance;
            global.isRefreshWorkDetail = true;*/

            //progress update
            Debug.WriteLine("Status Change is Called" + workorder.status);

            await progressBarUpdate.LoadProgress(workorder);
        }

       
    }
}