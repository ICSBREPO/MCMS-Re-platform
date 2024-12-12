using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.Helpers;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.ImagePreview;
using McmsApp.Views.Login;
using McmsApp.Views.PopupPages;
using McmsApp.Views.Work;
using McmsApp.Views.Work.WorkDetail.Permit;
using Newtonsoft.Json;
//using Plugin.FilePicker;
//using Plugin.FilePicker.Abstractions;
//using Plugin.Media;
//using Plugin.Media.Abstractions;
using Syncfusion.Maui.ListView;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;




namespace McmsApp.ViewModels
{
    public class PermitViewModel : BaseViewModel
    {
        IMaximoApi maxrest = new MaximoApi();

        public Workorder workorder { get; set; }
        public PermitWork permit { get; set; }
        public int permitcount { get; set; }
        public bool NoPermit { get; set; }
        public Stream m_pdfDocumentStream { get; set; }

        IPermitWork sqlpermit = new SQLitePermitWork();
        public IDoclinks sqldocs = new SQLiteDoclinks();

        public ICommand BackToCommand { get; set; }
        public ICommand BackModalCommand { get; set; }
        public ICommand SelectedPermitCommand { get; set; }
        public ICommand OptionUpload { get; set; }
        public ICommand AddPermitCommand { get; set; }
        public ICommand SaveDoclinksCommand { get; private set; }
        public ICommand SavePermitCommand { get; private set; }
        public ICommand DeleteDoclinksCommand { get; private set; }
        public ICommand SelectedDoclinksListCommand { get; private set; }
        public ICommand StateCommand { get; set; }
        public ICommand SubzoneCommand { get; set; }
        public ICommand PermitTypeCommand { get; set; }
        public ICommand BussinessAreaCommand { get; set; }
        //public ICommand PermitStatusCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand PersonLookupCommand { get; set; }
        public ICommand DownloadDocumentData { get; set; }
        public ICommand IssueByCommand { get; set; }
        public ICommand IssueToCommand { get; set; }
        public ICommand DownloadAttachment { get; set; }

        public Doclinks TempAttachment { get; set; }
        public List<PermitWork> PermitListData { get; set; }
        public ObservableCollection<Doclinks> AttachmentListData { get; set; }

        ObservableCollection<MxDomain> domaindata = new ObservableCollection<MxDomain>();
        public ObservableCollection<MxDomain> DomainData { get { return domaindata; } }

        public string selectedIndicator { get; set; }
        public bool isDownload { get; set; }
        public bool isNoDownload { get; set; }

        // Pdf creator
        public ISaveFile savefile = new SaveFile();

        // Binding view
        public bool isPdf { get; set; }
        public bool isImage { get; set; }

        // Open picker
        public ICommand PickerActionCommand { get; set; }
        public bool pickerIsOpen { get; set; }
        public ICommand pickerSetValueCommand { get; set; }
        public string pickerSetPermitStatus { set; get; }
        public string SelectedPermitStatus { get; set; }

        // Permit Status
        public ObservableCollection<string> PermitStatus { get; set; }

        INavigation Navigation;
        public string base64Attachment { get; set; }

