using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Login;
using McmsApp.Views.Work;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class ProjectDPMSViewModel : BaseViewModel
    {
        IAsset sqlasset = new SQLiteAsset();
        //rest api
        IMaximoApi maxrest = new MaximoApi();
        public Workorder workorder { get; set; }
        public Workorder dpmswo { get; set; }
        public int countasset { get; set; }
        INavigation Navigation { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand TryAgainCommand { get; set; }
        public bool nointernet { get; set; }
        public bool internet { get; set; }


        public ProjectDPMSViewModel(Workorder wo, INavigation navigation)
        {
            workorder = wo;
            Navigation = navigation;
            SaveCommand = new Command(Save);
            TryAgainCommand = new Command(async () => await LoadDPMS());
            Task task = LoadDPMS();
        }

        async Task LoadDPMS()
        {
            UserDialogs.Instance.ShowLoading("Load DPMS...");
            string select = "tnbprojectnumber,workorderid,wonum,tnbwodpms";
            Debug.WriteLine("lewat select");
            string where = $"tnbprojectnumber=%22{workorder.fcprojectid}%22 and tnbworkscope=%22PROJECT%22";
            Debug.WriteLine("lewat where"+where);
            await Task.Delay(500);
            bool check = await maxrest.Ping();
            if (check)
            {
                Debug.WriteLine("OKE PING");
                RespDownloadWorkorder resdownload = await maxrest.GetWorkorderQuery(select, where);
                if (resdownload.member != null)
                {
                    dpmswo = resdownload.member[0];
                    if (dpmswo.tnbwodpms.Count == 0)
                    {
                        dpmswo.tnbwodpms.Add(new TnbWODPMS
                        {
                            tnbwonum = workorder.wonum,
                            tnbwodpmsid = null
                        });
                    }
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync("DPMS data is not available", "Fail!", "Ok");
                }
                nointernet = false;
                internet = true;
            }
            else
            {
                nointernet = true;
                internet = false;
            }
           
            UserDialogs.Instance.HideLoading();
        }


        async void Save()
        {
            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            if (check)
            {
                UserDialogs.Instance.ShowLoading("Uploading DPMS...");
                Workorder resupdate = await maxrest.UpdateWO(dpmswo,"wodpms");
                UserDialogs.Instance.HideLoading();

                if (resupdate == null)
                {
                    await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                }
                else if (resupdate.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(resupdate.Error.message, "Failed!", "Ok");
                    if (resupdate.Error.reasonCode == "BMXAA9549E")
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
                    await UserDialogs.Instance.AlertAsync("Your Data Successfully Updated", "Success!", "Ok");
                }

            }
            else
            {
                await UserDialogs.Instance.AlertAsync("Check your internet connection", "Fail!", "Ok");
            }

        }
    }
}
