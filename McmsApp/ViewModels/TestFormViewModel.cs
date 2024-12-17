using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Login;
using McmsApp.Views.Work;
using McmsApp.Views.Work.WorkDetail.Attachment;
using McmsApp.Views.Work.WorkDetail.Testform;
using Syncfusion.Maui.ListView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class TestFormViewModel : BaseViewModel
    {
        //Rest Api Service
        IMaximoApi maxrest = new MaximoApi();

        //SQLite 
        ITnbwochecklisthdr sqlhdr = new SQLiteTnbwochecklisthdr();
        ITnbwochecklisttype sqltype = new SQLiteTnbwochecklisttype();
        ITnbpsiwochecklisttype sqlpsi= new SQLiteTnbpsiwochecklisttype();
        ITnbwochecklist sqlchecklist = new SQLiteTnbwochecklist();
        ITnbwochecklistsignature sqlsignature = new SQLiteTnbwochecklistsignature();
        IDoclinks sqldoc = new SQLiteDoclinks();
        IChecklistVal sqlval = new SQLiteTnbchecklistvalue();

        public ICommand SelectionChecklistCommand { get; set; }
        public ICommand SelectionPSIChecklistCommand { get; set; }
        public ICommand SelectedChecklistSignatureCommand { get; set; }
        public ICommand SelectionSignatureCommand { get; set; }
        public ICommand SelectionChecklistDetailCommand { get; set; }
        public ICommand SelectionChecklistAttachmentCommand { get; set; }
        public ICommand DeleteSignatureCommand { get; set; }
        public ICommand AddDigitalSignatureCommand { get; set; }
        public ICommand BackToCheckListCommand { get; set; }
        public ICommand SaveChecklistCommand { get; set; }
        public ICommand SaveSignatureCommand { get; set; }
        public ICommand PersonLookupCommand { get; set; }
        public ICommand OpenClosePickerCommand { get; set; }


        INavigation Navigation;
        public int checklistcount { get; set; }
        public bool IsRefreshing { get; set; }
        public string selectedIndicator { get; set; }
        public int heightchecklisttype { get; set; }
        private bool isBusy;
        public int totalIndicator { get; set; }
        public bool toggleAct { get; set; }
        public bool signvisible { get; set; }
        public bool imgvisible { get; set; }
        public bool hidesavesignature { get; set; }
        public bool clearsignature { get; set; }
        public bool pickerIsOpen { get; set; }
        public string selectedvalue { get; set; }

        public Workorder workorder { get; set; }
        public Tnbwochecklisthdr wochecklisthdr { get; set; }
        public Tnbwochecklisttype wochecklisttype { get; set; }
        public Tnbpsiwochecklisttype wopsichecklisttype { get; set;}
        public Tnbwochecklist wochecklist { get; set; }
        public Tnbwochecklist checklist { get; set; }
        public List<Tnbwochecklist> tnbwochecklists { get; set; }        
        public Tnbwochecklisttype SelectedCheckListType { get; private set; }
        public Tnbwochecklistsignature SelectedSignature { get; set; }
        public ObservableCollection<string> ListValue { get; set; }


        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                this.isBusy = value;
            }
        }

        public TestFormViewModel(Workorder wo, INavigation navigation)
        {
            workorder = wo;
            Navigation = navigation;
            _ = AddChecklist();
            SelectionChecklistCommand = new Command(SelectedChecklistType);
            SelectionPSIChecklistCommand = new Command(SelectedPSIChecklistType);
            SelectedChecklistSignatureCommand = new Command(SelectedChecklistSignature);
            SelectionSignatureCommand = new Command(SelectedSignatureDetail);
            SelectionChecklistDetailCommand = new Command(SelectedChecklistDetail);
            SelectionChecklistAttachmentCommand = new Command(SelectedChecklistAttachment);
            AddDigitalSignatureCommand = new Command(AddDigitalSignature);
            BackToCheckListCommand = new Command(BackToChecklist);
            SaveChecklistCommand = new Command(SaveChecklist);
            SaveSignatureCommand = new Command(async()=> await SaveDigitalSignature());
            DeleteSignatureCommand = new Command(DeleteSignature);
            PersonLookupCommand = new Command(lookupPerson);
            OpenClosePickerCommand = new Command(OpenClosePicker);

        }

        public async void OpenClosePicker(object param)
        {
            pickerIsOpen = !pickerIsOpen;
            if (pickerIsOpen)
            {
                ListValue = new ObservableCollection<string>();
                //wochecklisttype = param as Tnbwochecklisttype;
                Console.WriteLine(wochecklisttype.tnbchecklisttype);
                List<Tnbchecklistvalue> values = await sqlval.GetValueAsync(wochecklisttype.tnbchecklisttype);

                if(values.Count != 0)
                {
                    foreach (Tnbchecklistvalue value in values)
                    {
                        ListValue.Add(value.tnbvalue);                            
                    }
                }
                else
                {
                    ListValue.Add("Others");
                }
                
                Tnbwochecklist tnbwochecklist = param as Tnbwochecklist;
                checklist = tnbwochecklist;
            }
        }

        async void lookupPerson(object selected)
        {
            selectedIndicator = selected as string;
            string param = "person";
            if (selectedIndicator == "tnblabel")
            {
                param = "tnblabel";
            }
            await Navigation.PushModalAsync(new LookupUserModal(this,"checklist", Navigation,param));
        }


        async Task AddChecklist()
        {
            workorder.tnbpsiwochecklisttype = await sqlpsi.GetTnbpsiwochecklisttypeByWO(workorder.wonum);
            workorder.tnbpsiwochecklisttype = workorder.tnbpsiwochecklisttype.OrderByDescending(x => x.tnbsequence).ToList();
            foreach(Tnbpsiwochecklisttype tnbpsiwochecklisttype in workorder.tnbpsiwochecklisttype)
            {

                Debug.WriteLine("Value of TnbSequece is : " + tnbpsiwochecklisttype.tnbsequence);
                if (tnbpsiwochecklisttype.pendingupdate)
                {
                    tnbpsiwochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    tnbpsiwochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
                tnbpsiwochecklisttype.tnbwochecklisttype = await sqltype.GetTnbwochecklisttypeByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                tnbpsiwochecklisttype.totalIndicator = tnbpsiwochecklisttype.tnbwochecklisttype.Count;
                foreach(Tnbwochecklisttype tnbwochecklisttype in tnbpsiwochecklisttype.tnbwochecklisttype)
                {

                    
                    tnbwochecklisttype.tnbwochecklist = await sqlchecklist.GetTnbwochecklistByGroup(tnbwochecklisttype.tnbwochecklisttypeid);
                    tnbwochecklisttype.totalIndicator = tnbwochecklisttype.tnbwochecklist.Count;

                    for (int i = 0; i < tnbwochecklisttype.tnbwochecklist.Count; i++)
                    {

                        Debug.WriteLine("Describtion" + tnbwochecklisttype.tnbwochecklist[i].tnbdescription);

                        tnbwochecklisttype.tnbwochecklist[i].totalAttachment= (int)await sqldoc.CountDoclinks(workorder.id, "WORKORDER", tnbwochecklisttype.tnbwochecklist[i].tnbwochecklistid.ToString());
                    }



                }
                tnbpsiwochecklisttype.tnbwochecklisthdr = await sqlhdr.GetTnbwochecklisthdrByGroup(tnbpsiwochecklisttype.tnbpsiwochecklisttypeid);
                foreach(Tnbwochecklisthdr tnbwochecklisthdr in tnbpsiwochecklisttype.tnbwochecklisthdr)
                {
                    if (tnbwochecklisthdr.tnbapproveddate == null)
                    {
                        tnbwochecklisthdr.tnbapproveddate = DateTime.Now.ToLocalTime();
                    }
                    if (tnbwochecklisthdr.tnbchangedate == null)
                    {
                        tnbwochecklisthdr.tnbchangedate = DateTime.Now.ToLocalTime();
                    }
                    if (tnbwochecklisthdr.tnbvalidateddate == null)
                    {
                        tnbwochecklisthdr.tnbvalidateddate = DateTime.Now.ToLocalTime();
                    }
                    tnbwochecklisthdr.tnbchangedby = workorder.username;
                    tnbwochecklisthdr.tnbchangedate = DateTime.Now;
                }
                
            }
            checklistcount = workorder.tnbpsiwochecklisttype.Count;

        }

        async void AddDigitalSignature()
        {
            SelectedSignature = new Tnbwochecklistsignature();
            SelectedSignature.tnbwochecklistsignatureid = null;
            SelectedSignature.tnbownerid = wochecklisttype.tnbwochecklisttypeid;
            SelectedSignature.tnbsigneddate = DateTime.Now;
            hidesavesignature = true;
            clearsignature = true;
            signvisible = true;
            imgvisible = false;
            await Navigation.PushAsync(new DigitalSignatureForm(this));
        }

        async Task LoadSignatureList()
        {
            wochecklisttype.tnbwochecklistsignature = await sqlsignature.GetTnbwochecklistsignatureByGroup(wochecklisttype.tnbwochecklisttypeid);
            foreach(Tnbwochecklistsignature tnbwochecklistsignature in wochecklisttype.tnbwochecklistsignature)
            {
                if (tnbwochecklistsignature.pendingupdate)
                {
                    tnbwochecklistsignature.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    tnbwochecklistsignature.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
            }
            wochecklisttype.totalSignature = wochecklisttype.tnbwochecklistsignature.Count;
        }

        async void SelectedChecklistType(object obj)
        {
            SfListView selected = obj as SfListView;
            wochecklisttype = selected.SelectedItem as Tnbwochecklisttype;
            selected.SelectedItem = null;
            toggleAct = false;
            foreach (Tnbwochecklist chk in wochecklisttype.tnbwochecklist)
            {
                chk.xamvisible = true;
            }
            await Navigation.PushAsync(new TestformDetail(this));
        }

        void SelectedChecklistDetail(object obj)
        {
            SfListView selected = obj as SfListView;
            wochecklist = selected.SelectedItem as Tnbwochecklist;
            selected.SelectedItem = null;
            wochecklist.xamvisible = !wochecklist.xamvisible;
        }

       public async Task ChangedSignatureAsync(Boolean isChecked)
        {
                
            if (isChecked)
            {
                Debug.WriteLine("I am called method : "+isChecked);
                //SelectedSignature.tnbsigned = false;
                //change latter
                SelectedSignature.tnbIsTNBStuff = false;
                
                SelectedSignature.tnbsignedby = await SecureStorage.GetAsync("username");
                SelectedSignature.tnbsigneddate = DateTime.Now;
                // SelectedSignature.tnbsigneddate = DateTimeOffset.Now.ToUnixTimeSeconds();

                SelectedSignature.tnbsignedbycontractor = null;

            }
            else
            {
                //SelectedSignature.tnbsigned = true;
                //change latter
                SelectedSignature.tnbIsTNBStuff = true;
                SelectedSignature.tnbsignedby = null;
                SelectedSignature.tnbsigneddate = DateTime.Now;

            }
        }

        async void SelectedSignatureDetail(object obj)
        {
            SfListView selected = obj as SfListView;
            SelectedSignature = selected.SelectedItem as Tnbwochecklistsignature;
            selected.SelectedItem = null;
            if(SelectedSignature.signature!=null && SelectedSignature.signature != "" && SelectedSignature.pendingupdate==false)
            {
               
                if(SelectedSignature.tnbsigned)
                {
                    SelectedSignature.tnbIsTNBStuff = false;
                }
                else
                {
                    SelectedSignature.tnbIsTNBStuff = true;
                }
                hidesavesignature = false;
                clearsignature = false;
                signvisible = false;
                imgvisible = true;
                
            }
            else
            {
                if (SelectedSignature.tnbsigned)
                {
                    SelectedSignature.tnbIsTNBStuff = false;
                }
                else
                {
                    SelectedSignature.tnbIsTNBStuff = true;
                }
                hidesavesignature = true;
                clearsignature = true;
                signvisible = true;
                imgvisible = false;
            }
            await Navigation.PushAsync(new DigitalSignatureForm(this));
        }

        async public void SelectedPSIChecklistType(object obj)
        {
            SfListView selected = obj as SfListView;
            wopsichecklisttype = selected.SelectedItem as Tnbpsiwochecklisttype;
            foreach(Tnbwochecklisttype tnbwochecklisttype in wopsichecklisttype.tnbwochecklisttype)
            {
                tnbwochecklisttype.totalAttachment = (int)await sqldoc.CountDoclinks(workorder.id, "WORKORDER", tnbwochecklisttype.tnbwochecklisttypeid.ToString());
                tnbwochecklisttype.tnbwochecklistsignature = await sqlsignature.GetTnbwochecklistsignatureByGroup(tnbwochecklisttype.tnbwochecklisttypeid);
                tnbwochecklisttype.totalSignature = tnbwochecklisttype.tnbwochecklistsignature.Count;
                if (tnbwochecklisttype.pendingupdate)
                {
                    tnbwochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                }
                else
                {
                    tnbwochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                }
                foreach (Tnbwochecklistsignature tnbwochecklistsignature in tnbwochecklisttype.tnbwochecklistsignature)
                {
                    if (tnbwochecklistsignature.pendingupdate)
                    {
                        tnbwochecklistsignature.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    }
                    else
                    {
                        tnbwochecklistsignature.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    }
                }
            }
            //wochecklisthdr = wopsichecklisttype.tnbwochecklisthdr[0];
            heightchecklisttype = 100 + (85 * wopsichecklisttype.tnbwochecklisttype.Count);
            selected.SelectedItem = null;
            await Navigation.PushAsync(new TestformType(this));
        }

        async void SelectedChecklistAttachment(object obj)
        {

            //change in here
            //  Tnbwochecklisttype chcktype = obj as Tnbwochecklisttype;
            Tnbwochecklist chcktype = obj as Tnbwochecklist;
            await Navigation.PushModalAsync(new AttachmentChecklist(workorder, Navigation,chcktype, chcktype.tnbwochecklisttypeid));
        }

        async void SelectedChecklistSignature(object obj)
        {
            wochecklisttype = obj as Tnbwochecklisttype;
            await Navigation.PushAsync(new DigitalSignatureList(this));
        }

        void BackToChecklist()
        {
            Navigation.PopAsync();
        }

        public void SetCheckBox(int id, string type)
        {
            Console.WriteLine(id + " - " + type);
            Tnbwochecklist tnbwochecklist = wochecklisttype.tnbwochecklist.Where(x => x.tnbwochecklistid == id).First();
            if (type == "tnbyes")
            {
                tnbwochecklist.tnbno = false;
                tnbwochecklist.tnbna = false;
                tnbwochecklist.tnbpasswithcondition = false;
            }
            if (type == "tnbno")
            {
                tnbwochecklist.tnbyes = false;
                tnbwochecklist.tnbna = false;
                tnbwochecklist.tnbpasswithcondition = false;
            }
            if (type == "tnbna")
            {
                tnbwochecklist.tnbno = false;
                tnbwochecklist.tnbyes = false;
                tnbwochecklist.tnbpasswithcondition = false;
            }
            if (type == "tnbpasswithcondition")
            {
                tnbwochecklist.tnbno = false;
                tnbwochecklist.tnbna = false;
                tnbwochecklist.tnbyes = false;
            }
        }

        public void calculatePercentage()
        {
            double yes = wochecklisttype.tnbwochecklist.Where(x => x.tnbyes || x.tnbpasswithcondition).Count();
            double total = wochecklisttype.tnbwochecklist.Count;
            wochecklisttype.tnbpassper = Math.Round((yes / total) * 100, 0);

            double psipercent = wopsichecklisttype.tnbwochecklisttype.Sum(item => item.tnbpassper);
            wopsichecklisttype.tnbpassper = Math.Round(psipercent / wopsichecklisttype.tnbwochecklisttype.Count(),0);
        }

        async void SaveChecklist()
        {
            calculatePercentage();
            UserDialogs.Instance.ShowLoading("Uploading Test Form...");
            await Task.Delay(1000);

            bool check = await maxrest.Ping();
            if (check)
            {
                Tnbwochecklisttype checklisttype = new Tnbwochecklisttype();
                checklisttype = wochecklisttype;
                checklisttype.tnbwochecklistsignature = null;
                Tnbwochecklisttype res = await maxrest.UpdateChecklist(checklisttype);
                UserDialogs.Instance.HideLoading();

                if (res == null)
                {
                    checklisttype.pendingupdate = true;

                    wopsichecklisttype.pendingupdate = true;
                    SQLiteTnbpsiwochecklisttype type = new SQLiteTnbpsiwochecklisttype();
                    await type.UpdateTnbpsiwochecklisttype(wopsichecklisttype);
                    wopsichecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;




                    checklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    await sqltype.UpdateTnbwochecklisttype(checklisttype);
                    foreach (Tnbwochecklist tnbwochecklist in checklisttype.tnbwochecklist)
                    {
                        await sqlchecklist.UpdateTnbwochecklist(tnbwochecklist);
                    }
                    await UserDialogs.Instance.AlertAsync("Update Checklist Success but not upload to maximo", "Success", "Ok");
                    await Navigation.PopAsync();
                }
                else if (res.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(res.Error.message, "Failed", "Ok");
                    if (res.Error.reasonCode == "BMXAA9549E")
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
                    checklisttype.pendingupdate = false;
                    checklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    await sqltype.UpdateTnbwochecklisttype(checklisttype);
                    foreach (Tnbwochecklist tnbwochecklist in checklisttype.tnbwochecklist)
                    {
                        await sqlchecklist.UpdateTnbwochecklist(tnbwochecklist);
                    }
                    wochecklisttype.pendingupdate = false;
                    wochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    await UserDialogs.Instance.AlertAsync("Update Checklist Success", "Success", "Ok");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                wochecklisttype.pendingupdate = true;

                wopsichecklisttype.pendingupdate = true;
                SQLiteTnbpsiwochecklisttype type = new SQLiteTnbpsiwochecklisttype();
               await type.UpdateTnbpsiwochecklisttype(wopsichecklisttype);
                wopsichecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;

                wochecklisttype.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                await sqltype.UpdateTnbwochecklisttype(wochecklisttype);
                foreach (Tnbwochecklist tnbwochecklist in wochecklisttype.tnbwochecklist)
                {
                    await sqlchecklist.UpdateTnbwochecklist(tnbwochecklist);
                }
                await UserDialogs.Instance.AlertAsync("Update Checklist Success but not upload to maximo", "Success", "Ok");
                await Navigation.PopAsync();
            }
        }

        public async Task SaveDigitalSignature()
        {
            UserDialogs.Instance.ShowLoading("Uploading Signature...");
            await Task.Delay(1000);

            bool check = await maxrest.Ping();
            if (check)
            {
                SelectedSignature.tnbexecdate = DateTime.Now;
                SelectedSignature.tnbsigneddate = DateTime.Now;
                SelectedSignature.updateby = await SecureStorage.GetAsync("username");
                Tnbwochecklistsignature res = new Tnbwochecklistsignature();

                
                if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                {
                    res = await maxrest.UpdateSignature(SelectedSignature);
                    if (SelectedSignature.signature !=null)
                    res = await maxrest.UpdateImglibSignature(SelectedSignature);
                    //res = await maxrest.AddImglibSignature(SelectedSignature);
                    // 

                    /*Imglib imglib = new Imglib();
                    imglib.image = SelectedSignature.signature;
                    imglib.refobject = "TNBWOCHECKLISTSIGNATURE";
                    imglib.refobjectid = SelectedSignature.tnbwochecklistsignatureid;
                    imglib.imagename = "new.png";
                    imglib.mimetype = "image/png";
                    List<Imglib> imglibs = new List<Imglib>();
                    imglibs.Add(imglib);

                    SelectedSignature.imglib = imglibs;*/

                                    
                }
                else
                {
                    //do not use this condition because we do not create new signature
                    res = await maxrest.AddSignature(SelectedSignature);

                    res = null;
                }
                UserDialogs.Instance.HideLoading();

                if (res == null)
                {
                    SelectedSignature.pendingupdate = true;
                    if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                    {
                        await sqlsignature.UpdateTnbwochecklistsignature(SelectedSignature);
                    }
                    else
                    {
                        await sqlsignature.AddTnbwochecklistsignature(SelectedSignature);
                    }
                    await LoadSignatureList();
                    await UserDialogs.Instance.AlertAsync("Signature Save Locally in this device, New Signature always uploaded with parent", "Success", "Ok");
                    await Navigation.PopAsync();
                }
                else if (res.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(res.Error.message, "Failed", "Ok");
                    if (res.Error.reasonCode == "BMXAA9549E")
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
                    if (res._imagelibref==null && SelectedSignature.signature!=null)
                    {
                        SelectedSignature.tnbwochecklistsignatureid = res.tnbwochecklistsignatureid;
                        Tnbwochecklistsignature sglib = await maxrest.AddImglibSignature(SelectedSignature);
                        if(sglib==null || sglib.Error != null)
                        {
                            await UserDialogs.Instance.AlertAsync("Signature Cannot Save to IMAGELIB, Please try again!", "Error!", "Ok");
                            SelectedSignature.pendingupdate = true;
                        }
                        else
                        {
                            SelectedSignature.pendingupdate = false;
                            SelectedSignature._imagelibref = sglib._imagelibref;
                        }
                    }
                    if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                    {
                        SelectedSignature.pendingupdate = false;
                        await sqlsignature.UpdateTnbwochecklistsignature(SelectedSignature);
                    }
                    else
                    {
                        SelectedSignature.tnbwochecklistsignatureid = res.tnbwochecklistsignatureid;
                        await sqlsignature.AddTnbwochecklistsignature(SelectedSignature);
                    }
                    await LoadSignatureList();
                    await UserDialogs.Instance.AlertAsync("Signature Save to Maximo", "Success", "Ok");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                SelectedSignature.pendingupdate = true;
                SelectedSignature.tnbexecdate = DateTime.Now;
                SelectedSignature.tnbsigneddate = DateTime.Now;
                SelectedSignature.updateby = await SecureStorage.GetAsync("username");

                if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                {
                    await sqlsignature.UpdateTnbwochecklistsignature(SelectedSignature);
                }
                else
                {
                    await sqlsignature.AddTnbwochecklistsignature(SelectedSignature);
                }
                await LoadSignatureList(); ;
                await UserDialogs.Instance.AlertAsync("Signature Save Locally in this device, please upload again if you have better connection", "Success", "Ok");
                await Navigation.PopAsync();
            }
        }

        public async void DeleteSignature(object obj)
        {
            UserDialogs.Instance.ShowLoading("Delete Signature...");
            await Task.Delay(1000);

            SelectedSignature = obj as Tnbwochecklistsignature;
            if (SelectedSignature.tnbwochecklistsignatureid == 0 && SelectedSignature.tnbwochecklistsignatureid == null)
            {
                await sqlsignature.DeleteTnbwochecklistsignature(SelectedSignature);
                await LoadSignatureList();
                await UserDialogs.Instance.AlertAsync("Signature Deleted", "Success", "Ok");
                return;
            }
            bool check = await maxrest.Ping();
            if (check)
            {
                SelectedSignature._action = "Delete";
                Tnbwochecklistsignature res = await maxrest.UpdateSignature(SelectedSignature);
                UserDialogs.Instance.HideLoading();

                if (res == null)
                {
                    await UserDialogs.Instance.AlertAsync("Please check your internet connection and try again!", "Failed!", "Ok");
                    await Navigation.PopAsync();
                }
                else if (res.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(res.Error.message, "Failed", "Ok");
                    if (res.Error.reasonCode == "BMXAA9549E")
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
                    await sqlsignature.DeleteTnbwochecklistsignature(SelectedSignature);
                    await LoadSignatureList();
                    await UserDialogs.Instance.AlertAsync("Signature Delete in Maximo", "Success", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("Please check your internet connection and try again!", "Failed!", "Ok");
            }
        }

    }
}