        public PermitViewModel(Workorder wo, INavigation navigation)
        {
            workorder = wo;
            Navigation = navigation;
            BackToCommand = new Command(BackTo);
            SelectedPermitCommand = new Command(SelectedPermit);
            _ = LoadPermit();
            OptionUpload = new Command(PopActionSheet);
            AddPermitCommand = new Command(AddPermit);
            SavePermitCommand = new Command(SavePermit);
            BackModalCommand = new Command(BackModal);
            SaveDoclinksCommand = new Command(async () => await SaveAttachment());
            DeleteDoclinksCommand = new Command(async () => await DeleteAttachment());
            SelectedDoclinksListCommand = new Command(SelectedDoclinks);
            StateCommand = new Command(SelectStateCommand);
            SubzoneCommand = new Command(SelectSubzoneCommand);
            PermitTypeCommand = new Command(SelectPermitTypeCommand);
            BussinessAreaCommand = new Command(SelectBussinessAreaCommand);
            //PermitStatusCommand = new Command(SelectPermitStatusCommand);
            UnitCommand = new Command(SelectUnitCommand);
            PersonLookupCommand = new Command(lookupPerson);
            DownloadDocumentData = new Command(downloadDocumentData);
            DownloadAttachment = new Command(async (obj) => downloadAttachment());
            LoadMxDomain();

            // Set Value List Status
            //check later is this connected to some other Dropdown?
            PermitStatus = new ObservableCollection<string>();
            PermitStatus.Add("ISSUED");
            //PermitStatus.Add("REQUEST");
           // PermitStatus.Add("APPR");
            //PermitStatus.Add("DRAFT");
            PermitStatus.Add("CANCELLED");
            //PermitStatus.Add("CLOSED");
            base64Attachment = "";
            PickerActionCommand = new Command(OpenPicker);
        }

        public void OpenPicker()
        {
            pickerIsOpen = !pickerIsOpen;
        }

        async void downloadDocumentData()
        {
            UserDialogs.Instance.ShowLoading("Downloading Attachment...");
            await Task.Delay(2000);
            try
            {

                bool check = await maxrest.Ping();
                if (check)
                {
                    Doclinks doclinks = await maxrest.GetDoclinks(TempAttachment.doclinksid);
                    UserDialogs.Instance.HideHud();
                    if (doclinks == null)
                    {
                        await UserDialogs.Instance.AlertAsync("Failed to download attachment, please try again later!", "Warning!", "Ok");
                    }
                    else if (doclinks.Error != null)
                    {
                        await UserDialogs.Instance.AlertAsync(doclinks.Error.message, "Failed!", "Ok");
                        if (doclinks.Error.reasonCode == "BMXAA9549E")
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
                        isDownload = true;
                        isNoDownload = false;
                        TempAttachment.documentdata = doclinks.documentdata;
                        TempAttachment.previewdoc = doclinks.documentdata;
                        if (Base64Files.GetFileExtension(TempAttachment.documentdata) == "pdf")
                        {
                            isImage = false;
                            isPdf = true;
                            byte[] _fromBase64String = Convert.FromBase64String(TempAttachment.documentdata);
                            string filename = doclinks.fileName;
                            string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                            m_pdfDocumentStream = File.OpenRead(path);
                            TempAttachment.previewdoc = Base64Files.pdfimage;
                        }
                        else
                        {
                            isImage = true;
                            isPdf = false;
                            TempAttachment.previewdoc = TempAttachment.documentdata;
                        }
                        await sqldocs.UpdateDoclinks(TempAttachment);
                        await UserDialogs.Instance.AlertAsync("Attachment Successfully Downloaded", "Success!", "Ok");
                    }
                }
                else
                {
                    UserDialogs.Instance.HideHud();
                    await UserDialogs.Instance.AlertAsync("Failed to download attachment, please try again later!", "Warning!", "Ok");
                }
            }
            catch (Exception e)
            {
                await UserDialogs.Instance.AlertAsync($"Error : {e.Message}", "Warning", "OK");
                UserDialogs.Instance.HideHud();
            }
        }

        async void lookupPerson(object selected)
        {
            selectedIndicator = selected as string;
            string type = "person";
            if (selectedIndicator == "tnbpermittype")
            {
                type = "tnbpermittype";
            }
            await Navigation.PushModalAsync(new LookupUserModal(this, "permit", Navigation,type));
        }

        void BackTo()
        {
            Navigation.PopAsync();
        }

        async void BackModal()
        {
            await Navigation.PopModalAsync();
        }

