using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using mcms.ApiServices;
using mcms.General;
using mcms.Models;
using mcms.Persistence;
using mcms.Views.Login;
using mcms.Views.Work.WorkDetail.Testing;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.ViewModels
{
    public class TestingMeterViewModel : BaseViewModel
    {
        IMaximoApi maxrest = new MaximoApi();
        ITnbwometer sqlmeter = new SQLiteTnbwometer();
        ITnbwometergroup sqlmetgroup = new SQLiteTnbwometergroup();
        IMxDomain sqldomain = new SQLiteMxDomain();

        public ICommand BackToCommand { get; set; }
        public ICommand SelectedMeterSpecificCommand { get; set; }
        public ICommand SelectedMeterGroupCommand { get; set; }
        public ICommand SaveMeterGroupCommand { get; set; }
        public ICommand OpenClosePickerCommand { get; set; }
        public ICommand SelectedPickerCommand { get; set; }


        public List<Tnbwometer> MeterListData { get; set; }
        public List<Tnbwometergroup> MeterGroupListData { get; private set; }
        public ObservableCollection<string> ListDomain { get; set; }

        public Tnbwometer selectedMeter { get; set; }
        public Tnbwometergroup selectedMeterGroup { get; set; }
        public bool pickerIsOpen { get; set; }
        public bool NoTesting { get; set; }
        public string selecteddomain { get; set; }
        public ProgressBarUpdate progressBarUpdate;

        INavigation Navigation;

        public Workorder workorder { get; set; }

        public TestingMeterViewModel(Workorder wo, INavigation navigation, ProgressBarUpdate progressBarUpdate)
        {
            workorder = wo;
            Navigation = navigation;
            BackToCommand = new Command(BackTo);
            SelectedMeterSpecificCommand = new Command(SelectedMeterSpecific);
            SelectedMeterGroupCommand = new Command(async (param)=> { await SelectedMeterGroup(param); });
            SaveMeterGroupCommand = new Command(SaveMeterReading);

            this.progressBarUpdate = progressBarUpdate;

            OpenClosePickerCommand = new Command(OpenClosePicker);
            _ = LoadMeterGroup();
        }

        public async void OpenClosePicker(object param)
        {
            pickerIsOpen = !pickerIsOpen;
            if (pickerIsOpen)
            {
                ListDomain = new ObservableCollection<string>();
                selectedMeter = param as Tnbwometer;
                List<MxDomain> domains = await sqldomain.GetMxDomainAsync(selectedMeter.domainid);
                foreach(MxDomain domain in domains)
                {
                    ListDomain.Add(domain.value);
                }

            }
        }

        void BackTo()
        {
            Navigation.PopAsync();
        }

        async void SelectedMeterSpecific(object obj)
        {
           
            SfListView selected = obj as SfListView;
            selectedMeter = selected.SelectedItem as Tnbwometer;
            selected.SelectedItem = null;
            await Navigation.PushAsync(new MeterSpecificDetail(this));
        }

        async Task LoadMeterGroup()
        {
            workorder.tnbwometergroup = await sqlmetgroup.GetTnbwometergroupByWO(workorder.wonum);

            workorder.tnbwometergroup = workorder.tnbwometergroup.OrderBy(x => x.tnbsequence).ToList();



            if (workorder.tnbwometergroup.Count < 1)
            {
                NoTesting = true;
            }
            else
            {
                NoTesting = false;
                foreach (Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                {
                    if (tnbwometergroup.pendingupload)
                    {
                        tnbwometergroup.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    }
                    else
                    {
                        tnbwometergroup.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    }
                    tnbwometergroup.tnbwometers = await sqlmeter.GetTnbwometerByGroup(tnbwometergroup.tnbwometergroupid);
                    foreach(Tnbwometer tnbwometer in tnbwometergroup.tnbwometers)
                    {
                        tnbwometer.tnblastreadinginspector = workorder.username;
                    }
                }
            }
            
        }

        async Task SelectedMeterGroup(object obj)
        {
            try
            {
                SfListView selected = obj as SfListView;
                selectedMeterGroup = selected.SelectedItem as Tnbwometergroup;
                Debug.WriteLine("Total metergroups are : "+ selectedMeterGroup.tnbwometers.Count());
                selectedMeterGroup.tnbwometers = selectedMeterGroup.tnbwometers.OrderBy(x => x.tnbsequence).ToList();
               /* for (int i=0;i< selectedMeterGroup.tnbwometers.Count();i++)
                {

                    Debug.WriteLine("Sequence is : " + selectedMeterGroup.tnbwometers[i].tnbsequence);
                }*/





                selected.SelectedItem = null;

                await Navigation.PushAsync(new MeterSpecificList(this));
            }
            catch (Exception e){
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            
        }

        async void SaveMeterReading()
        {

           
            UserDialogs.Instance.ShowLoading("Uploading Data...");
            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            if (check)
            {
                Workorder updatewo = new Workorder();
                updatewo.workorderid = workorder.workorderid;
                updatewo.tnbwometergroup = new List<Tnbwometergroup>();
                updatewo.tnbwometergroup = workorder.tnbwometergroup;
                Workorder updwo = await maxrest.UpdateWO(updatewo, "tnbwometergroup");
                UserDialogs.Instance.HideLoading();
                if (updwo == null)
                {
                    foreach(Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                    {
                        tnbwometergroup.pendingupload = true;
                        await sqlmetgroup.UpdateTnbwometergroup(tnbwometergroup);
                        foreach(Tnbwometer tnbwometer in tnbwometergroup.tnbwometers)
                        {
                            await sqlmeter.UpdateTnbwometer(tnbwometer);
                        }
                    }
                    await progressBarUpdate.LoadProgress(workorder);
                    await LoadMeterGroup();
                    await UserDialogs.Instance.AlertAsync("Update Testing Success but not upload to maximo", "Success", "Ok");
                }
                else if (updwo.Error != null)
                {
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
                    foreach (Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                    {
                        tnbwometergroup.pendingupload = false;
                        await sqlmetgroup.UpdateTnbwometergroup(tnbwometergroup);
                        foreach (Tnbwometer tnbwometer in tnbwometergroup.tnbwometers)
                        {
                            await sqlmeter.UpdateTnbwometer(tnbwometer);
                        }
                    }
                    await progressBarUpdate.LoadProgress(workorder);
                    await LoadMeterGroup();
                    await UserDialogs.Instance.AlertAsync("Update Testing Meter Success", "Success", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                foreach (Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                {
                    tnbwometergroup.pendingupload = true;
                    await sqlmetgroup.UpdateTnbwometergroup(tnbwometergroup);
                    foreach (Tnbwometer tnbwometer in tnbwometergroup.tnbwometers)
                    {
                        await sqlmeter.UpdateTnbwometer(tnbwometer);
                    }
                }
                await progressBarUpdate.LoadProgress(workorder);
                await LoadMeterGroup();
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("Update Testing Meter Success but not upload to maximo", "Success", "Ok");
            }

            
        }
    }
}
