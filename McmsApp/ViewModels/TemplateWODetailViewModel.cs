using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using McmsApp.ApiServices;
using McmsApp.General;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Home;
using McmsApp.Views.Work.WorkDetail;
using McmsApp.Views.Work.WorkDetail.Attachment;
using McmsApp.Views.Work.WorkDetail.ChildWork;
using McmsApp.Views.Work.WorkDetail.Permit;
using McmsApp.Views.Work.WorkDetail.SQA;
using McmsApp.Views.Work.WorkDetail.Testform;
using McmsApp.Views.Work.WorkDetail.Testing;
using McmsApp.Views.Work.WorkDetail.Woklog;
using McmsApp.Views.Work.WorkDetail.Workorderspec;
using Syncfusion.Maui.ProgressBar;
using Syncfusion.Maui.TabView;

namespace McmsApp.ViewModels
{
    public class TemplateWODetailViewModel : BaseViewModel
    {
        public bool IsBusy { get; set; }

        public IWorkorder sqlwo = new SQLiteWorkorder();
        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        public IPlusgaudline sqlplusgaud = new SQLitePlusgaudline();
        public ITnbwometergroup sqlmetgroup = new SQLiteTnbwometergroup();
        public IDoclinks sqldoc = new SQLiteDoclinks();
        ITnbwometer sqlmeter = new SQLiteTnbwometer();
        IPermitWork sqlpermit = new SQLitePermitWork();
        public IWorklog sqlWorklog = new SQLiteWorklog();
        public IWorkorderspec sqlwospec = new SQLiteWorkorderspec();
        public IAssetSpec sqlaspec = new SQLiteAssetSpec();
        public ILocationSpec sqllspec = new SQLiteLocationSpec();
        public IAsset sqlasset = new SQLiteAsset();
        ITnbwochecklisthdr sqlhdr = new SQLiteTnbwochecklisthdr();
        ITnbpsiwochecklisttype sqlpsi = new SQLiteTnbpsiwochecklisttype();
        public Tnbwochecklisthdr wochecklisthdr { get; set; }

        // Step Progress Bar
        public StepStatus prsqa { get; set; }
        public StepStatus prgps { get; set; }
        public StepStatus prinprg { get; set; }
        public StepStatus prte { get; set; }
        public StepStatus prphotos { get; set; }
        public StepStatus prcomp { get; set; }
        public ImageSource Iprsqa { get; internal set; }
        public ImageSource Iprgps { get; internal set; }
        public ImageSource Iprinprg { get; internal set; }
        public ImageSource Iprte { get; internal set; }
        public ImageSource Iprphotos { get; internal set; }
        public ImageSource Iprcomp { get; internal set; }

