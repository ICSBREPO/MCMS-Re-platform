using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Home;
using McmsApp.Views.Work.WorkDetail;
using Syncfusion.Maui.ListView;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using McmsApp.Views.Login;
using McmsApp.Views.Work;
using Syncfusion.Maui.TabView;

namespace McmsApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        //Rest Api Service
        IMaximoApi maxrest = new MaximoApi();

        // SQLite 
        IWorkorder sqlwo = new SQLiteWorkorder();
        IUserProfile sqlprofile = new SQLiteUserprofile();
        IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        IPlusgaudline sqlplusgaud = new SQLitePlusgaudline();
        IMxDomain sqlmx = new SQLiteMxDomain();
        ICompanies sqlcomp = new SQLiteCompanies();
        IChecklistVal sqlvalue = new SQLiteTnbchecklistvalue();

        // Command Action
        public ICommand AddFavouriteCommand { get; private set; }
        public ICommand FilterViewCommand { get; private set; }
        public ICommand DownloadMasterDataCommand { get; private set; }
        public ICommand SelectedWOCommand { get; private set; }

        // Store data
        ObservableCollection<Workorder> wolistdata = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> WOListData { get { return wolistdata; } }

        ObservableCollection<Workorder> wodummy = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> WOListDummy { get { return wodummy; } }

        ObservableCollection<Workorder> wolistdataChild = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> WOListDataChild { get { return wolistdataChild; } }

        ObservableCollection<Workorder> wolistdataappr = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> WOListDataAppr { get { return wolistdataappr; } }

        ObservableCollection<WOChart> typedata = new ObservableCollection<WOChart>();
        public ObservableCollection<WOChart> WOCharts { get { return typedata; } }


        // Binding Custom
        public string Username { get; set; }
        public string displayName { get; set; }
        public string Country { get; set; }
        public int wocount { get; set; }
        public int Childcount { get; set; }
        public int inprgcount { get; private set; }
        public int apprcount { get; set; }
        public bool IsRefreshing { get; set; }
        public bool noFav { get; set; }
        public bool yesFav { get; set; }
        public bool FilterVisible { get; set; }
        public bool WOStatus { get; private set; }
        public bool WorkType { get; private set; }
        public bool pickerIsOpen { get; set; }
        public string SelectedCharts { get; set; }
        public UserProfile UserBinding { get; set; }

        public ObservableCollection<string> TypeCharts { get; set; }

        // Navigation
        INavigation Navigation;
        HomePage homepage;
        WorkorderList wolistPage;

        public HomeViewModel(INavigation navigation, HomePage homePage)
        {
            homepage = homePage;
            wolistPage = new WorkorderList(navigation, homePage.tabview);
            Navigation = navigation;
            noFav = true;
            FilterVisible = false;
            

            TypeCharts = new ObservableCollection<string>();
            TypeCharts.Add("Status");
            TypeCharts.Add("Worktype");
            SelectedCharts = "Status";

            AddFavouriteCommand = new Command(async (param) => await AddFavourite(param));
            FilterViewCommand = new Command(FilterView);
            DownloadMasterDataCommand = new Command(async () => await DownloadMasterData());
            SelectedWOCommand = new Command(SelectedWO);
            _ = LoadUserProfile();
            _ = LoadListWo();
            _ = LoadChart();
            _ = LoadChild();
            _ = LoadAppr();
            LoadWODummy();
            _ = CheckWorkorderSync();
        }

        private async Task CheckWorkorderSync() {
            await wolistPage.woListViewModel.loadWOPending();
            await Task.Delay(2000);
            if(wolistPage.woListViewModel.wopendingcount>0)
            {
                bool answer = await UserDialogs.Instance.ConfirmAsync("You Have some pending workorder, Please click yes to check it", "Attention", "Yes", "No");
                if (answer)
                {
                    SfTabItem tabItem = homepage.tabview.Items[1];
                    wolistPage.woListViewModel.issync = true;
                    tabItem.Content = new WorkorderSync(wolistPage.woListViewModel).Content;
                    homepage.tabview.SelectedIndex = 1;
                }
            }
        }


        void LoadWODummy()
        {
            WOListDummy.Add(new Workorder
            {
                wonum = "1"
            });
            WOListDummy.Add(new Workorder
            {
                wonum = "2"
            });
            WOListDummy.Add(new Workorder
            {
                wonum = "3"
            });
            WOListDummy.Add(new Workorder
            {
                wonum = "4"
            });
        }

        async void SelectedWO(object obj)
        {
            SfListView selected = obj as SfListView;
            Workorder wo = selected.SelectedItem as Workorder;
            selected.SelectedItem = null;
            //SfTabItem tabItem = WOListPage.TabView.Items[1];
            //tabItem.Content = new WorkorderDetail(wo,WOListPage).Content;
            await Navigation.PushAsync(new TemplateWODetail(wo, Navigation, homepage.tabview));
        }


        void FilterView()
        {
            pickerIsOpen = true;
        }

        public async Task LoadListWo()
        {
            IsRefreshing = true;
            await Task.Delay(1000);
            try
            {
                WOListData.Clear();
                List<Workorder> workorders = await sqlwo.GetWorkorderAsync();
                foreach (Workorder wo in workorders)
                {
                    WOListData.Add(wo);
                    
                }
                wocount = WOListData.Count;
                IsRefreshing = false;
            }
            catch
            {
                IsRefreshing = false;
            }
        }
        public async Task LoadChild()
        {
            await Task.Delay(1000);
            try
            {
                WOListDataChild.Clear();
                List<Workorder> workorders = await sqlwo.GetWorkorderAsync();
                foreach (Workorder wo in workorders.Where(x=>x.parent!=null))
                {
                    WOListDataChild.Add(wo);

                }
                Childcount = WOListDataChild.Count;
            }
            catch
            {
            }
        }

        public async Task LoadAppr()
        {

            WOListDataAppr.Clear();
            List<Workorder> workorders = await sqlwo.GetWorkorderAppr();
            List<Workorder> woinprg = await sqlwo.GetWorkorderInprg();
            foreach (Workorder wo in workorders)
            {
                WOListDataAppr.Add(wo);
            }
            inprgcount = woinprg.Count();
            apprcount = WOListDataAppr.Count();
        }


        public async Task LoadUserProfile()
        {
            try
            {
                string username = await SecureStorage.GetAsync("username");
                UserBinding = await sqlprofile.GetUserProfileByUserName(username);
                if (UserBinding.picture == null || UserBinding.picture == "")
                {
                    UserBinding.picture = "dpDefault.png";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error  : {0}", ex.Message);
            }

        }

        async Task AddFavourite(object wo)
        {
            Workorder wodata = wo as Workorder;
            wodata.isFav = !wodata.isFav;
            await sqlwo.UpdateWorkorder(wodata);
        }

        public async Task LoadChart()
        {
            List<WOChart> chartdata = await sqlwo.GetWOChart(SelectedCharts.ToLower());
            WOCharts.Clear();
            foreach (WOChart wochart in chartdata)
            {
                WOCharts.Add(wochart);
            }
        }

        async Task DownloadMasterData()
        {
            var dlg = UserDialogs.Instance.Progress("Checking Connection..");
            dlg.PercentComplete = 0;
            try
            {
                bool check = await maxrest.Ping();
                if (check)
                {
                    dlg.Title = "Downloading Person Lists";
                    dlg.PercentComplete += 10;
                    ResPerson persons = await maxrest.GetPersons();
                    if (persons.member != null)
                    {
                        await sqlprofile.ResetUserProfile();
                        await sqlprofile.AddUserProfile(UserBinding);
                        foreach (UserProfile person in persons.member.Where(x=>x.personid!=UserBinding.personid))
                        {
                            person.lastupdate = DateTime.Now.ToLocalTime();
                            await sqlprofile.AddUserProfile(person);
                        }
                    }
                    dlg.Title = "Downloading SQA Template";
                    dlg.PercentComplete +=20;
                    ResSQA sqas = await maxrest.GetTemplateSQA();


                    // Expired token direct login
                    if (sqas.Error != null)
                    {
                        await UserDialogs.Instance.AlertAsync(sqas.Error.message, "Failed!", "Ok");
                        if (sqas.Error.reasonCode == "BMXAA9549E")
                        {
                            var hostname = await SecureStorage.GetAsync("Hostname");
                            SecureStorage.RemoveAll();
                            _ = SecureStorage.SetAsync("Hostname", hostname);
                            dlg.Dispose();
                            await Navigation.PushModalAsync(new LoginPage());
                            return;
                        }
                    }

                    if (sqas.member != null)
                    {
                        await sqlsqa.DeleteByTemplate(true);
                        await sqlplusgaud.DeleteByTemplate(true);
                        foreach (Plusgaudit sqa in sqas.member)
                        {
                            int id = await sqlsqa.AddPlusgaudit(sqa);
                            foreach (Plusgaudline plusgaudline in sqa.plusgaudline)
                            {
                                plusgaudline.sqaid = id;
                                plusgaudline.template = true;
                                await sqlplusgaud.AddPlusgaudline(plusgaudline);
                            }
                        }
                    }
                    dlg.Title = "Downloading Domain Lookups";
                    dlg.PercentComplete += 30;

                    MaxDomains memberDom = await maxrest.GetDomain();

                    if (memberDom.member != null)
                    {
                        await sqlmx.ResetMxDomain();
                        foreach (MemberDom domain in memberDom.member)
                        {
                            if (domain.domaintype == "ALN" && domain.alndomain != null)
                            {
                                foreach (MxDomain dom in domain.alndomain)
                                {
                                    dom.domainid = domain.domainid;
                                    await sqlmx.AddMxDomain(dom);
                                }
                            }
                            else if (domain.domaintype == "SYNONYM" && domain.synonymdomain != null)
                            {
                                foreach (MxDomain dom in domain.synonymdomain)
                                {
                                    dom.domainid = domain.domainid;
                                    await sqlmx.AddMxDomain(dom);
                                }
                            }

                        }
                    }
                    dlg.Title = "Downloading Companies";
                    dlg.PercentComplete += 20;
                    ResCompanies memberComp = await maxrest.GetCompanies();
                    if (memberComp.member != null)
                    {
                        await sqlcomp.ResetCompanies(); ;
                        foreach (Companies comp in memberComp.member)
                        {
                            await sqlcomp.AddCompanies(comp);
                        }
                        System.Diagnostics.Debug.WriteLine("count comp"+memberComp.member.Count);
                    }

                    dlg.Title = "Downloading Checklist Value";
                    dlg.PercentComplete += 10;
                    Restnbchecklistval checklistval = await maxrest.GetChecklistValue();
                    if (checklistval.member != null)
                    {
                        await sqlvalue.ResetValue(); ;
                        foreach (Tnbchecklistvalue val in checklistval.member)
                        {
                            await sqlvalue.AddValue(val);
                        }
                        System.Diagnostics.Debug.WriteLine("count value" + checklistval.member.Count);
                    }


                    dlg.Title = "Downloading Complete";
                    dlg.PercentComplete += 10;
                    dlg.Dispose();
                    await UserDialogs.Instance.AlertAsync("Master Data Successfully Downloaded!", "Success", "Ok");
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync("Cannot connected to the server, please check your connetion!", "Failed", "Ok");
                    dlg.Dispose();
                }
                
            }
            catch(Exception e)
            {
                await UserDialogs.Instance.AlertAsync("Download Master Data is Failed, Please Check your internet connection : "+e.Message , "Failed", "Ok");
                dlg.Dispose();
            }           
        }
    }

}
