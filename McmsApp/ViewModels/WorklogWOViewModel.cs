using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using mcms.ApiServices;
using mcms.Models;
using mcms.Persistence;
using mcms.Views.Login;
using mcms.Views.Work.WorkDetail.Woklog;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.ViewModels
{
    public class WorklogWOViewModel : BaseViewModel
    {
        // Binding
        public Workorder workorder { get; set; }
        public string ButtonSaveTitle { get; set; }
        public Worklog selectedWorklog { get; set; }

        // Binding Form
        public string WorklogSummaryEntry { get; set; }
        public string WorklogDetailEntry { get; set; }
        public string WorklogTypeEntry { get; set; }
        public string subtitle { get; set; }
        public DateTime DateCreate { get; set; }
        public string CreatedBy { get; set; }
        public bool ClientViewable { get; set; }
        public bool NoWorklog { get; set; }

        public bool isErrorWorklogDetailEntry { get; set; }
        public string ErrorWorklogDetailEntryMessage { get; set; }

        public bool isErrorWorklogSummaryEntry { get; set; }
        public string ErrorWorklogSummaryEntryMessage { get; set; }

        public bool isErrorWorklogTypeEntry { get; set; }
        public string ErrorWorklogTypeEntryMessage { get; set; }

        // Binding
        public int WorklogListCount { get; set; }
        public bool pickerIsOpen { get; set; }
        public ObservableCollection<string> TypeList { get; set; }

        // Navigation From Parent
        INavigation Navigation { get; set; }

        // ICommand AddWorklog
        public ICommand AddWorklogCommand { get; set; }
        public ICommand SelectedWorklogWOCommand { get; set; }
        public ICommand BackPage { get; set; }
        public ICommand ChangeViewClient { get; set; }
        public ICommand SaveWorklogCommand { get; set; }
        public ICommand PickerActionCommand { get; set; }

        // SQLITE
        public IWorklog sqlWorklog = new SQLiteWorklog();
        //rest api
        IMaximoApi maxrest = new MaximoApi();

        public WorklogWOViewModel(Workorder wo, INavigation navigation)
        {

            BackPage = new Command(backPage);
            ChangeViewClient = new Command(changeClientView);
            SaveWorklogCommand = new Command(saveFormAsync);
            PickerActionCommand = new Command(OpenPicker);
            TypeList = new ObservableCollection<string>();
            TypeList.Add("PMOVAL");
            TypeList.Add("RECOCOM");
            TypeList.Add("RVWCOM");
            TypeList.Add("WORK");
            WorklogTypeEntry = "";
            WorklogDetailEntry = "";
            WorklogSummaryEntry = "";
            //oke
            workorder = wo;
            Navigation = navigation;
            AddWorklogCommand = new Command(addWorklog);
            SelectedWorklogWOCommand = new Command(editToPage);
            loadWorklog();
        }

        async void loadWorklog()
        {
            workorder.worklog = await sqlWorklog.GetWorklogByWonum(workorder.wonum);
            if (workorder.worklog.Count <1)
            {
                NoWorklog = true;
            }
            else
            {
                NoWorklog = false;
                foreach(Worklog wl in workorder.worklog)
                {
                    if (wl.pendingupload)
                    {
                        wl.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    }
                    else
                    {
                        wl.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    }
                }
                WorklogListCount = workorder.worklog.Count;
            }
        }

        async void addWorklog()
        {
            pickerIsOpen = false;
            selectedWorklog = new Worklog()
            {
                logtype = "WORK",
                recordkey = workorder.wonum,
                wonum = workorder.wonum,
                createby = workorder.username,
                createdate = DateTime.Now.ToLocalTime(),
                modifydate = DateTime.Now.ToLocalTime(),
                orgid = "TNBDN"
            };
            subtitle = "Add Worklog";
            await Navigation.PushModalAsync(new WorklogForm(this));
        }



        public void OpenPicker()
        {
            pickerIsOpen = !pickerIsOpen;
        }


        void changeClientView()
        {
            ClientViewable = !ClientViewable;
        }



        async void editToPage(object datas)
        {
            pickerIsOpen = false;
            SfListView selected = datas as SfListView;
            selectedWorklog = selected.SelectedItem as Worklog;
            selected.SelectedItem = null;
            subtitle = "Detail Worklog";
            await Navigation.PushModalAsync(new WorklogForm(this));
        }

        private async void backPage()
        {
            loadWorklog();
            await Navigation.PopModalAsync();
        }



        private async void saveFormAsync()
        {
            isErrorWorklogTypeEntry = isErrorWorklogDetailEntry = isErrorWorklogSummaryEntry = false;
            if (selectedWorklog.logtype == "")
            {
                isErrorWorklogTypeEntry = true;
                ErrorWorklogTypeEntryMessage = "Type cannot be empty!";
            }

            if (selectedWorklog.description == "")
            {
                isErrorWorklogSummaryEntry = true;
                ErrorWorklogSummaryEntryMessage = "Summary cannot be empty!";
            }

            if (selectedWorklog.tnbremarks == "")
            {
                isErrorWorklogDetailEntry = true;
                ErrorWorklogDetailEntryMessage = "Detail cannot be empty!";
            }


            if (!isErrorWorklogTypeEntry && !isErrorWorklogDetailEntry && !isErrorWorklogSummaryEntry)
            {
                UserDialogs.Instance.ShowLoading("Uploading Worklog...");
                switch (selectedWorklog.logtype)
                {
                    case "PMOVAL":
                        selectedWorklog.logtype_description = "PMO Validation";
                        break;
                    case "RECOCOM":
                        selectedWorklog.logtype_description = "Recomandation Comments";
                        break;
                    case "RVWCOM":
                        selectedWorklog.logtype_description = "Review";
                        break;
                    default:
                        selectedWorklog.logtype_description = "Work";
                        break;
                }

                await Task.Delay(1000);
                bool check = await maxrest.Ping();
                if (check)
                {
                    if (selectedWorklog.id == null || selectedWorklog.id == 0)
                    {
                        //Not yet Save to SQLITE
                        Workorder updatewo = new Workorder();
                        updatewo.worklog = new List<Worklog>();
                        updatewo.worklog.Add(selectedWorklog);
                        updatewo.workorderid = workorder.workorderid;
                        Workorder addworklog = await maxrest.UpdateWO(updatewo, "worklog");
                        UserDialogs.Instance.HideLoading();
                        if (addworklog == null)
                        {
                            selectedWorklog.pendingupload = true;
                            await sqlWorklog.AddWorklog(selectedWorklog);
                            await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                        }
                        else if (addworklog.Error != null)
                        {
                            await UserDialogs.Instance.AlertAsync(addworklog.Error.message, "Failed!", "Ok");
                            if (addworklog.Error.reasonCode == "BMXAA9549E")
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
                            List<Worklog> worklogs = await sqlWorklog.GetWorklogByWonum(workorder.wonum);
                            foreach (Worklog wl in worklogs)
                            {
                                await sqlWorklog.DeleteWorklog(wl);
                            }
                            foreach (Worklog wl in addworklog.worklog)
                            {
                                await sqlWorklog.AddWorklog(wl);
                            }
                            await UserDialogs.Instance.AlertAsync("Your Data Successfully Add", "Success!", "Ok");
                        }
                    }
                    else
                    {
                        Workorder updatewo = new Workorder();
                        updatewo.worklog = new List<Worklog>();
                        updatewo.worklog.Add(selectedWorklog);
                        updatewo.workorderid = workorder.workorderid;
                        Workorder updateworklog = await maxrest.UpdateWO(updatewo, "worklog");
                        UserDialogs.Instance.HideLoading();

                        if (updateworklog == null)
                        {
                            selectedWorklog.pendingupload = true;
                            await sqlWorklog.UpdateWorklog(selectedWorklog);
                            await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                        }
                        else if (updateworklog.Error != null)
                        {
                            await UserDialogs.Instance.AlertAsync(updateworklog.Error.message, "Failed!", "Ok");
                            if (updateworklog.Error.reasonCode == "BMXAA9549E")
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
                            List<Worklog> worklogs = await sqlWorklog.GetWorklogByWonum(workorder.wonum);
                            foreach (Worklog wl in worklogs)
                            {
                                await sqlWorklog.DeleteWorklog(wl);
                            }
                            foreach (Worklog wl in updateworklog.worklog)
                            {
                                await sqlWorklog.AddWorklog(wl);
                            }
                            await UserDialogs.Instance.AlertAsync("Your Data Successfully Add", "Success!", "Ok");
                        }
                        //Already Save to SQLITE
                    }

                }
                else
                {
                    if (selectedWorklog.id == null || selectedWorklog.id == 0)
                    {
                        //Not yet Save to SQLITE
                        workorder.worklog = new List<Worklog>();
                        workorder.worklog.Add(selectedWorklog);
                        UserDialogs.Instance.HideLoading();
                        selectedWorklog.pendingupload = true;
                        await sqlWorklog.AddWorklog(selectedWorklog);
                        await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                    }
                    else
                    {
                        workorder.worklog = new List<Worklog>();
                        workorder.worklog.Add(selectedWorklog);
                        UserDialogs.Instance.HideLoading();
                        selectedWorklog.pendingupload = true;
                        await sqlWorklog.UpdateWorklog(selectedWorklog);
                        await UserDialogs.Instance.AlertAsync("Your Data Already Save but not uploaded to Maximo", "Success!", "Ok");
                    }
                }

                loadWorklog();
                await Navigation.PopModalAsync();
            }

        }

    }
}