        // Binding UI View
        public Workorder workorder { get; set; }
        public WoProgress woProgress { get; set; }
        public View WOContent { get; set; }
        public DataLabel datalabel { get; set; }
        public Tnbwometer tnbwometer { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public int totaldoclink { get; set; }


        ObservableCollection<SubmenuInfo> submenuList = new ObservableCollection<SubmenuInfo>();
        public ObservableCollection<SubmenuInfo> SubmenuList { get { return submenuList; } }

        public ObservableCollection<WoProgress> progressdata = new ObservableCollection<WoProgress>();
        public ObservableCollection<WoProgress> WoProgressList {  get { return progressdata; } }

        ObservableCollection<Tnbwochecklisttype> checklistdata = new ObservableCollection<Tnbwochecklisttype>();
        public ObservableCollection<Tnbwochecklisttype> Checklistdata { get { return checklistdata; } }

        // Command
        public ICommand ChangeWOContentCommand { get; set; }
        public ICommand BackToWOListCommand { get; set; }
        INavigation Navigation;

        // API
        IMaximoApi maxrest = new MaximoApi();

        // Badge Counting
        public int sqaCounting = 0;
        public int permitCounting = 0;
        public int childCounting = 0;
        public int attachmentCounting = 0;
        public int worklogCounting = 0;
        public int checklistCounting = 0;
        public int testingCounting = 0;
        public int wospecCounting = 0;
        public int woassetCounting = 0;

        //progress call
       public ProgressBarUpdate progressBarUpdate;

        // Binding Table
        public List<Tnbwometergroup> MeterGroupListData { get; set; }
        public List<Doclinks> DoclinkListData { get; set; }
        public List<Worklog> WorklogListData { get; set; }
        public List<PermitWork> PermitListData { get; set; }
        public List<Plusgaudit> SQAListData { get; private set; }
        public List<Workorder> ChildWOList { get; set; }

        SfTabView sftabview;

        public TemplateWODetailViewModel(Workorder wo, INavigation navigation, SfTabView sftabView)
        {
            title = "Workorder Detail";
            subtitle = $"WONUM {wo.wonum}";
            workorder = wo;
            Navigation = navigation;
            ChangeWOContentCommand = new Command(async (param) => await ChangeWOContent(param.ToString()));

            //for update progress Bar
            progressBarUpdate = new ProgressBarUpdate(WoProgressList, prsqa, prinprg, prte, prphotos, prcomp, IsBusy);
            //progressBarUpdate.LoadProgress(workorder);

            WOContent = new WorkorderDetail(new WoDetailViewModel(workorder, Navigation,progressBarUpdate)).Content;
            
            BackToWOListCommand = new Command(BackToWOList);
            sftabview = sftabView;
            _ = LoadProgress();
            _ = LoadWoData();
            
            sqaCounting = 0;
            permitCounting = 0;
            childCounting = 0;
            attachmentCounting = 0;
            worklogCounting = 0;
            checklistCounting = 0;
            testingCounting = 0;
            _ = LoadCountingBadge();
        }

        public async Task LoadCountingBadge()
        {
            // attachment counting
            attachmentCounting = (int)await sqldoc.CountDoclinks(workorder.id, "WORKORDER", "0");

            // permit conting
            PermitListData = await sqlpermit.GetPermitWorkWO(workorder.wonum);
            permitCounting = PermitListData.Count;

            // worklog counting
            WorklogListData = await sqlWorklog.GetWorklogByWonum(workorder.wonum);
            worklogCounting = WorklogListData.Count;

            // sqa counting
            SQAListData = await sqlsqa.GetPlusgauditByWO(workorder.wonum);
            sqaCounting = SQAListData.Count;

            // child counting
            ChildWOList = await sqlwo.GetChildWorkorders(workorder.wonum);
            childCounting = ChildWOList.Count;

            // loadmetter counting
            MeterGroupListData = await sqlmetgroup.GetTnbwometergroupByWO(workorder.wonum);
            testingCounting = MeterGroupListData.Count;

            //load workorderspec counting
            List<AssetSpec> aspec = await sqlaspec.GetAssetSpecByWO(workorder.workorderid);
            List<LocationSpec> lspec = await sqllspec.GetLocationSpecByWO(workorder.workorderid);
            wospecCounting = 0;
            if(aspec.Count>0 || lspec.Count > 0)
            {
                wospecCounting = 1;
            }

            // load asset counting
            List<Asset> woasset = await sqlasset.GetAssetByWonum(workorder.wonum);
            woassetCounting = woasset.Count;
            // checklist counting
            List<Tnbpsiwochecklisttype> tnbpsiwochecklisttypes = await sqlpsi.GetTnbpsiwochecklisttypeByWO(workorder.wonum);
            checklistCounting = tnbpsiwochecklisttypes.Count;

            GenerateSubMenuInfo();
        }


        public void GenerateSubMenuInfo()
        {
            SubmenuList.Clear();
            SubmenuList.Add(new SubmenuInfo()
            {
                MenuTitle = "Workorder Detail",
                MenuImage = "menu_wodetail.png",
                SlugMenu  = "wodetail",
                MenuBadge = 0,
                TextColor = "Transparent",
                Background = "Transparent"
            });

            SubmenuList.Add(new SubmenuInfo()
            {
                MenuTitle = "SQA Checklist",
                MenuImage = "menu_sqa.png",
                SlugMenu = "sqalist",
                MenuBadge = sqaCounting,
                TextColor = "White",
                Background = "Red"
            });

            SubmenuList.Add(new SubmenuInfo()
            {
                MenuTitle = "Permit",
                MenuImage = "menu_permit.png",
                SlugMenu = "permit",
                MenuBadge = permitCounting,
                TextColor = "White",
                Background = "Red"
            });
            if (childCounting > 0)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Child Workorder",
                    MenuImage = "child.png",
                    SlugMenu = "child",
                    MenuBadge = childCounting,
                    TextColor = "White",
                    Background = "Red"
                });
            }