        async Task LoadPermit()
        {
            PermitListData = await sqlpermit.GetPermitWorkWO(workorder.wonum);
            if (PermitListData.Count < 1)
            {
                NoPermit = true;
            }
            else
            {
                NoPermit = false;
                foreach (PermitWork ptw in PermitListData)
                {
                    if (ptw.pendingupload)
                    {
                        ptw.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.Away;
                    }
                    else
                    {
                        ptw.badgeicon = Syncfusion.XForms.BadgeView.BadgeIcon.None;
                    }
                    ptw.doclinks = await sqldocs.GetDoclinksFilter(ptw.id, "PLUSGPERMITWORK");
                    Debug.WriteLine("No of attachment in List : " + ptw.doclinks.Count());
                }
                permitcount = PermitListData.Count;
            }
        }

        async void SelectedPermit(object obj)
        {
            SfListView selected = obj as SfListView;
            PermitWork per = selected.SelectedItem as PermitWork;
            selected.SelectedItem = null;
            permit = per;
            SelectedPermitStatus = permit.tnbstatus;
            AttachmentListData = new ObservableCollection<Doclinks>();
            Debug.WriteLine("No of attachment start: " + per.doclinks.Count());
            foreach (Doclinks doc in per.doclinks)
            {
                string json = JsonConvert.SerializeObject(
                doc, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                });
                System.Diagnostics.Debug.WriteLine(json);
                
                if (doc.documentdata == null)
                {
                    doc.previewdoc = Base64Files.downloadimage;
                }
                else
                {
                    if (Base64Files.GetFileExtension(doc.documentdata) == "pdf")
                    {
                        doc.previewdoc = Base64Files.pdfimage;
                        isImage = false;
                        isPdf = true;
                    }
                    else
                    {
                        isImage = true;
                        isPdf = false;
                        doc.previewdoc = doc.documentdata;
                    }
                }
                AttachmentListData.Add(doc);
                Debug.WriteLine("No of attachment : " + AttachmentListData.Count());
            }
            await Navigation.PushAsync(new PermitDetail(this));
        }

        async void AddPermit()
        {
            permit = new PermitWork();
            permit.tnbstatus = "DRAFT";
            permit.tnbbusinessarea = workorder.tnbbusinessarea;
            permit.orgid = "TNBDN";
            permit.siteid = workorder.siteid;
            permit.tnbpermittype = "HARDCOPY";
            permit.workorderid = workorder.workorderid;
            permit.tnbwonum = workorder.wonum;
            permit.tnbsafety = "YES";
            permit._action = "Add";
            permit.tnbstate = workorder.tnbstate;
            permit.tnbsubzone = workorder.tnbsubzone;
            permit.ptwclass = "PERMIT";
            permit.tnbzone = workorder.tnbzone;
            permit.tnbissuedate = DateTime.Now.ToLocalTime();
            permit.doclinks = new List<Doclinks>();
            AttachmentListData = new ObservableCollection<Doclinks>();
            await Navigation.PushAsync(new PermitDetail(this));
        }



