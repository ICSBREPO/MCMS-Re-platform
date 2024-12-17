using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Themes;
using McmsApp.Views.Login;
using McmsApp.Views.PopupPages;
using McmsApp.Views.Work;
using McmsApp.Views.Work.WorkDetail;
using Newtonsoft.Json;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.TabView;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class WoListViewModel : BaseViewModel
    {
        //Rest Api Service
        IMaximoApi maxrest = new MaximoApi();

        //SQLite 
        IWorkorder sqlwo = new SQLiteWorkorder();
        ITnbwometergroup sqlmetergroup = new SQLiteTnbwometergroup();
        ITnbwometer sqlmeter = new SQLiteTnbwometer();
        IPermitWork sqlptw = new SQLitePermitWork();
        IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        IPlusgaudline sqlsqaquestions = new SQLitePlusgaudline();
        ITnbwochecklist sqlchecklist = new SQLiteTnbwochecklist();
        ITnbwochecklisttype sqlchecklisttype = new SQLiteTnbwochecklisttype();
        ITnbpsiwochecklisttype sqlpsichecklisttype = new SQLiteTnbpsiwochecklisttype();
        ITnbwochecklisthdr sqlchecklisthdr = new SQLiteTnbwochecklisthdr();
        IDoclinks sqldocs = new SQLiteDoclinks();
        IWorklog sqlwl = new SQLiteWorklog();
        IMxDomain sqldomain = new SQLiteMxDomain();
        IWorkorderspec sqlwospec = new SQLiteWorkorderspec();
        IAsset sqlasset = new SQLiteAsset();
        IAssetSpec sqlassetspec = new SQLiteAssetSpec();
        ILocations sqlocations = new SQLiteLocations();
        ILocationSpec sqllocationspec = new SQLiteLocationSpec();
        ITnbwochecklistsignature sqlsign = new SQLiteTnbwochecklistsignature();
        ITnbsignature sqlsigns = new SQLiteTnbsignature();
        ITnbmetergroup sqlmetgroup = new SQLiteTnbmetergroup();

        //Command Action
        public ICommand SyncWOCommand { get; private set; }
        public ICommand LoadWOListCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }
        public ICommand ApplyFilterCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }
        public ICommand FilterViewCommand { get; private set; }
        public ICommand AddFavouriteCommand { get; private set; }
        public ICommand DownloadWOCommand { get; set; }
        public ICommand OpenDownloadWOCommand { get; set; }
        public ICommand SelectedWOCommand { get; private set; }
        public ICommand MapNavCommand { get; private set; }
        public ICommand GoToChildWorkorder { get; set; }
        public ICommand BackToWOListCommand { get; set; }
        public ICommand SyncWorkorderCommand { get; set; }
        public ICommand UploadWOCommand { get; set; }

        //Binding Custom
        public int wocount { get; set; }
        public bool IsRefreshing { get; set; }
        public bool FilterVisible { get; set; }
        public bool Newest { get; set; }
        public bool Oldest { get; set; }
        public bool yesFav { get; set; }
        public bool noFav { get; set; }
        public bool ismap { get; set; }
        public bool issync { get; set; }
        public bool checkedall { get; set; }
        public bool popUpDownload { get; set; }
        public bool isdownloadall { get; set; }
        public bool isdownloadwonum { get; set; }
        public string textColor { get; set; }
        public string wonumdownload { get; set; }

        //Store Workorder List Data
        ObservableCollection<Workorder> wolistdata = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> WOListData { get { return wolistdata; } }

        ObservableCollection<Distinct> favour = new ObservableCollection<Distinct>();
        public ObservableCollection<Distinct> Favourite { get { return favour; } }

        ObservableCollection<Workorder> pendingwos = new ObservableCollection<Workorder>();
        public ObservableCollection<Workorder> ListPendingWO { get { return pendingwos; } }

        List<string> parents { get; set; }
        public bool syncButton { get; set; }

        // Filter Attribute Chip
        public List<Distinct> WOStatus { get; set; }
        public List<Distinct> WorkScope { get; set; }
        public List<Distinct> WorkType { get; set; }
        public List<Distinct> Vertical { get; set; }
        public List<Distinct> SubVertical { get; set; }

        // Selected Filter
        public ObservableCollection<Distinct> SelectedWOStatus { get; set; }
        public ObservableCollection<Distinct> SelectedWorkScope { get; set; }
        public ObservableCollection<Distinct> SelectedWorkType { get; set; }
        public ObservableCollection<Distinct> SelectedVertical { get; set; }
        public ObservableCollection<Distinct> SelectedSubVertical { get; set; }

        public ObservableCollection<Distinct> SelectList { get; set; }
        public int wopendingcount { get; private set; }


        //Store Navigation from Parent
        INavigation Navigation;
        WorkorderList WOListPage;
        WorkorderMap WOMapPage;

        public WoListViewModel(INavigation navigation, WorkorderList wolistpage)
        {

            WOListPage = wolistpage;
            SelectedWOStatus = new ObservableCollection<Distinct>();
            SelectedWorkType = new ObservableCollection<Distinct>();
            SelectedWorkScope = new ObservableCollection<Distinct>();
            SelectedVertical = new ObservableCollection<Distinct>();
            SelectedSubVertical = new ObservableCollection<Distinct>();
            SelectList = new ObservableCollection<Distinct>();
            WOMapPage = new WorkorderMap(this);
            FilterVisible = false;
            noFav = true;
            Newest = true;
            Navigation = navigation;
            FilterCommand = new Command(async () => await WoFilter());
            FilterViewCommand = new Command(async () => await FilterView());
            ApplyFilterCommand = new Command(async () => await ApplyFilter());
            ClearFilterCommand = new Command(async () => await ClearFilter());
            AddFavouriteCommand = new Command(async (param) => await AddFavourite(param));
            DownloadWOCommand = new Command(async () => await LoadAllWO());
            OpenDownloadWOCommand = new Command(async () => await OpenDownloadPageAsync());
            LoadWOListCommand = new Command(async () => await LoadListWo());
            SelectedWOCommand = new Command(SelectedWO);
            MapNavCommand = new Command(MapView);
            SyncWOCommand = new Command(goToSyncWorkorder);
            UploadWOCommand = new Command(UploadWO);
            _ = LoadListWo();
        }

        async void SelectedWO(object obj)
        {
            SfListView selected = obj as SfListView;
            Workorder wo = selected.SelectedItem as Workorder;
            selected.SelectedItem = null;
            //SfTabItem tabItem = WOListPage.TabView.Items[1];
            //tabItem.Content = new WorkorderDetail(wo,WOListPage).Content;
            await Navigation.PushAsync(new TemplateWODetail(wo, Navigation, WOListPage.TabView));
            // await Navigation.PushAsync(new TemplateWODetail(wo, Navigation));
        }

        void MapView()
        {
            ismap = !ismap;
            SfTabItem tabItem = WOListPage.TabView.Items[1];
            if (ismap)
            {
                tabItem.Content = WOMapPage.Content;
            }
            else
            {
                tabItem.Content = WOListPage.Content;
            }
        }

        async Task OpenDownloadPageAsync()
        {
            _ = loadWOPending();
            if (wopendingcount > 0)
            {
                bool answer = await UserDialogs.Instance.ConfirmAsync("You Have some pending workorder, Please click yes to check it first berfore to download data workorder", "Attention", "Yes", "No");
                if (answer)
                {
                    goToSyncWorkorder();
                }
                else
                {
                    isdownloadall = true;
                    isdownloadwonum = false;
                    popUpDownload = !popUpDownload;
                }
            }
            else
            {
                isdownloadall = true;
                isdownloadwonum = false;
                popUpDownload = !popUpDownload;
            }
        }

        private async Task LoadAllWO()
        {
            var dlg = UserDialogs.Instance.Progress("Downloading...");
            await Task.Delay(1000);
            try
            {
                await DownloadAction(dlg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task DownloadAction(IProgressDialog dlg)
        {
            bool ping = await maxrest.Ping();
            if (ping)
            {
                dlg.PercentComplete += 10;
                RespDownloadWorkorder allwo = new RespDownloadWorkorder();
                if (isdownloadall)
                {
                    allwo = await maxrest.GetWorkorder(null, true);
                }
                else
                {
                    allwo = await maxrest.GetWorkorder(null, false);
                }
                parents = new List<string>();
                await FetchDownload(allwo, true, dlg);
                //if (parents.Count > 0)
                //{
                //    string par = JsonConvert.SerializeObject(parents);
                //    RespDownloadWorkorder parentwo = await maxrest.GetParentWorkorder(par,false);
                //    if (isdownloadall)
                //    {
                //        parentwo = await maxrest.GetParentWorkorder(par, true);
                //    }
                //    await FetchDownload(parentwo,false,dlg);
                //}
                Console.WriteLine("Percent" + dlg.PercentComplete);
                await LoadListWo();
                await UserDialogs.Instance.AlertAsync("Workorder downloaded successfully", "Success", "Ok");
                dlg.Dispose();
            }
            else
            {
                await UserDialogs.Instance.AlertAsync("Please check your internet connections!", "Error", "Ok");
                dlg.Dispose();
            }
        }

        private async Task FetchDownload(RespDownloadWorkorder resp, bool isparent, IProgressDialog dlg)
        {
            string username = await SecureStorage.GetAsync("username");
            // Expired token direct login
            if (resp.Error != null)
            {
                await UserDialogs.Instance.AlertAsync(resp.Error.message, "Failed!", "Ok");
                if (resp.Error.reasonCode == "BMXAA9549E")
                {
                    dlg.Hide();
                    var hostname = await SecureStorage.GetAsync("Hostname");
                    SecureStorage.RemoveAll();
                    await SecureStorage.SetAsync("Hostname", hostname);
                    await Navigation.PushModalAsync(new LoginPage());
                    return;
                }
            }


            if (resp.member != null)
            {
                if (isparent)
                {
                    await sqlwo.ResetWorkorder();
                    await sqlmetergroup.ResetTnbwometergroup();
                    await sqlmeter.ResetTnbwometer();
                    await sqlsqaquestions.DeleteByTemplate(false);
                    await sqlsqa.DeleteByTemplate(false);
                    await sqlptw.ResetPermitWork();
                    await sqlchecklist.ResetTnbwochecklist();
                    await sqlchecklisttype.ResetTnbwochecklisttype();
                    await sqlpsichecklisttype.ResetTnbpsiwochecklisttype();
                    await sqlchecklisthdr.ResetTnbwochecklisthdr();
                    await sqldocs.ResetDoclinks();
                    await sqlwospec.ResetWorkorderspec();
                    await sqlwl.ResetWorklog();
                    await sqlasset.ResetAsset();
                    await sqlassetspec.ResetAssetSpec();
                    await sqlocations.ResetLocations();
                    await sqllocationspec.ResetLocationSpec();
                    await sqlsign.ResetTnbwochecklistsignature();
                    await sqlsigns.ResetTnbsignature();
                    await sqlmetgroup.ResetTnbmetergroup();
                }
                int pref = 0;
                int countwo = 0;
                if (isparent)
                {
                    countwo = resp.member.Count;
                    if (countwo == 0)
                    {
                        dlg.PercentComplete += 80;
                    }
                    else
                    {
                        pref = 90 / countwo;
                    }
                }
                else
                {
                    countwo = resp.member.Count;
                    if (countwo == 0)
                    {
                        dlg.PercentComplete += 10;
                    }
                    else
                    {
                        pref = 10 / countwo;
                    }
                }

                foreach (Workorder wo in resp.member)
                {
                    System.Diagnostics.Debug.WriteLine("WO" + wo.wonum);
                    if (wo.parent != null && isparent == true)
                    {
                        if (parents.Where(x => x == wo.parent).Count() == 0)
                        {
                            parents.Add(wo.parent);
                        }
                    }
                    if(wo.statusdate != null)
                    {
                        DateTime statsdate = Convert.ToDateTime(wo.statusdate);
                        statsdate = statsdate.ToLocalTime();
                        wo.statusdate = statsdate;
                    }

                    if (wo.schedstart != null)
                    {
                        DateTime schedstart = Convert.ToDateTime(wo.schedstart);
                        schedstart = schedstart.ToLocalTime();
                        wo.schedstart = schedstart;
                    }

                    if (wo.schedfinish != null)
                    {
                        DateTime schedfinish = Convert.ToDateTime(wo.schedfinish);
                        schedfinish = schedfinish.ToLocalTime();
                        wo.schedfinish = schedfinish;
                    }

                    if (wo.changedate != null)
                    {
                        DateTime changedate = Convert.ToDateTime(wo.changedate);
                        changedate = changedate.ToLocalTime();
                        wo.changedate = changedate;
                    }

                    if(wo.reportdate != null)
                    {
                        DateTime reportdate = Convert.ToDateTime(wo.reportdate);
                        reportdate = reportdate.ToLocalTime();
                        wo.reportdate = reportdate;
                    }


                    wo.username = username;
                    int woid = await sqlwo.AddWorkorder(wo);                    
                    //end workorder

                    //start save doclinks wo
                    if (wo.doclinks != null)
                    {
                        foreach (Doclinks doc in wo.doclinks)
                        {
                            doc.refid = woid;
                            doc.username = username;

                            if(doc.createdate != null)
                            {
                                DateTime createdate = Convert.ToDateTime(doc.createdate);
                                createdate = createdate.ToLocalTime();
                                doc.createdate = createdate;
                            }

                            if (doc.modified != null)
                            {
                                DateTime modified = Convert.ToDateTime(doc.modified);
                                modified = modified.ToLocalTime();
                                doc.createdate = modified;
                            }
                            
                            await sqldocs.AddDoclinks(doc);
                        }
                    }

                    //start save worklog wo
                    if (wo.worklog != null)
                    {
                        foreach (Worklog wl in wo.worklog)
                        {
                            if(wl.modifydate != null)
                            {
                                DateTime modifydate = Convert.ToDateTime(wl.modifydate);
                                modifydate = modifydate.ToLocalTime();
                                wl.modifydate = modifydate;
                            }
                            
                            if(wl.createdate != null)
                            {
                                DateTime createdate = Convert.ToDateTime(wl.createdate);
                                createdate = createdate.ToLocalTime();
                                wl.createdate = createdate;
                            }
                            
                            if(wl.tnbcompleteddate != null)
                            {
                                DateTime tnbcompleteddate = Convert.ToDateTime(wl.tnbcompleteddate);
                                tnbcompleteddate = tnbcompleteddate.ToLocalTime();
                                wl.tnbcompleteddate = tnbcompleteddate;
                            }

                            wl.recordkey = wo.wonum;
                            await sqlwl.AddWorklog(wl);
                        }
                    }

                    //start save wometer grup
                    if (wo.tnbwometergroup != null)
                    {
                        foreach (Tnbwometergroup wogroup in wo.tnbwometergroup)
                        {
                            foreach (Metergroup metergroup in wogroup.metergroup)
                            {
                                if (metergroup != null)
                                {
                                    wogroup.description = metergroup.description;
                                    await sqlmetgroup.AddTnbmetergroup(metergroup);
                                }
                                await sqlmetergroup.AddTnbwometergroup(wogroup);
                            }

                            //start save wometer
                            foreach (Tnbwometer wometer in wogroup.tnbwometers)
                            {
                                if (wometer.meter != null)
                                {
                                    Meter meter = wometer.meter[0];
                                    if (meter.metertype == "CHARACTERISTIC")
                                    {
                                        wometer.islookup = true;
                                        wometer.domainid = meter.domainid;
                                        if (meter.alndomain != null)
                                        {
                                            foreach (MxDomain domain in meter.alndomain)
                                            {
                                                await sqldomain.AddMxDomain(domain);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        wometer.measureunitid = meter.measureunitid;
                                    }
                                    wometer.meterdescription = meter.description;
                                    wometer.metertype = meter.metertype;
                                }
                                wometer.tnbwometergroupid = wogroup.tnbwometergroupid;
                                await sqlmeter.AddTnbwometer(wometer);
                            }
                        }
                    }

                    //start save sqa
                    if (wo.plusgaudit != null)
                    {
                        foreach (Plusgaudit sqa in wo.plusgaudit)
                        {
                            if(sqa.statusdate != null)
                            {
                                DateTime statusdate = Convert.ToDateTime(sqa.statusdate);
                                statusdate = statusdate.ToLocalTime();
                                sqa.statusdate = statusdate;
                            }
                            
                            if(sqa.createdbydate != null)
                            {
                                DateTime createdbydate = Convert.ToDateTime(sqa.createdbydate);
                                createdbydate = createdbydate.ToLocalTime();
                                sqa.createdbydate = createdbydate;
                            }

                           if(sqa.approvedbydate != null)
                            {
                                DateTime approvedbydate = Convert.ToDateTime(sqa.approvedbydate);
                                approvedbydate = approvedbydate.ToLocalTime();
                                sqa.approvedbydate = approvedbydate;
                            }

                            int id = await sqlsqa.AddPlusgaudit(sqa);
                            //start save doclinks sqa
                            if (sqa.doclinks != null)
                            {
                                foreach (Doclinks doc in sqa.doclinks)
                                {
                                    if(doc.createdate != null)
                                    {
                                        DateTime createdate = Convert.ToDateTime(doc.createdate);
                                        createdate = createdate.ToLocalTime();
                                        doc.createdate = createdate;
                                    }
                                   
                                    if(doc.modified != null)
                                    {
                                        DateTime modified = Convert.ToDateTime(doc.modified);
                                        modified = modified.ToLocalTime();
                                        doc.createdate = modified;
                                    }

                                    doc.refid = id;
                                    doc.username = username;
                                    Console.WriteLine(doc.document);
                                    await sqldocs.AddDoclinks(doc);
                                }
                            }

                            if (sqa.tnbsignature != null)
                            {
                                foreach (Tnbsignature sid in sqa.tnbsignature)
                                {
                                    if (sid._imagelibref != null)
                                    {
                                        sid.signature = sid.imglib[0].image;
                                    }

                                    if(sid.tnbsigdate != null)
                                    {
                                        DateTime tnbsigdate = Convert.ToDateTime(sid.tnbsigdate);
                                        tnbsigdate = tnbsigdate.ToLocalTime();
                                        sid.tnbsigdate = tnbsigdate;
                                    }
                                    
                                    if(sid.tnbexecdate != null)
                                    {
                                        DateTime tnbexecdate = Convert.ToDateTime(sid.tnbexecdate);
                                        tnbexecdate = tnbexecdate.ToLocalTime();
                                        sid.tnbexecdate = tnbexecdate;
                                    }                                    

                                    sid.refid = id;
                                    await sqlsigns.AddTnbsignature(sid);
                                }
                            }

                            if (sqa.plusgaudline != null)
                            {
                                //start save plusgaudline
                                foreach (Plusgaudline audline in sqa.plusgaudline)
                                {
                                    audline.sqaid = id;
                                    await sqlsqaquestions.AddPlusgaudline(audline);
                                }
                            }
                        }
                    }

                    //start save permitwork
                    if (wo.plusgpermitwork != null)
                    {
                        foreach (PermitWork permit in wo.plusgpermitwork)
                        {
                            permit.workorderid = wo.workorderid;
                            permit.tnbwonum = wo.wonum;

                            if(permit.createdate != null)
                            {
                                DateTime createdate = Convert.ToDateTime(permit.createdate);
                                createdate = createdate.ToLocalTime();
                                permit.createdate = createdate;
                            }
                            
                            if(permit.tnbissuedate != null)
                            {
                                DateTime tnbissuedate = Convert.ToDateTime(permit.tnbissuedate);
                                tnbissuedate = tnbissuedate.ToLocalTime();
                                permit.tnbissuedate = tnbissuedate;
                            }

                            if(permit.statusdate != null)
                            {
                                DateTime statusdate = Convert.ToDateTime(permit.statusdate);
                                statusdate = statusdate.ToLocalTime();
                                permit.statusdate = statusdate;
                            }

                            int id = await sqlptw.AddPermitWork(permit);
                            if (permit.doclinks != null)
                            {
                                foreach (Doclinks doc in permit.doclinks)
                                {
                                    if(doc.createdate != null)
                                    {
                                        DateTime createdatedoc = Convert.ToDateTime(doc.createdate);
                                        createdatedoc = createdatedoc.ToLocalTime();
                                        doc.createdate = createdatedoc;
                                    }
                                    
                                    if(doc.modified != null)
                                    {
                                        DateTime modified = Convert.ToDateTime(doc.modified);
                                        modified = modified.ToLocalTime();
                                        doc.createdate = modified;
                                    }                                    

                                    doc.refid = id;
                                    doc.username = username;
                                    Console.WriteLine(doc.document);
                                    await sqldocs.AddDoclinks(doc);
                                }
                            }
                        }
                    }

                    //start save asset
                    if (wo.asset != null)
                    {
                        foreach (Asset woasset in wo.asset)
                        {
                            if(woasset.tnbdeliverydate != null)
                            {
                                DateTime tnbdeliverydate = Convert.ToDateTime(woasset.tnbdeliverydate);
                                tnbdeliverydate = tnbdeliverydate.ToLocalTime();
                                woasset.tnbdeliverydate = tnbdeliverydate;
                            }
                            
                            if(woasset.tnbmanufdate != null)
                            {
                                DateTime tnbmanufdate = Convert.ToDateTime(woasset.tnbmanufdate);
                                tnbmanufdate = tnbmanufdate.ToLocalTime();
                                woasset.tnbmanufdate = tnbmanufdate;
                            }

                            woasset.wonum = wo.wonum;
                            await sqlasset.AddAsset(woasset);
                            //start save assetspec
                            if (woasset.assetspec != null)
                            {
                                foreach (AssetSpec aspec in woasset.assetspec)
                                {
                                    aspec.workorderid = wo.workorderid;
                                    if (aspec.assetattribute != null)
                                    {
                                        aspec.datatype = aspec.assetattribute[0].datatype;
                                        aspec.xamtype = aspec.datatype == "ALN" ? "Default" : "Numeric";
                                        aspec.description = aspec.assetattribute[0].description;
                                        aspec.domainid = aspec.assetattribute[0].domainid;
                                        if (aspec.assetattribute[0].alndomain != null)
                                        {
                                            aspec.islookup = true;
                                            foreach (MxDomain domain in aspec.assetattribute[0].alndomain)
                                            {
                                                await sqldomain.AddMxDomain(domain);
                                            }
                                        }
                                    }

                                    await sqlassetspec.AddAssetSpec(aspec);
                                }
                            }
                        }
                    }

                    //start save workorderspec
                    if (wo.workorderspec != null)
                    {
                        foreach (Workorderspec wospec in wo.workorderspec)
                        {
                            if(wospec.changedate != null)
                            {
                                DateTime changedatespec = Convert.ToDateTime(wospec.changedate);
                                changedatespec = changedatespec.ToLocalTime();
                                wospec.changedate = changedatespec;
                            }
                            await sqlwospec.AddWorkorderspec(wospec);
                        }
                    }

                    if (wo.locations != null)
                    {
                        foreach (Locations loc in wo.locations)
                        {
                            loc.wonum = wo.wonum;
                            await sqlocations.AddLocations(loc);
                            //start save locationspec
                            if (loc.locationspec != null)
                            {
                                foreach (LocationSpec lspec in loc.locationspec)
                                {
                                    lspec.workorderid = wo.workorderid;
                                    lspec.locationsid = loc.locationsid;
                                    if (lspec.assetattribute != null)
                                    {
                                        lspec.datatype = lspec.assetattribute[0].datatype;
                                        lspec.xamtype = lspec.datatype == "ALN" ? "Default" : "Numeric";
                                        lspec.description = lspec.assetattribute[0].description;
                                        lspec.domainid = lspec.assetattribute[0].domainid;
                                        if (lspec.assetattribute[0].alndomain != null)
                                        {
                                            lspec.islookup = true;
                                            foreach (MxDomain domain in lspec.assetattribute[0].alndomain)
                                            {
                                                await sqldomain.AddMxDomain(domain);
                                            }
                                        }
                                    }
                                    await sqllocationspec.AddLocationSpec(lspec);
                                }
                            }
                        }
                    }

                    if (wo.tnbpsiwochecklisttype != null)
                    {
                        //start save checklist type
                        foreach (Tnbpsiwochecklisttype tnbpsiwochecklisttype in wo.tnbpsiwochecklisttype)
                        {
                            await sqlpsichecklisttype.AddTnbpsiwochecklisttype(tnbpsiwochecklisttype);
                            if (tnbpsiwochecklisttype.tnbwochecklisttype != null)
                            {
                                foreach (Tnbwochecklisttype wochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                                {
                                    wochecklisttype.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                    await sqlchecklisttype.AddTnbwochecklisttype(wochecklisttype);
                                    //start save checklist
                                    if (wochecklisttype.tnbwochecklist != null)
                                    {
                                        foreach (Tnbwochecklist wochecklist in wochecklisttype.tnbwochecklist)
                                        {
                                            wochecklist.tnbwochecklisttypeid = wochecklisttype.tnbwochecklisttypeid;

                                            if(wochecklist.tnbdefectrectdate != null)
                                            {
                                                DateTime tnbdefectrectdate = Convert.ToDateTime(wochecklist.tnbdefectrectdate);
                                                tnbdefectrectdate = tnbdefectrectdate.ToLocalTime();
                                                wochecklist.tnbdefectrectdate = tnbdefectrectdate;
                                            }

                                            await sqlchecklist.AddTnbwochecklist(wochecklist);
                                        }
                                    }
                                    //start save signature
                                    if (wochecklisttype.tnbwochecklistsignature != null)
                                    {
                                        foreach (Tnbwochecklistsignature wochecklistsignature in wochecklisttype.tnbwochecklistsignature)
                                        {
                                            wochecklistsignature.tnbownerid = wochecklisttype.tnbwochecklisttypeid;

                                            if(wochecklistsignature.tnbsigneddate != null)
                                            {
                                                if(wochecklistsignature.tnbsigneddate != null)
                                                {
                                                    DateTime tnbsigneddate = Convert.ToDateTime(wochecklistsignature.tnbsigneddate);
                                                    tnbsigneddate = tnbsigneddate.ToLocalTime();
                                                    wochecklistsignature.tnbsigneddate = tnbsigneddate;
                                                }
                                                
                                            }

                                            if (wochecklistsignature.tnbexecdate != null)
                                            {
                                                DateTime tnbexecdate = Convert.ToDateTime(wochecklistsignature.tnbexecdate);
                                                tnbexecdate = tnbexecdate.ToLocalTime();
                                                wochecklistsignature.tnbexecdate = tnbexecdate;
                                            }

                                            if (wochecklistsignature._imagelibref != null)
                                            {
                                                if(wochecklistsignature.imglib != null)
                                                wochecklistsignature.signature = wochecklistsignature.imglib[0].image;
                                            }
                                            await sqlsign.AddTnbwochecklistsignature(wochecklistsignature);
                                        }
                                    }
                                }
                            }
                            if (tnbpsiwochecklisttype.tnbwochecklisthdr != null)
                            {
                                foreach (Tnbwochecklisthdr tnbwochecklisthdr in tnbpsiwochecklisttype.tnbwochecklisthdr)
                                {
                                    if (tnbwochecklisthdr.tnbprepareddate != null)
                                    {
                                        DateTime tnbprepareddate = Convert.ToDateTime(tnbwochecklisthdr.tnbprepareddate);
                                        tnbprepareddate = tnbprepareddate.ToLocalTime();
                                        tnbwochecklisthdr.tnbprepareddate = tnbprepareddate;
                                    }

                                    if (tnbwochecklisthdr.tnbvalidateddate != null)
                                    {
                                        DateTime tnbvalidateddate = Convert.ToDateTime(tnbwochecklisthdr.tnbvalidateddate);
                                        tnbvalidateddate = tnbvalidateddate.ToLocalTime();
                                        tnbwochecklisthdr.tnbvalidateddate = tnbvalidateddate;
                                    }

                                    if (tnbwochecklisthdr.tnbapproveddate != null)
                                    {
                                        DateTime tnbapproveddate = Convert.ToDateTime(tnbwochecklisthdr.tnbapproveddate);
                                        tnbapproveddate = tnbapproveddate.ToLocalTime();
                                        tnbwochecklisthdr.tnbapproveddate = tnbapproveddate;
                                    }

                                    if(tnbwochecklisthdr.tnbchangedate != null)
                                    {
                                        DateTime tnbchangedate = Convert.ToDateTime(tnbwochecklisthdr.tnbchangedate);
                                        tnbchangedate = tnbchangedate.ToLocalTime();
                                        tnbwochecklisthdr.tnbchangedate = tnbchangedate;
                                    }

                                    tnbwochecklisthdr.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                    tnbwochecklisthdr.tnbwonum = wo.wonum;
                                    await sqlchecklisthdr.AddTnbwochecklisthdr(tnbwochecklisthdr);
                                }
                            }
                            else
                            {
                                Tnbwochecklisthdr tnbwochecklisthdr = new Tnbwochecklisthdr();
                                tnbwochecklisthdr.tnbwochecklisthdrid = null;
                                tnbwochecklisthdr.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                tnbwochecklisthdr.tnbwonum = wo.wonum;
                                await sqlchecklisthdr.AddTnbwochecklisthdr(tnbwochecklisthdr);
                            }
                        }

                    }

                    dlg.PercentComplete += pref;
                }

            }
        }

        public async Task LoadListWo()
        {
            IsRefreshing = true;
            await Task.Delay(1000);
            try
            {
                syncButton = false;
                await loadWOPending();
                WOListData.Clear();
                List<Workorder> workorders = await sqlwo.GetWorkorderAsync();
                foreach (Workorder wo in workorders)
                {
                    int check = ListPendingWO.Where(x => x.wonum == wo.wonum).Count();
                    if (check > 0)
                    {
                        wo.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    }
                    else
                    {
                        wo.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    }
                    if (wo.parent != null)
                    {
                        wo.textColor = "#025498";
                        wo.textImage = "parentwo.png";
                    }
                    else
                    {
                        wo.textColor = "#09862B";
                        wo.textImage = "childwo.png";
                    }
                    wo.countwo = new List<CountWO>();
                    int? countattachment = await sqldocs.CountDoclinks(wo.id, "WORKORDER");
                    int? countsqa = await sqlsqa.CountSQA(wo.wonum);
                    int? countpermit = await sqlptw.CountPermit(wo.wonum);
                    int? countchecklist = await sqlpsichecklisttype.CountChecklist(wo.wonum);
                    int? countmeter = await sqlmetergroup.CountMeter(wo.wonum);
                    int? countworklog = await sqlwl.CountWorklog(wo.wonum);
                    int? countchild = await sqlwo.CountChild(wo.wonum);
                    int? countwospec = await sqlwospec.CountWOSpec(wo.workorderid);
                    if (countattachment != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_attachment.png",
                            label = "Attachment",
                            count = countattachment
                        });
                    }
                    if (countsqa != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_sqa.png",
                            label = "SQA",
                            count = countsqa
                        });
                    }
                    if (countpermit != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_permit.png",
                            label = "Permit",
                            count = countpermit
                        });
                    }
                    if (countchecklist != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_checklist.png",
                            label = "Checklist",
                            count = countchecklist
                        });
                    }
                    if (countmeter != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_testing.png",
                            label = "Testing Meter",
                            count = countmeter
                        });
                    }
                    if (countworklog != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "menu_log.png",
                            label = "Worklog",
                            count = countworklog
                        });
                    }

                    if (countchild != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "child.png",
                            label = "Child WO",
                            count = countchild
                        });
                    }

                    if (countwospec != 0)
                    {
                        wo.countwo.Add(new CountWO
                        {
                            woid = wo.id,
                            icon = "specification.png",
                            label = "Specification",
                            count = countwospec
                        });
                    }

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

        async Task FilterView()
        {
            await WoFilter();
            if (ismap)
            {
                WOMapPage.FilterView(true);
            }
            else
            {
                WOListPage.FilterView(true);
            }
        }

        async Task WoFilter()
        {
            SelectedWOStatus.Clear();
            SelectedWorkType.Clear();
            SelectedWorkScope.Clear();
            SelectedVertical.Clear();
            SelectedSubVertical.Clear();
            WOStatus = await sqlwo.GetListDistinct("status");
            WorkScope = await sqlwo.GetListDistinct("tnbworkscope");
            WorkType = await sqlwo.GetListDistinct("worktype");
            Vertical = await sqlwo.GetListDistinct("tnbvertical");
            SubVertical = await sqlwo.GetListDistinct("tnbsubvertical");

        }

        async Task ApplyFilter()
        {
            UserDialogs.Instance.ShowLoading("Loading...");
            string where = "where ";
            int check = 0;
            StringBuilder tmpsts = new StringBuilder();
            foreach (Distinct dis in SelectedWOStatus)
            {
                check++;
                if (SelectedWOStatus.Last() == dis)
                    tmpsts.Append($"'{dis.value}'");
                else
                    tmpsts.Append($"'{dis.value}',");
            }
            if (SelectedWOStatus.Count() > 0)
            {
                where = where + $"status in ({tmpsts}) and ";
            }
            StringBuilder tmpwt = new StringBuilder();
            foreach (Distinct dis in SelectedWorkType)
            {
                check++;
                if (SelectedWorkType.Last() == dis)
                    tmpwt.Append($"'{dis.value}'");
                else
                    tmpwt.Append($"'{dis.value}',");
            }
            if (SelectedWorkType.Count() > 0)
            {
                where = where + $"worktype in ({tmpwt}) and ";
            }
            StringBuilder tmpws = new StringBuilder();
            foreach (Distinct dis in SelectedWorkScope)
            {
                check++;
                if (SelectedWorkScope.Last() == dis)
                    tmpws.Append($"'{dis.value}'");
                else
                    tmpws.Append($"'{dis.value}',");
            }
            if (SelectedWorkScope.Count() > 0)
            {
                where = where + $"tnbworkscope in ({tmpws}) and ";
            }
            StringBuilder tmpv = new StringBuilder();
            foreach (Distinct dis in SelectedVertical)
            {
                check++;
                if (SelectedVertical.Last() == dis)
                    tmpv.Append($"'{dis.value}'");
                else
                    tmpv.Append($"'{dis.value}',");
            }
            if (SelectedVertical.Count() > 0)
            {
                where = where + $"tnbvertical in ({tmpv}) and ";
            }
            StringBuilder tmpsv = new StringBuilder();
            foreach (Distinct dis in SelectedSubVertical)
            {
                check++;
                if (SelectedSubVertical.Last() == dis)
                    tmpsv.Append($"'{dis.value}'");
                else
                    tmpsv.Append($"'{dis.value}',");
            }
            if (SelectedSubVertical.Count() > 0)
            {
                where = where + $"tnbsubvertical in ({tmpsv}) and ";
            }

            where = where + $"isFav={yesFav} ";

            if (Newest)
                where = where + $"order by statusdate desc";
            else
                where = where + $"order by statusdate asc";

            WOListData.Clear();
            List<Workorder> workorders = await sqlwo.GetFilterWorkorder(where);
            foreach (Workorder wo in workorders)
            {
                int pending = ListPendingWO.Where(x => x.wonum == wo.wonum).Count();
                if (pending > 0)
                {
                    wo.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    wo.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
                if (wo.parent != null)
                {
                    wo.textColor = "#025498";
                    wo.textImage = "parentwo.png";
                }
                else
                {
                    wo.textColor = "#09862B";
                    wo.textImage = "childwo.png";
                }
                WOListData.Add(wo);
            }
            wocount = WOListData.Count;
            WOListPage.FilterView(false);
            UserDialogs.Instance.HideLoading();
        }

        async Task ClearFilter()
        {
            SelectedWOStatus.Clear();
            SelectedWorkType.Clear();
            SelectedWorkScope.Clear();
            SelectedVertical.Clear();
            SelectedSubVertical.Clear();
            await LoadListWo();
        }

        async Task AddFavourite(object wo)
        {
            Workorder wodata = wo as Workorder;
            wodata.isFav = !wodata.isFav;
            await sqlwo.UpdateWorkorder(wodata);
        }


        async void goToSyncWorkorder()
        {
            issync = !issync;
            SfTabItem tabItem = WOListPage.TabView.Items[1];
            if (issync)
            {
                tabItem.Content = new WorkorderSync(this).Content;
                await loadWOPending();
            }
            else
            {
                tabItem.Content = WOListPage.Content;
                await LoadListWo();
            }
        }

        public async Task loadWOPending()
        {
            checkedall = false;
            ListPendingWO.Clear();
            List<Workorder> workorders = await sqlwo.GetWorkorderAsync();
            List<Tnbsignature> listsing = new List<Tnbsignature>();
            foreach (Workorder wo in workorders)
            {
                bool isinclude = false;

                //status workorder
                if(wo.pendingupdate == true)
                {
                    isinclude = true;
                }

                //sqa
                wo.plusgaudit = new List<Plusgaudit>();
                List<Plusgaudit> sqas = await sqlsqa.GetPlusgauditByWO(wo.wonum);

                foreach (Plusgaudit sqa in sqas.Where(x => x.pendingupload))
                {
                    isinclude = true;
                    if (sqa.auditnum == null)
                    {
                        sqa._action = "Add";
                    }

                    sqa.plusgaudline = new List<Plusgaudline>();
                    sqa.doclinks = new List<Doclinks>();
                    sqa.tnbsignature = new List<Tnbsignature>();
                    List<Plusgaudline> plusgaudlines = await sqlsqaquestions.GetPlusgaudlineBySQA(sqa.id);

                    foreach (Plusgaudline plusgaudline in plusgaudlines)
                    {
                        plusgaudline.linenum = null;
                        sqa.plusgaudline.Add(plusgaudline);
                    }

                    /*if(sqa.plusgauditid.HasValue)
                    {*/
                    List<Tnbsignature> tnbsignatures = await sqlsigns.GetTnbsignaturefromPlusgauditList(sqa.id, true);
                    if (tnbsignatures != null)
                        foreach (Tnbsignature tnbsignature in tnbsignatures.Where(x => x.pendingupdate))
                        {
                            Debug.WriteLine("Pending TnbLabel  : " + tnbsignature.tnbownerid);
                            sqa.tnbsignature.Add(tnbsignature);
                        }
                    /*}*/

                    List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(sqa.id, "PLUSGAUDIT");
                    foreach (Doclinks doc in doclinks.Where(x => x.docinfoid == null || x.docinfoid == 0))
                    {
                        sqa.doclinks.Add(doc);
                    }
                    List<Doclinks> doclinksdel = await sqldocs.GetDoclinksFilter(sqa.id, "PLUSGAUDIT", true);
                    foreach (Doclinks doc in doclinksdel)
                    {
                        sqa.doclinks.Add(doc);
                    }
                    wo.plusgaudit.Add(sqa);
                }

                //worklog
                wo.worklog = new List<Worklog>();
                List<Worklog> worklogs = await sqlwl.GetWorklogByWonum(wo.wonum);
                foreach (Worklog worklog in worklogs.Where(x => x.pendingupload))
                {
                    isinclude = true;
                    wo.worklog.Add(worklog);
                }

                //attachment
                wo.doclinks = new List<Doclinks>();
                List<Doclinks> doclinkswo = await sqldocs.GetDoclinksFilter(wo.id, "WORKORDER");
                foreach (Doclinks doc in doclinkswo.Where(x => x.docinfoid == null || x.docinfoid == 0))
                {
                    isinclude = true;
                    wo.doclinks.Add(doc);
                }
                List<Doclinks> doclinksdelwo = await sqldocs.GetDoclinksFilter(wo.id, "WORKORDER", true);
                foreach (Doclinks doc in doclinksdelwo)
                {
                    isinclude = true;
                    wo.doclinks.Add(doc);
                }
                //permit
                wo.plusgpermitwork = new List<PermitWork>();
                List<PermitWork> permitWorks = await sqlptw.GetPermitWorkWO(wo.wonum);
                foreach (PermitWork ptw in permitWorks.Where(x => x.pendingupload))
                {
                    isinclude = true;
                    ptw.doclinks = new List<Doclinks>();
                    List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(ptw.id, "PLUSGPERMITWORK");
                    foreach (Doclinks doc in doclinks.Where(x => x.docinfoid == null || x.docinfoid == 0))
                    {
                        Console.WriteLine("anjing" + doc.document);
                        if (ptw.plusgpermitworkid != null)
                        {
                            doc.ownerid = ptw.plusgpermitworkid;
                        }
                        ptw.doclinks.Add(doc);
                    }
                    List<Doclinks> doclinksdel = await sqldocs.GetDoclinksFilter(ptw.id, "PLUSGPERMITWORK", true);
                    foreach (Doclinks doc in doclinksdel)
                    {
                        ptw.doclinks.Add(doc);
                    }
                    wo.plusgpermitwork.Add(ptw);
                }
                //checklist
                wo.tnbpsiwochecklisttype = new List<Tnbpsiwochecklisttype>();
                List<Tnbpsiwochecklisttype> tnbpsiwochecklisttypes = await sqlpsichecklisttype.GetTnbpsiwochecklisttypeByWO(wo.wonum);
                foreach (Tnbpsiwochecklisttype tnbpsiwochecklisttype in tnbpsiwochecklisttypes)
                {
                    if(tnbpsiwochecklisttype.pendingupdate == true)
                    {
                        isinclude = true;
                        Console.WriteLine(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                    }

                    tnbpsiwochecklisttype.tnbwochecklisttype = await sqlchecklisttype.GetTnbwochecklisttypeByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                    foreach (Tnbwochecklisttype tnbwochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                    {
                        tnbwochecklisttype.tnbwochecklist = await sqlchecklist.GetTnbwochecklistByGroup(tnbwochecklisttype.tnbwochecklisttypeid);

                        tnbwochecklisttype.tnbwochecklistsignature = await sqlsign.GetTnbwochecklistsignatureByGroup(tnbwochecklisttype.tnbwochecklisttypeid);

                        foreach (Tnbwochecklistsignature tnbwochecklistsignature in tnbwochecklisttype.tnbwochecklistsignature.Where(x => x.pendingupdate))
                        {
                            isinclude = true;                                                     
                        }
                    }
                   // tnbpsiwochecklisttype.tnbwochecklisthdr = await sqlchecklisthdr.GetTnbwochecklisthdrByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                    wo.tnbpsiwochecklisttype.Add(tnbpsiwochecklisttype);
                }

                //testingmeter
                wo.tnbwometergroup = new List<Tnbwometergroup>();
                List<Tnbwometergroup> tnbwometergroups = await sqlmetergroup.GetTnbwometergroupByWO(wo.wonum);

                foreach (Tnbwometergroup tnbwometergroup in tnbwometergroups.Where(x => x.pendingupload))
                {
                    Debug.WriteLine("Meter in Metergroup : " + tnbwometergroup.tnbmetergroup.Count());
                    isinclude = true;

                    /* if (tnbwometergroup.tnbwometers != null)
                     {*/
                    Debug.WriteLine("Inside the tnbmeters Count is : " + tnbwometergroup.tnbmetergroup.Count());
                    tnbwometergroup.tnbwometers = await sqlmeter.GetTnbwometerByGroup(tnbwometergroup.tnbwometergroupid);
                    /*  }*/
                    wo.tnbwometergroup.Add(tnbwometergroup);
                }

                //Asset
                wo.asset = new List<Asset>();
                List<Asset> assets = await sqlasset.GetAssetByWonum(wo.wonum);
                foreach (Asset asset in assets.Where(x => x.pendingupload))
                {
                    isinclude = true;
                    wo.asset.Add(asset);
                }

                //Asset Spec
                wo.asset = new List<Asset>();
                List<Asset> asse = await sqlasset.GetAssetByWonum(wo.wonum);
                foreach (Asset asset in asse)
                {
                    List<AssetSpec> assetSpecs = await sqlassetspec.GetAssetSpecByWO(wo.workorderid);
                    foreach (AssetSpec assetSpec in assetSpecs.Where(x => x.pendingupload))
                    {
                        isinclude = true;
                        asset.assetspec = assetSpecs;
                    }
                    wo.asset.Add(asset);
                }

                //Locations Spec
                wo.locations = new List<Locations>();
                List<Locations> locations = await sqlocations.GetLocationsByWonum(wo.wonum);
                foreach (Locations loca in locations)
                {
                    List<LocationSpec> loc = await sqllocationspec.GetLocationSpecByWO(wo.workorderid);
                    foreach (LocationSpec spec in loc.Where(x => x.pendingupload))
                    {
                        isinclude = true;
                        loca.locationspec = loc;
                    }
                    wo.locations.Add(loca);
                }

                if (isinclude)
                {
                    syncButton = true;
                    ListPendingWO.Add(wo);
                }
            }
            wopendingcount = ListPendingWO.Count;
        }

        public void checkAll()
        {
            foreach (Workorder wo in ListPendingWO)
            {
                wo.checkedpending = checkedall;
            }
        }

        async void UploadWO()
        {
            bool ping = await maxrest.Ping();
            if (!ping)
            {
                await UserDialogs.Instance.AlertAsync("Please check your internet connection!", "Failed", "OK");
                return;
            }
            foreach (Workorder wo in ListPendingWO.Where(x => x.checkedpending))
            {
                //start save asset wo
                if (wo.asset != null)
                {
                    UserDialogs.Instance.ShowLoading("Uploading Specification and Asset...");
                    List<Asset> assets = wo.asset;
                    foreach (Asset asset in assets)
                    {
                        Asset ass = await maxrest.UpdateAsset(asset);

                        string jsonasset = JsonConvert.SerializeObject(
                            ass, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                            });

                        if (ass.Error != null)
                        {
                            UserDialogs.Instance.HideLoading();
                            await UserDialogs.Instance.AlertAsync(ass.Error.message, "Failed to update Specification and Asset!", "Ok");
                            if (ass.Error.reasonCode == "BMXAA9549E")
                            {
                                var hostname = await SecureStorage.GetAsync("Hostname");
                                SecureStorage.RemoveAll();
                                await SecureStorage.SetAsync("Hostname", hostname);
                                await Navigation.PushModalAsync(new LoginPage());
                                return;
                            }
                            wo.asset = null;
                        }
                        else
                        {
                            List<Asset> assett = await sqlasset.GetAssetByWonum(wo.wonum);
                            foreach (Asset asss in assett)
                            {
                                await sqlasset.DeleteAsset(asss);
                            }
                            List<AssetSpec> assetSpecs = await sqlassetspec.GetAssetSpecByWO(wo.workorderid);
                            foreach (AssetSpec spec in assetSpecs)
                            {
                                await sqlassetspec.DeleteAssetSpec(spec);
                            }

                            if (ass != null)
                            {
                                ass.wonum = wo.wonum;
                                await sqlasset.AddAsset(ass);

                                if (ass.assetspec != null)
                                {
                                    foreach (AssetSpec aspec in ass.assetspec)
                                    {
                                        aspec.workorderid = wo.workorderid;

                                        if (aspec.assetattribute != null)
                                        {
                                            aspec.datatype = aspec.assetattribute[0].datatype;
                                            aspec.xamtype = aspec.datatype == "ALN" ? "Default" : "Numeric";
                                            aspec.description = aspec.assetattribute[0].description;
                                            aspec.domainid = aspec.assetattribute[0].domainid;
                                            if (aspec.assetattribute[0].alndomain != null)
                                            {
                                                aspec.islookup = true;
                                                foreach (MxDomain domain in aspec.assetattribute[0].alndomain)
                                                {
                                                    await sqldomain.AddMxDomain(domain);
                                                }
                                            }
                                        }
                                        await sqlassetspec.AddAssetSpec(aspec);
                                        UserDialogs.Instance.HideLoading();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("BODOH!!!!");
                                }
                            }
                            wo.asset = null;
                        }
                    }
                }
                //End save asset wo

                //Start save location
                if (wo.locations != null)
                {
                    UserDialogs.Instance.ShowLoading("Uploading Locations Specification...");
                    List<Locations> locations = wo.locations;
                    foreach (Locations loc in locations)
                    {
                        Locations locs = await maxrest.UpdateLocation(loc);

                        string jsonlocations = JsonConvert.SerializeObject(
                            locs, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                            });

                        if (locs.Error != null)
                        {
                            UserDialogs.Instance.HideLoading();
                            await UserDialogs.Instance.AlertAsync(locs.Error.message, "Failed to update Locations Specification!", "Ok");
                            if (locs.Error.reasonCode == "BMXAA9549E")
                            {
                                var hostname = await SecureStorage.GetAsync("Hostname");
                                SecureStorage.RemoveAll();
                                await SecureStorage.SetAsync("Hostname", hostname);
                                await Navigation.PushModalAsync(new LoginPage());
                                return;
                            }
                            wo.locations = null;
                        }
                        else
                        {
                            List<Locations> locat = await sqlocations.GetLocationsByWonum(wo.wonum);
                            foreach (Locations locats in locat)
                            {
                                await sqlocations.DeleteLocations(locats);
                            }
                            List<LocationSpec> locationSpecs = await sqllocationspec.GetLocationSpecByWO(wo.workorderid);
                            foreach (LocationSpec spec in locationSpecs)
                            {
                                await sqllocationspec.DeleteLocationSpec(spec);
                            }

                            if (locs != null)
                            {
                                locs.wonum = wo.wonum;
                                await sqlocations.AddLocations(locs);

                                if (locs.locationspec != null)
                                {
                                    foreach (LocationSpec locationSpec in locs.locationspec)
                                    {
                                        locationSpec.workorderid = wo.workorderid;
                                        locationSpec.locationsid = locs.locationsid;
                                        if (locationSpec.assetattribute != null)
                                        {
                                            locationSpec.datatype = locationSpec.assetattribute[0].datatype;
                                            locationSpec.xamtype = locationSpec.datatype == "ALN" ? "Default" : "Numeric";
                                            locationSpec.description = locationSpec.assetattribute[0].description;
                                            locationSpec.domainid = locationSpec.assetattribute[0].domainid;
                                            if (locationSpec.assetattribute[0].alndomain != null)
                                            {
                                                locationSpec.islookup = true;
                                                foreach (MxDomain domain in locationSpec.assetattribute[0].alndomain)
                                                {
                                                    await sqldomain.AddMxDomain(domain);
                                                }
                                            }
                                        }
                                        await sqllocationspec.AddLocationSpec(locationSpec);
                                        UserDialogs.Instance.HideLoading();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("BODOH2!!!!");
                                }
                            }
                            wo.locations = null;
                        }
                    }
                }

                UserDialogs.Instance.ShowLoading("Uploading Workorder...");
                string doclink = "doclinks{doclinksid,urltype,changeby,document,createdate,description,changedate,ownerid,ownertable,docinfoid,doctype,createby,weburl,getlatestversion,printthrulink,urlname,reference}";
                Workorder resupdate = await maxrest.UpdateWO(wo, "wonum,description,status,statusdate,worktype,workorderid,tnbvertical,tnbsubvertical,tnbworkscope,tnbprojectphase,fcprojectid,fctaskid,tnbprojectnumber,parent,tnbvoltagelevel,tnbbusinessarea,tnbzone,tnbstate,tnbsubzone,tnbstation,tnbflsubstationtype,location,lead,siteid,tnbpsiwochecklisttype,assetnum,schedstart,schedfinish,locations{locationsid,description,locationspec},workorderspec,plusgaudit{*," + doclink + "},plusgpermitwork{*," + doclink + "},worklog,tnbwometergroup," + doclink);
                
                string json = JsonConvert.SerializeObject(
                resupdate, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });

                Debug.WriteLine("Data from Upload : " + json);
                if (resupdate == null)
                {
                    await UserDialogs.Instance.AlertAsync($"Workorder {wo.wonum} Fail to update! \n Connection time out!", "Failed", "OK");
                }
                else if (resupdate.Error != null)
                {
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync($"Workorder {wo.wonum} Fail to update! \n {resupdate.Error.message}", "Failed", "OK");
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
                    resupdate.username = wo.username;
                    resupdate.id = wo.id;
                    Debug.WriteLine("After saving Data : " + resupdate);
                    await sqlwo.UpdateWorkorder(resupdate);
                    //end workorder

                    //start save doclinks wo
                    List<Doclinks> doclinkswo = await sqldocs.GetDoclinksFilter(wo.id, "WORKORDER");
                    foreach (Doclinks doc in doclinkswo)
                    {
                        await sqldocs.DeleteDoclinks(doc);
                    }
                    if (resupdate.doclinks != null)
                    {
                        foreach (Doclinks doc in resupdate.doclinks)
                        {
                            doc.refid = resupdate.id;
                            doc.username = resupdate.username;
                            await sqldocs.AddDoclinks(doc);
                        }
                    }

                    //start save worklog wo
                    List<Worklog> worklogs = await sqlwl.GetWorklogByWonum(resupdate.wonum);
                    foreach (Worklog wl in worklogs)
                    {
                        await sqlwl.DeleteWorklog(wl);
                    }
                    if (resupdate.worklog != null)
                    {
                        foreach (Worklog wl in resupdate.worklog)
                        {
                            wl.recordkey = resupdate.wonum;
                            await sqlwl.AddWorklog(wl);
                        }
                    }

                    List<Locations> locations = await sqlocations.GetLocationsByWonum(resupdate.wonum);
                    foreach (Locations loc in locations)
                    {
                        await sqlocations.DeleteLocations(loc);
                    }
                    //start save locationspec wo
                    List<LocationSpec> locationSpecs = await sqllocationspec.GetLocationSpecByWO(resupdate.workorderid);
                    foreach (LocationSpec location in locationSpecs)
                    {
                        await sqllocationspec.DeleteLocationSpec(location);
                    }

                    if (resupdate.locations != null)
                    {
                        foreach (Locations loc in resupdate.locations)
                        {
                            loc.wonum = resupdate.wonum;
                            await sqlocations.AddLocations(loc);

                            if (loc.locationspec != null)
                            {
                                foreach (LocationSpec locationSpec in loc.locationspec)
                                {
                                    locationSpec.workorderid = resupdate.workorderid;
                                    locationSpec.locationsid = loc.locationsid;
                                    if (locationSpec.assetattribute != null)
                                    {
                                        locationSpec.datatype = locationSpec.assetattribute[0].datatype;
                                        locationSpec.xamtype = locationSpec.datatype == "ALN" ? "Default" : "Numeric";
                                        locationSpec.description = locationSpec.assetattribute[0].description;
                                        locationSpec.domainid = locationSpec.assetattribute[0].domainid;
                                        if (locationSpec.assetattribute[0].alndomain != null)
                                        {
                                            locationSpec.islookup = true;
                                            foreach (MxDomain domain in locationSpec.assetattribute[0].alndomain)
                                            {
                                                await sqldomain.AddMxDomain(domain);
                                            }
                                        }
                                    }
                                    await sqllocationspec.AddLocationSpec(locationSpec);
                                }
                            }
                            else
                            {
                                Console.WriteLine("BODOH2!!!!");
                            }
                        }
                    }

                    //start save wometer grup
                    List<Tnbwometergroup> tnbwometergroups = await sqlmetergroup.GetTnbwometergroupByWO(resupdate.wonum);
                    foreach (Tnbwometergroup wogroup in tnbwometergroups)
                    {
                        await sqlmetergroup.DeleteTnbwometergroup(wogroup);
                        //start save wometer
                        if (wogroup.tnbwometers != null)
                        {
                            foreach (Tnbwometer wometer in wogroup.tnbwometers)
                            {
                                await sqlmeter.DeleteTnbwometer(wometer);
                            }
                        }
                    }
                    if (resupdate.tnbwometergroup != null)
                    {
                        foreach (Tnbwometergroup wogroup in resupdate.tnbwometergroup)
                        {
                            foreach (Metergroup metergroup in wogroup.metergroup)
                            {
                                if (metergroup != null)
                                {
                                    wogroup.description = metergroup.description;
                                    await sqlmetgroup.AddTnbmetergroup(metergroup);
                                }
                                await sqlmetergroup.AddTnbwometergroup(wogroup);
                            }
                            //start save wometer
                            foreach (Tnbwometer wometer in wogroup.tnbwometers)
                            {
                                if (wometer.meter != null)
                                {
                                    Meter meter = wometer.meter[0];
                                    if (meter.metertype == "CHARACTERISTIC")
                                    {
                                        wometer.islookup = true;
                                        wometer.domainid = meter.domainid;
                                        if (meter.alndomain != null)
                                        {
                                            foreach (MxDomain domain in meter.alndomain)
                                            {
                                                await sqldomain.AddMxDomain(domain);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        wometer.measureunitid = meter.measureunitid;
                                    }
                                    wometer.meterdescription = meter.description;
                                    wometer.metertype = meter.metertype;
                                }
                                wometer.tnbwometergroupid = wogroup.tnbwometergroupid;
                                await sqlmeter.AddTnbwometer(wometer);
                            }
                        }
                    }

                    //start save sqa
                    List<Plusgaudit> plusgaudits = await sqlsqa.GetPlusgauditByWO(resupdate.wonum);
                    foreach (Plusgaudit sqa in plusgaudits)
                    {
                        await sqlsqa.DeletePlusgaudit(sqa);
                        //start save doclinks sqa
                        List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(sqa.id, "PLUSGAUDIT");
                        foreach (Doclinks doc in doclinks)
                        {
                            await sqldocs.DeleteDoclinks(doc);
                        }
                        List<Doclinks> doclinksdel = await sqldocs.GetDoclinksFilter(sqa.id, "PLUSGAUDIT", true);
                        foreach (Doclinks doc in doclinksdel)
                        {
                            await sqldocs.DeleteDoclinks(doc);
                        }
                        List<Plusgaudline> plusgaudlines = await sqlsqaquestions.GetPlusgaudlineBySQA(sqa.id);
                        //start save plusgaudline
                        foreach (Plusgaudline audline in plusgaudlines)
                        {
                            await sqlsqaquestions.DeletePlusgaudline(audline);
                        }

                        //start saving signature mycode


                        List<Tnbsignature> tnbsignatures;

                        if (sqa.plusgauditid.HasValue)
                        {
                            tnbsignatures = await sqlsigns.GetTnbsignaturefromPlusgauditList((int)sqa.plusgauditid, true);

                        }
                        else
                        {
                            tnbsignatures = await sqlsigns.GetTnbsignaturefromPlusgauditList((int)sqa.id, true);

                        }


                        Debug.WriteLine("Size of signatures" + tnbsignatures.Count);
                        foreach (Tnbsignature tnbsignature in tnbsignatures)
                        {
                            await sqlsigns.DeleteTnbsignature(tnbsignature);
                        }
                    }
                    if (resupdate.plusgaudit != null)
                    {
                        foreach (Plusgaudit sqa in resupdate.plusgaudit)
                        {
                            DateTime statsdate = Convert.ToDateTime(sqa.statusdate);
                            statsdate = statsdate.ToLocalTime();
                            sqa.statusdate = statsdate;

                            int id = await sqlsqa.AddPlusgaudit(sqa);
                            //start save doclinks sqa
                            if (sqa.doclinks != null)
                            {
                                foreach (Doclinks doc in sqa.doclinks)
                                {
                                    doc.refid = id;
                                    doc.username = resupdate.username;
                                    await sqldocs.AddDoclinks(doc);
                                }
                            }
                            if (sqa.plusgaudline != null)
                            {
                                //start save plusgaudline
                                foreach (Plusgaudline audline in sqa.plusgaudline)
                                {
                                    audline.sqaid = id;

                                    await sqlsqaquestions.AddPlusgaudline(audline);
                                }
                            }

                            //myadd
                            if (sqa.tnbsignature != null)
                            {

                                foreach (Tnbsignature tnbsignature in sqa.tnbsignature)
                                {
                                    DateTime signdate = Convert.ToDateTime(tnbsignature.tnbsigdate);
                                    signdate = signdate.ToLocalTime();
                                    tnbsignature.tnbsigdate = signdate;

                                    if (tnbsignature.imglib != null)
                                    {
                                        tnbsignature.signature = tnbsignature.imglib[0].image;
                                        Debug.WriteLine("Image Address is :  " + tnbsignature.imglib[0].image);
                                    }

                                    await sqlsigns.AddTnbsignature(tnbsignature);
                                }
                            }
                        }
                    }

                    //start save permitwork
                    List<PermitWork> permitWorks = await sqlptw.GetPermitWorkWO(resupdate.wonum);
                    foreach (PermitWork permit in permitWorks)
                    {
                        await sqlptw.DeletePermitWork(permit);
                        List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(permit.id, "PERMITWORK");
                        foreach (Doclinks doc in doclinks)
                        {
                            await sqldocs.DeleteDoclinks(doc);
                        }
                        List<Doclinks> doclinksdel = await sqldocs.GetDoclinksFilter(permit.id, "PERMITWORK", true);
                        foreach (Doclinks doc in doclinksdel)
                        {
                            await sqldocs.DeleteDoclinks(doc);
                        }
                    }
                    if (resupdate.plusgpermitwork != null)
                    {
                        foreach (PermitWork permit in resupdate.plusgpermitwork)
                        {
                            permit.workorderid = wo.workorderid;
                            permit.tnbwonum = wo.wonum;

                            DateTime statdate = Convert.ToDateTime(permit.statusdate);
                            statdate = statdate.ToLocalTime();
                            permit.statusdate = statdate;

                            int id = await sqlptw.AddPermitWork(permit);
                            if (permit.doclinks != null)
                            {
                                foreach (Doclinks doc in permit.doclinks)
                                {
                                    doc.refid = id;
                                    doc.username = resupdate.username;
                                    await sqldocs.AddDoclinks(doc);
                                }
                            }

                        }
                    }

                    //start save checklisthdr
                    List<Tnbpsiwochecklisttype> tnbpsiwochecklisttypes = await sqlpsichecklisttype.GetTnbpsiwochecklisttypeByWO(resupdate.wonum);
                    foreach (Tnbpsiwochecklisttype tnbpsiwochecklisttype in tnbpsiwochecklisttypes)
                    {
                        await sqlpsichecklisttype.DeleteTnbpsiwochecklisttype(tnbpsiwochecklisttype);
                        tnbpsiwochecklisttype.tnbwochecklisttype = await sqlchecklisttype.GetTnbwochecklisttypeByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                        if (tnbpsiwochecklisttype.tnbwochecklisttype != null)
                        {
                            foreach (Tnbwochecklisttype wochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                            {
                                await sqlchecklisttype.DeleteTnbwochecklisttype(wochecklisttype);
                                wochecklisttype.tnbwochecklist = await sqlchecklist.GetTnbwochecklistByGroup(wochecklisttype.tnbwochecklisttypeid);
                                if (wochecklisttype.tnbwochecklist != null)
                                {
                                    foreach (Tnbwochecklist wochecklist in wochecklisttype.tnbwochecklist)
                                    {
                                        await sqlchecklist.DeleteTnbwochecklist(wochecklist);
                                    }
                                }

                                wochecklisttype.tnbwochecklistsignature = await sqlsign.GetTnbwochecklistsignatureByGroup(wochecklisttype.tnbwochecklisttypeid);
                                if (wochecklisttype.tnbwochecklistsignature != null)
                                {
                                    foreach (Tnbwochecklistsignature tnbwochecklistsignature in wochecklisttype.tnbwochecklistsignature)
                                    {
                                        await sqlsign.DeleteTnbwochecklistsignature(tnbwochecklistsignature);
                                    }
                                }
                            }
                        }
                        tnbpsiwochecklisttype.tnbwochecklisthdr = await sqlchecklisthdr.GetTnbwochecklisthdrByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                        foreach (Tnbwochecklisthdr tnbwochecklisthdr in tnbpsiwochecklisttype.tnbwochecklisthdr)
                        {
                            await sqlchecklisthdr.DeleteTnbwochecklisthdr(tnbwochecklisthdr);
                        }
                    }
                    if (resupdate.tnbpsiwochecklisttype != null)
                    {
                        foreach (Tnbpsiwochecklisttype tnbpsiwochecklisttype in resupdate.tnbpsiwochecklisttype)
                        {
                            await sqlpsichecklisttype.AddTnbpsiwochecklisttype(tnbpsiwochecklisttype);
                            if (tnbpsiwochecklisttype.tnbwochecklisttype != null)
                            {
                                foreach (Tnbwochecklisttype wochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                                {
                                    wochecklisttype.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                    await sqlchecklisttype.AddTnbwochecklisttype(wochecklisttype);
                                    //start save checklist
                                    if (wochecklisttype.tnbwochecklist != null)
                                    {
                                        foreach (Tnbwochecklist wochecklist in wochecklisttype.tnbwochecklist)
                                        {
                                            wochecklist.tnbwochecklisttypeid = wochecklisttype.tnbwochecklisttypeid;
                                            await sqlchecklist.AddTnbwochecklist(wochecklist);
                                        }
                                    }
                                    //start save signature
                                   /* if (wochecklisttype.tnbwochecklistsignature != null)
                                    {
                                        foreach (Tnbwochecklistsignature wochecklistsignature in wochecklisttype.tnbwochecklistsignature)
                                        {
                                            wochecklistsignature.tnbownerid = wochecklisttype.tnbwochecklisttypeid;
                                            await sqlsign.AddTnbwochecklistsignature(wochecklistsignature);
                                        }*/


                                        //start save signature
                                        if (wochecklisttype.tnbwochecklistsignature != null)
                                        {
                                            foreach (Tnbwochecklistsignature wochecklistsignature in wochecklisttype.tnbwochecklistsignature)
                                            {
                                                wochecklistsignature.tnbownerid = wochecklisttype.tnbwochecklisttypeid;
                                                if (wochecklistsignature._imagelibref != null)
                                                {
                                                if(wochecklistsignature.imglib !=null)
                                                    wochecklistsignature.signature = wochecklistsignature.imglib[0].image;
                                                }
                                                await sqlsign.AddTnbwochecklistsignature(wochecklistsignature);
                                            }
                                        }
                                    /*}*/
                                }
                            }
                            if (tnbpsiwochecklisttype.tnbwochecklisthdr != null)
                            {
                                foreach (Tnbwochecklisthdr tnbwochecklisthdr in tnbpsiwochecklisttype.tnbwochecklisthdr)
                                {
                                    tnbwochecklisthdr.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                    tnbwochecklisthdr.tnbwonum = wo.wonum;
                                   
                                    await sqlchecklisthdr.AddTnbwochecklisthdr(tnbwochecklisthdr);
                                }
                            }
                            else
                            {
                                Tnbwochecklisthdr tnbwochecklisthdr = new Tnbwochecklisthdr();
                                tnbwochecklisthdr.tnbpsiwochecklisttypeid = tnbpsiwochecklisttype.tnbpsiwochecklisttypeid;
                                tnbwochecklisthdr.tnbwonum = wo.wonum;
                                
                                await sqlchecklisthdr.AddTnbwochecklisthdr(tnbwochecklisthdr);
                            }
                        }
                    }

                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync($"Workorder {resupdate.wonum} successfully updated!", "Success", "OK");
                }
            }
          loadWOPending();
        }

    }
}