            if (wospecCounting > 0)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Specification",
                    MenuImage = "specification.png",
                    SlugMenu = "specification",
                    MenuBadge = wospecCounting,
                    TextColor = "White",
                    Background = "Red"
                });
            }

            if(woassetCounting > 0)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Asset",
                    MenuImage = "asset.png",
                    SlugMenu = "asset",
                    MenuBadge = woassetCounting,
                    TextColor = "White",
                    Background = "Red"
                });
            }
            

            if (workorder.fcprojectid != null)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Project DPMS",
                    MenuImage = "project.png",
                    SlugMenu = "project",
                    MenuBadge = 1,
                    TextColor = "White",
                    Background = "Red"
                });
            }

            if (checklistCounting > 0)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Checklist Form",
                    MenuImage = "menu_checklist.png",
                    SlugMenu = "testform",
                    MenuBadge = checklistCounting,
                    TextColor = "White",
                    Background = "Red"
                });
            }
            if (testingCounting>0)
            {
                SubmenuList.Add(new SubmenuInfo()
                {
                    MenuTitle = "Testing",
                    MenuImage = "menu_testing.png",
                    SlugMenu = "testing",
                    MenuBadge = testingCounting,
                    TextColor = "White",
                    Background = "Red"

                });
            }
            
            SubmenuList.Add(new SubmenuInfo()
            {
                MenuTitle = "Attachment",
                MenuImage = "menu_attachment.png",
                SlugMenu = "attachment",
                MenuBadge = attachmentCounting,
                TextColor = "White",
                Background = "Red"
            });

            SubmenuList.Add(new SubmenuInfo()
            {
                MenuTitle = "Worklog",
                MenuImage = "menu_log.png",
                SlugMenu  = "worklog",
                MenuBadge = worklogCounting,
                TextColor = "White",
                Background = "Red"
            });
                
        }

        async public Task ChangeWOContent(string SlugMenu)
        {
            await LoadWoData();
            await LoadProgress();
            if (SlugMenu == "permit")
            {
                title = "Workorder Permit";
                subtitle = "Permit List";
                WOContent = new PermitList(new PermitViewModel(workorder, Navigation)).Content;
            }
            else if (SlugMenu == "wodetail")
            {
                title = "Workorder Details";
                subtitle = $"WONUM {workorder.wonum}";
                WOContent = new WorkorderDetail(new WoDetailViewModel(workorder, Navigation,progressBarUpdate)).Content;
            }
            else if (SlugMenu == "sqalist")
            {
                title = "SQA/SQE Checklist";
                subtitle = $"List of SQA/SQE Created For : {workorder.wonum}";
                WOContent = new SQAList(new SqaViewModel(workorder, Navigation, sftabview, progressBarUpdate)).Content;
            }
            else if (SlugMenu == "testing")
            {
                title = "Testing";
                subtitle = "Workorder Meter Group";
                WOContent = new MeterList(new TestingMeterViewModel(workorder, Navigation, progressBarUpdate)).Content;
            }
            else if (SlugMenu == "child")
            {
                title = "Child Workorder";
                subtitle = $"Parent WO : {workorder.wonum}";
                WOContent = new ChildWorkorder(new ChildWOViewModel(workorder, Navigation, sftabview)).Content;
            }
            else if (SlugMenu == "testform")
            {
                title = "Test Form";
                subtitle = "Test Form List";
                WOContent = new TestformPage(new TestFormViewModel(workorder, Navigation)).Content;
            }
            else if (SlugMenu == "attachment")
            {
                title = "Attachment";
                subtitle = "Attachment List";
                WOContent = new AttachmentList(new AttachmentWOViewModel(workorder, Navigation, progressBarUpdate)).Content;
            }
            else if (SlugMenu == "worklog")
            {
                title = "Worklog";
                subtitle = "Worklog List";
                WOContent = new WorklogList(new WorklogWOViewModel(workorder, Navigation)).Content;
            }
            else if (SlugMenu == "specification")
            {
                title = "Specification";
                subtitle = "WO Spec List";
                WOContent = new WorkorderSpecification(new WorkorderspecViewModel(workorder, Navigation)).Content;
            }
            else if (SlugMenu == "asset")
            {
                title = "Asset";
                subtitle = "Asset Detail";
                WOContent = new AssetDetail(new AssetViewModel(workorder, Navigation)).Content;
            }
            else if (SlugMenu == "project")
            {
                title = "Project DPMS";
                subtitle = "DPMS PROGRESS "+workorder.fcprojectid;
                WOContent = new DPMSProjectPage(new ProjectDPMSViewModel(workorder, Navigation)).Content;
            }
        }

        async public Task LoadWoData()
        {
            workorder = await sqlwo.GetWorkorder(workorder.id);
            workorder.tnb_latitude2 = Math.Round(workorder.tnb_latitude2, 8);
            workorder.tnb_longitude2 = Math.Round(workorder.tnb_longitude2, 8);
        }

        async public Task LoadProgress()
        {
            if (IsBusy == false)
            {
                /*Debug.WriteLine("Load Progress function is run");
                Debug.WriteLine("Status is : " + workorder.status);*/
                IsBusy = true;

                WoProgressList.Clear();

                prsqa = prgps = prinprg = prte = prphotos = prcomp = StepStatus.NotStarted;

                List<Plusgaudit> sqa = await sqlsqa.GetPlusgauditByWO(workorder.wonum);
                List<Doclinks> doc = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER");
                totaldoclink = doc.Count;

                int totalmeterreading = 0;
                workorder.tnbwometergroup = await sqlmetgroup.GetTnbwometergroupByWO(workorder.wonum);
                if (workorder.tnbwometergroup != null)
                {
                    foreach (Tnbwometergroup tnbwometergroup in workorder.tnbwometergroup)
                    {
                        totalmeterreading += await sqlmeter.GetCountMeterReading(tnbwometergroup.tnbwometergroupid);
                    }
                }
                
                Debug.WriteLine("Number of Metter is : "+totalmeterreading);

                if (sqa.Where(x => x.status == "DRAFT").Count() == 0 && sqa.Count() > 0)
                {
                    prsqa = StepStatus.Completed;
                    prgps = StepStatus.InProgress;

                    Iprsqa = ImageSource.FromFile("SqaProgress.png");

                    

                    if (workorder.status == "INPRGS" || workorder.status == "TESTCOMP")
                        {
                            prinprg = StepStatus.Completed;
                            prte = StepStatus.InProgress;

                            Iprinprg = ImageSource.FromFile("StatusProgress.png");

                            if (totalmeterreading == 0)
                            {
                                prte = StepStatus.Completed;
                                prphotos = StepStatus.InProgress;
                                //skip meter dulu
                                Iprte = ImageSource.FromFile("TestingProgress.png");
                                if (totaldoclink > 1)
                                {
                                    prphotos = StepStatus.Completed;
                                    prcomp = StepStatus.InProgress;

                                    Iprphotos = ImageSource.FromFile("UploadProgress.png");

                                    if (workorder.status == "TESTCOMP")
                                    {
                                        prcomp = StepStatus.Completed;

                                        Iprcomp = ImageSource.FromFile("CompleteProgress.png");

                                    }
                                    else
                                    {
                                        Iprcomp = ImageSource.FromFile("complete_gray.png");
                                    }
                                }
                                else
                                {
                                    Iprphotos = ImageSource.FromFile("upload_gray.png");
                                    Iprcomp = ImageSource.FromFile("complete_gray.png");
                                }
                            }
                            else
                            {
                                Iprte = ImageSource.FromFile("testing_gray.png");
                                Iprphotos = ImageSource.FromFile("upload_gray.png");
                                Iprcomp = ImageSource.FromFile("complete_gray.png");
                            }
                        }
                        else
                        {
                            Iprinprg = ImageSource.FromFile("inprg_gray.png");
                            Iprte = ImageSource.FromFile("testing_gray.png");
                            Iprphotos = ImageSource.FromFile("upload_gray.png");
                            Iprcomp = ImageSource.FromFile("complete_gray.png");
                        }
                }
                else
                {
                    Iprsqa = ImageSource.FromFile("sqa_gray.png");
                    Iprgps = ImageSource.FromFile("gps_gray.png");
                    Iprinprg = ImageSource.FromFile("inprg_gray.png");
                    Iprte = ImageSource.FromFile("testing_gray.png");
                    Iprphotos = ImageSource.FromFile("upload_gray.png");
                    Iprcomp = ImageSource.FromFile("complete_gray.png");
                }

                WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprsqa,
                    Title = "Create SQA",
                    Status = prsqa
                });

                /*WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprgps,
                    Title = "Capture GPS Cordinate",
                    Status = prgps
                });*/

                WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprinprg,
                    Title = "Change Status to INPRG - In Progress",
                    Status = prinprg
                });

                WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprte,
                    Title = "Testing",
                    Status = prte
                });

                WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprphotos,
                    Title = "Upload Photos",
                    Status = prphotos
                });

                WoProgressList.Add(new WoProgress
                {
                    ProgressImage = Iprcomp,
                    Title = "Change Status to TESTCOMP - Complete",
                    Status = prcomp
                });

                IsBusy = false;


            }
           
        }


        void BackToWOList()
        {
          
            Navigation.PopAsync();
           // Navigation.PopToRootAsync();
        }

    }
}