        async void SavePermit()
        {
            UserDialogs.Instance.ShowLoading("Uploading Permit. . .");

            await Task.Delay(1000);
            bool check = await maxrest.Ping();
            //Doclinks Treatment
            List<Doclinks> deletedoc = await sqldocs.GetDoclinksFilter(permit.id, "PLUSGPERMITWORK",true);
            Debug.WriteLine("Total Attachment in Permit is  : " + deletedoc.Count());
            List<Doclinks> uploaddoc = new List<Doclinks>();
            foreach(Doclinks doc in deletedoc)
            {
                uploaddoc.Add(doc);
            }
            foreach (Doclinks doc in permit.doclinks.Where(x=>x.docinfoid==null || x.isdelete))
            {
                uploaddoc.Add(doc);
            }
            permit.doclinks.Clear();
            permit.doclinks = uploaddoc;
            // End Docllinks
            permit.tnbstatus = SelectedPermitStatus;
            if (check)
            {
                //start save action
                PermitWork respermit = new PermitWork();
                if (permit.permitworknum != null)
                {
                    respermit = await maxrest.UpdatePermit(permit);
                }
                else
                {
                    respermit = await maxrest.AddPermit(permit);
                }
                UserDialogs.Instance.HideHud();
                if(respermit == null)
                {
                    permit.pendingupload = true;
                    if (permit.id == 0)
                    {
                        await UserDialogs.Instance.AlertAsync("ADD Permit Success but not upload to maximo", "Success", "Ok");
                        int id = await sqlpermit.AddPermitWork(permit);
                        permit.id = id;
                    }
                    else
                    {
                        await UserDialogs.Instance.AlertAsync("Update Permit Success but not upload to maximo", "Success", "Ok");
                        await sqlpermit.UpdatePermitWork(permit);
                    }
                    foreach (Doclinks doc in permit.doclinks)
                    {
                        doc.refid = permit.id;
                        doc.pendingupload = true;
                        if (doc.id == 0)
                        {
                            await sqldocs.AddDoclinks(doc);
                        }
                        else
                        {
                            if (doc.isdelete)
                            {
                                await sqldocs.UpdateDoclinks(doc);
                            }
                            else
                            {
                                await sqldocs.AddDoclinks(doc);
                            }
                        }
                    }
                    await LoadPermit();
                    await Navigation.PopAsync();
                }
                else if (respermit.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(respermit.Error.message, "Failed", "Ok");
                    if (respermit.Error.reasonCode == "BMXAA9549E")
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
                    respermit.id = permit.id;
                    respermit.pendingupload = false;
                    if (permit.id == 0)
                    {
                        DateTime statdate = Convert.ToDateTime(respermit.statusdate);
                        statdate = statdate.ToLocalTime();
                        respermit.statusdate = statdate;

                        int id = await sqlpermit.AddPermitWork(respermit);
                        respermit.id = id;
                    }
                    else
                    {
                        await sqlpermit.UpdatePermitWork(respermit);
                    }
                    List<Doclinks> deldocs = await sqldocs.GetDoclinksFilter(respermit.id, "PLUSGPERMITWORK");
                    foreach(Doclinks doc in deldocs)
                    {
                        await sqldocs.DeleteDoclinks(doc);
                    }
                    if (respermit.doclinks != null)
                    {
                        foreach (Doclinks doc in respermit.doclinks)
                        {
                            doc.refid = respermit.id;
                            await sqldocs.AddDoclinks(doc);
                        }
                    }
                    await UserDialogs.Instance.AlertAsync("Data uploaded to maximo", "Success", "Ok");
                    await LoadPermit();
                    await Navigation.PopAsync();
                }

            }
            else
            {
                UserDialogs.Instance.HideHud();
                permit.pendingupload = true;
                Debug.WriteLine("Permit ID is : =" + permit.id);

                if (permit.id == 0)
                {
                    await UserDialogs.Instance.AlertAsync("ADD Permit Success but not upload to maximo", "Success", "Ok");
                    int id = await sqlpermit.AddPermitWork(permit);
                    permit.id = id;
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync("Update Permit Success but not upload to maximo", "Success", "Ok");
                    await sqlpermit.UpdatePermitWork(permit);
                }
                foreach (Doclinks doc in permit.doclinks)
                {

                    Debug.WriteLine("Saveing attachment : " + permit.doclinks.Count());
                    Debug.WriteLine("doc.id : " +doc.id);
                    doc.refid = permit.id;
                    doc.refid = permit.id;
                    doc.pendingupload = true;
                    if (doc.id == null)
                    {
                        await sqldocs.AddDoclinks(doc);
                    }
                    else
                    {
                        if (doc.isdelete)
                        {
                            await sqldocs.UpdateDoclinks(doc);
                        }
                        else
                        {
                            await sqldocs.AddDoclinks(doc);
                        }
                    }
                }
                
                await LoadPermit();
                await Navigation.PopAsync();
            }
        }
        async void PopActionSheet()
        {
            string actionSheet = await Application.Current.MainPage.DisplayActionSheet("Option", null, "Cancel", "Take Photo", "Take Photo From Gallery", "Take PDF");

            switch (actionSheet)
            {
                case "Take Photo":

                    // Do Something when 'Take Photo' Button is pressed
                    try
                    {
                        //var status = await Permissions.RequestAsync<Permissions.Camera>();
                        
                        var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                        var storageStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

                        if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
                        {
                            await Application.Current.MainPage.DisplayAlert("Permission Denied", "Camera and storage access are required.", "Ok");
                            return;
                        }


                        //await CrossMedia.Current.Initialize();

                        //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        //{
                        //    await UserDialogs.Instance.AlertAsync(":(", "Camera Not Available", "Ok");
                        //    return;
                        //}

                        var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                        {
                            Title = "Capture a photo"
                        });

                        if (photo == null)
                            return;

                        string filename = "permit" + DateTimeOffset.Now.ToUnixTimeSeconds();

                        byte[] fileBytes;
                        using (var stream = await photo.OpenReadAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }
                        string base64File = Convert.ToBase64String(fileBytes);


                        //var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        //{
                        //    AllowCropping = false,
                        //    Directory = "MCMS",
                        //    Name = filename,
                        //    PhotoSize = PhotoSize.MaxWidthHeight,
                        //    CompressionQuality = 92,
                        //    SaveToAlbum = true
                        //});

                        //if (file == null)
                        //    return;
                        //byte[] b = File.ReadAllBytes(file.Path);
                        //String Base64File = Convert.ToBase64String(b);

                        isImage = true;
                        isPdf = false;

                        //TempAttachment = new Doclinks
                        //{
                        //    fileName = filename + ".jpg",
                        //    title = filename,
                        //    ownertable = "PLUSGPERMITWORK",
                        //    documentdata = Base64File,
                        //    previewdoc = Base64File,
                        //    urlname = filename + ".jpg",
                        //    urltype = "FILE",
                        //    _action = "Add",
                        //    doctype = "Attachments",
                        //    modified = DateTime.Now.ToLocalTime(),
                        //    createdate = DateTime.Now.ToLocalTime(),
                        //};

                        TempAttachment = new Doclinks
                        {
                            fileName = filename,
                            title = Path.GetFileNameWithoutExtension(filename),
                            ownertable = "PLUSGPERMITWORK",
                            documentdata = base64File,
                            previewdoc = base64File,
                            urlname = filename + ".jpg",
                            urltype = "FILE",
                            _action = "Add",
                            doctype = "Attachments",
                            modified = DateTime.Now,
                            createdate = DateTime.Now,
                        };

                        await Navigation.PushModalAsync(new AddAttachmentPermit(this));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await UserDialogs.Instance.AlertAsync("Please Check Camera Permission, Close App And Open Again", "Alert!", "Ok");
                    }

                    break;

                //case "Take Photo From Gallery":

                //    // Do Something when 'Choose From Photo' Button is pressed
                //    try
                //    {
                //        await CrossMedia.Current.Initialize();
                //        if (!CrossMedia.Current.IsPickPhotoSupported)
                //        {
                //            await UserDialogs.Instance.AlertAsync("No Gallery Detected");
                //            return;
                //        }
                //        else
                //        {
                //            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                //            {
                //                PhotoSize = PhotoSize.MaxWidthHeight,

                //            });
                //            if (file == null)
                //                return;
                //            string filename = "permit" + DateTimeOffset.Now.ToUnixTimeSeconds();
                //            byte[] b = File.ReadAllBytes(file.Path);
                //            String Base64File = Convert.ToBase64String(b);

                //            isImage = true;
                //            isPdf = false;

                //            TempAttachment = new Doclinks
                //            {
                //                fileName = filename + ".jpg",
                //                title = filename,
                //                ownertable = "PLUSGPERMITWORK",
                //                documentdata = Base64File,
                //                previewdoc = Base64File,
                //                urlname = filename + ".jpg",
                //                urltype = "FILE",
                //                doctype = "Attachments",
                //                _action = "Add",
                //                createdate = DateTime.Now.ToLocalTime(),
                //                modified = DateTime.Now.ToLocalTime(),
                //            };
                //            await Navigation.PushModalAsync(new AddAttachmentPermit(this));
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //        await UserDialogs.Instance.AlertAsync("Please Check Permission to Pick From Galery, Close App And Open Again", "Alert!", "Ok");
                //    }
                //    break;

                case "Take Photo From Gallery":
                    try
                    {
                        // Request storage permissions
                        var storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();

                        if (storageStatus != PermissionStatus.Granted)
                        {
                            await Application.Current.MainPage.DisplayAlert("Permission Denied", "Storage access is required.", "Ok");
                            return;
                        }

                        // Pick photo using MediaPicker
                        var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                        {
                            Title = "Pick a photo"
                        });

                        if (photo == null)
                            return;

                        // Define filename
                        string filename = "permit" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".jpg";

                        // Read file bytes
                        byte[] fileBytes;
                        using (var stream = await photo.OpenReadAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        // Convert to Base64
                        string base64File = Convert.ToBase64String(fileBytes);

                        // Prepare attachment object
                        isImage = true;
                        isPdf = false;

                        TempAttachment = new Doclinks
                        {
                            fileName = filename,
                            title = Path.GetFileNameWithoutExtension(filename),
                            ownertable = "PLUSGPERMITWORK",
                            documentdata = base64File,
                            previewdoc = base64File,
                            urlname = filename,
                            urltype = "FILE",
                            _action = "Add",
                            doctype = "Attachments",
                            modified = DateTime.Now,
                            createdate = DateTime.Now,
                        };

                        // Navigate to the attachment page
                        await Application.Current.MainPage.Navigation.PushModalAsync(new AddAttachmentPermit(this));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await Application.Current.MainPage.DisplayAlert("Error", "Please check permission to pick from gallery, close app and try again.", "Ok");
                    }
                    break;


                //case "Take PDF":
                //    // Do Something when 'Choose From File' Button is pressed
                //    var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                //    {
                //        { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
                //        { DevicePlatform.Android, new[] { "application/pdf" } },
                //        { DevicePlatform.UWP, new[] { ".pdf" } },
                //        { DevicePlatform.Tizen, new[] { "*/*" } },
                //        { DevicePlatform.macOS, new[] { "pdf" } }, // or general UTType values
                //    });

                //    var pickResult = await FilePicker.PickAsync(new PickOptions
                //    {
                //        FileTypes = customFileType,
                //        PickerTitle = "Pick a file"
                //    });

                //    if (pickResult != null)
                //    {
                //        string ext = Path.GetExtension(pickResult.FullPath);
                //        string filename = "permit" + DateTimeOffset.Now.ToUnixTimeSeconds();

                //        m_pdfDocumentStream = await pickResult.OpenReadAsync();
                //        var bytes = new Byte[(int)m_pdfDocumentStream.Length];

                //        m_pdfDocumentStream.Seek(0, SeekOrigin.Begin);
                //        m_pdfDocumentStream.Read(bytes, 0, (int)m_pdfDocumentStream.Length);

                //        isImage = false;
                //        isPdf = true;


                //        TempAttachment = new Doclinks
                //        {
                //            fileName = filename + ".pdf",
                //            title = filename,
                //            ownertable = "PLUSGPERMITWORK",
                //            documentdata = Convert.ToBase64String(bytes),
                //            previewdoc = Base64Files.pdfimage,
                //            urlname = filename + ".pdf",
                //            urltype = "FILE",
                //            doctype = "Attachments",
                //            _action = "Add",
                //            createdate = DateTime.Now.ToLocalTime(),
                //            modified = DateTime.Now.ToLocalTime(),
                //        };
                //        if (ext.ToLower() == ".pdf")
                //        {
                //            TempAttachment.previewdoc = Base64Files.pdfimage;
                //        }
                //        await Navigation.PushModalAsync(new AddAttachmentPermit(this));

                //    }
                //    break;

                case "Take PDF":
                    try
                    {
                        // Request storage permissions
                        var storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();

                        if (storageStatus != PermissionStatus.Granted)
                        {
                            await Application.Current.MainPage.DisplayAlert("Permission Denied", "Storage access is required.", "Ok");
                            return;
                        }

                        // Define custom file type for PDF
                        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
            { DevicePlatform.Android, new[] { "application/pdf" } },
            { DevicePlatform.WinUI, new[] { ".pdf" } },
            { DevicePlatform.Tizen, new[] { "*/*" } },
            { DevicePlatform.macOS, new[] { "pdf" } },
        });

                        // Pick a PDF file
                        var pickResult = await FilePicker.PickAsync(new PickOptions
                        {
                            FileTypes = customFileType,
                            PickerTitle = "Pick a PDF file"
                        });

                        if (pickResult == null)
                            return;

                        // Define filename
                        string filename = "permit" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".pdf";

                        // Read file bytes
                        byte[] fileBytes;
                        using (var stream = await pickResult.OpenReadAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        // Convert to Base64
                        string base64File = Convert.ToBase64String(fileBytes);

                        // Prepare attachment object
                        isImage = false;
                        isPdf = true;

                        TempAttachment = new Doclinks
                        {
                            fileName = filename,
                            title = Path.GetFileNameWithoutExtension(filename),
                            ownertable = "PLUSGPERMITWORK",
                            documentdata = base64File,
                            previewdoc = Base64Files.pdfimage,
                            urlname = filename,
                            urltype = "FILE",
                            _action = "Add",
                            doctype = "Attachments",
                            modified = DateTime.Now,
                            createdate = DateTime.Now,
                        };

                        // Set preview document for PDFs
                        if (Path.GetExtension(pickResult.FullPath).ToLower() == ".pdf")
                        {
                            TempAttachment.previewdoc = Base64Files.pdfimage;
                        }

                        // Navigate to the attachment page
                        await Application.Current.MainPage.Navigation.PushModalAsync(new AddAttachmentPermit(this));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await Application.Current.MainPage.DisplayAlert("Error", "Please check permission to pick a PDF, close app and try again.", "Ok");
                    }
                    break;

            }
        }

        private async Task SaveAttachment()
        {
            /*if (TempAttachment.description == null)
            {
                await UserDialogs.Instance.AlertAsync("Please insert description before save", "Validate!", "Ok");
            }
            else
            {*/
                //Minus Add to sqlite after save permit
                TempAttachment.document = TempAttachment.description;
                permit.doclinks.Add(TempAttachment);
                AttachmentListData.Add(TempAttachment);
                await UserDialogs.Instance.AlertAsync("Your Attachment has been store in this Permit", "Success!", "Ok");
                await Navigation.PopModalAsync();
            //}
        }

        private async Task DeleteAttachment()
        {
            //Logic if attachment has maxid should be isdelete else remove from temp
            if (TempAttachment.docinfoid != null)
            {
                TempAttachment.isdelete = true;
                TempAttachment._action = "Delete";
            }
            else
            {
                permit.doclinks.Remove(TempAttachment);
            }

            AttachmentListData.Remove(TempAttachment);
            await UserDialogs.Instance.AlertAsync("Your Attachment has been delete in this Permit", "Success!", "Ok");
            await Navigation.PopModalAsync();
        }

        async void SelectedDoclinks(object obj)
        {
            SfListView selected = obj as SfListView;
            Doclinks doc = selected.SelectedItem as Doclinks;
            selected.SelectedItem = null;
            TempAttachment = doc;
            if (m_pdfDocumentStream != null)
            {
                m_pdfDocumentStream.Close();
                m_pdfDocumentStream = null;
            }
            isImage = false;
            isPdf = false;
            base64Attachment = doc.documentdata;
            if (doc.documentdata == null)
            {
                isDownload = false;
                isNoDownload = true;
            }
            else
            {
                if (Base64Files.GetFileExtension(doc.documentdata) == "pdf")
                {
                    byte[] _fromBase64String = Convert.FromBase64String(doc.documentdata);
                    string filename = doc.fileName;
                    string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                    await Task.Delay(1000);
                    m_pdfDocumentStream = File.OpenRead(path);
                    isImage = false;
                    isPdf = true;
                }
                else
                {
                    isImage = true;
                    isPdf = false;
                }
                isDownload = true;
                isNoDownload = false;
            }
            await Navigation.PushModalAsync(new DetailAttachmentPermit(this));
        }

        void LoadMxDomain()
        {
            DomainData.Clear();
            try
            {
                DomainData.Add(new MxDomain
                {
                    value = "Value 1",
                    description = "Description 1"
                });

                DomainData.Add(new MxDomain
                {
                    value = "Value 2",
                    description = "Description 2"
                });

                DomainData.Add(new MxDomain
                {
                    value = "Value 3",
                    description = "Description 3"
                });

                foreach (MxDomain mxDomain in DomainData)
                {
                    Console.WriteLine(mxDomain.value);
                    Console.WriteLine(mxDomain.description);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        async void SelectStateCommand()
        {
            //await Navigation.PushModalAsync(new DomainLookupPermits("TNBSTATE", this, "tnbstate", true));
            await Navigation.PushModalAsync(new DomainLookupPermits("TNBSTATE", this, "tnbstate", permit));
        }

        async void SelectSubzoneCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("TNBSUBZONE", this, "tnbsubzone", permit));
        }

        async void SelectPermitTypeCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("TNBPERMITTYPE", this, "tnbpermittype", permit));
        }

        async void SelectBussinessAreaCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("TNBBSNSAREA", this, "tnbbusinessarea", permit));
        }

        //async void SelectPermitStatusCommand()
        //{
        //    await Navigation.PushModalAsync(new DomainLookupPermits("PLUSGPERWORKSTATUS", this, "tnbstatus", permit));
        //}

        async void SelectUnitCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("TNBUNITGRP", this, "tnbunit", permit));
        }

        async void SelectIssueByCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("", this, "tnbissuedby", permit));
        }

        async void SelectIssueToCommand()
        {
            await Navigation.PushModalAsync(new DomainLookupPermits("", this, "tnbissuedto", permit));
        }

        public async Task downloadAttachment()
        {

            bool answer = await UserDialogs.Instance.ConfirmAsync("Want to download it to your gallery?", "Attention", "Yes", "No");
            if (answer)
            {
                if (Base64Files.GetFileExtension(TempAttachment.documentdata) == "pdf")
                {
                    byte[] _fromBase64String = Convert.FromBase64String(TempAttachment.documentdata);
                    string filename = "permit-xam-attachment-pdf" + TempAttachment.document;
                    string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                }
                else
                {
                    byte[] _fromBase64String = Convert.FromBase64String(TempAttachment.documentdata);
                    string filename = "permit-xam-attachment-image" + TempAttachment.document;
                    string path = savefile.SaveBinaryImg(filename, _fromBase64String);
                    //byte[] _fromBase64String = Convert.FromBase64String(selectedDoclink.documentdata);
                    //string filename = "xam-attachment-image" + workorder.wonum;
                    //byte[] b = File.ReadAllBytes(file.Path);
                    //String Base64File = Convert.ToBase64String(b);
                    //var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    //var filePath = Path.Combine(folderPath, filename + ".jpg");
                    //File.WriteAllBytes(filePath, b);
                }

                await UserDialogs.Instance.AlertAsync("Attachment Successfully Downloaded", "Success!", "Ok");
            }
        }
    }
}
