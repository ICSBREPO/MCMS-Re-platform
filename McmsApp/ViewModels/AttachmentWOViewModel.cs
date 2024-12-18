using System.Diagnostics;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.General;
using McmsApp.Helpers;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.ImagePreview;
using McmsApp.Views.Login;
using McmsApp.Views.Work.WorkDetail.Attachment;
using Microsoft.Maui.Media;
using Syncfusion.Maui.ListView;

namespace McmsApp.ViewModels
{
    public class AttachmentWOViewModel : BaseViewModel
    {
        // Navigation From Parent
        INavigation Navigation { get; set; }
        TestFormViewModel testform;

        // Binding View
        public Workorder workorder { get; set; }
        public Doclinks selectedDoclink { get; set; }
        public Stream m_pdfDocumentStream { get; set; }
        TestFormViewModel testmodel { get; set; }
        Tnbwochecklisttype chcktype { get; set; }
        //Add new
        Tnbwochecklist chcktypelist { get; set; }

        public string DateNow { get; set; }
        public string imageTake { get; set; }
        public string subtitle { get; set; }
        public int AttachmentLisCount { get; set; }
        public bool NoAttachment { get; set; }
        public int wochecklisttypeid { get; set; }

        // Message handler
        public bool isErrorAttachmentNameEntry { get; set; }
        public bool isErrorAttachmentDescriptionEntry { get; set; }
        public string ErrorAttachmentNameEntryMessage { get; set; }
        public string ErrorAttachmentDescriptionEntryMessage { get; set; }
        public bool isAdd { get; set; }
        public bool isDelete { get; set; }
        public bool isDownload { get; set; }
        public bool isNoDownload { get;  set; }
        public bool isPdf { get; set; }
        public bool isImage { get; set; }

        // Command
        public ICommand SelectedDoclinksListCommand { get; set; }
        public ICommand OptionTakePicture { get; set; }
        public ICommand BackToListAttachment { get; set; }
        public ICommand DownloadDocumentData { get; set; }
        public ICommand SaveAttachmentCommand { get; set; }
        public ICommand DeleteAttachmentCommand { get; set; }
        public ICommand LoadAttachment { get; set; }
        public ICommand ChooseToDeleteAttachment { get; set; }
        public ICommand DeleteAttachment { get; set; }
        public ICommand ChooseToEditAttachment { get; set; }
        public ICommand DownloadAttachment { get; set; }

        // SQL
        IDoclinks sqldoc = new SQLiteDoclinks();

        // Rest api
        IMaximoApi maxrest = new MaximoApi();

        // pdf creator
        public ISaveFile savefile = new SaveFile();

        //change in here
        public ProgressBarUpdate progressBarUpdate;
        public AttachmentWOViewModel(Workorder wo, INavigation navigation, ProgressBarUpdate progressBarUpdate=null, Tnbwochecklist chcktype =null, int tnbwochecklisttypeid=0)
        {

            //change in here
           this.chcktypelist = chcktype;
            Navigation = navigation;
            workorder = wo;
            this.progressBarUpdate = progressBarUpdate;


            wochecklisttypeid = tnbwochecklisttypeid;
            SelectedDoclinksListCommand = new Command(async (param) => await SelectedDoclink(param));
            

            OptionTakePicture = new Command(async (param) => await PopActionSheet());
            BackToListAttachment = new Command(backPage);
            DownloadDocumentData = new Command(downloadDocumentData);
            SaveAttachmentCommand = new Command(saveAttachment);
            DeleteAttachmentCommand = new Command(deleteAttachment);
            LoadAttachment = new Command(loadAttachment);
            DownloadAttachment = new Command(async (param) => await downloadAttachment());
            DateNow = DateTime.Now.ToString();
            imageTake = "noAttachImage.png";
            AttachmentLisCount = 0;
            loadAttachment();
        }

        async Task SelectedDoclink(object obj)
        {
            SfListView selected = obj as SfListView;
            Doclinks doc = selected.SelectedItem as Doclinks;
            selected.SelectedItem = null;
            selectedDoclink = doc;
            isDelete = true;
            isImage = false;
            isPdf = false;
            if (doc.docinfoid == null || doc.docinfoid == 0)
            {
                isAdd = true;
            }
            else
            {
                isAdd = false;
            }
            if (doc.documentdata == null)
            {
                isDownload = false;
                isNoDownload = true;
            }
            else
            {
                if (Base64Files.GetFileExtension(doc.documentdata) == "pdf")
                {
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
            subtitle = "Detail Attachment";
            await Navigation.PushModalAsync(new AddAttachment(this));
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
                    Doclinks doclinks = await maxrest.GetDoclinks(selectedDoclink.doclinksid);
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
                        selectedDoclink.documentdata = doclinks.documentdata;
                        selectedDoclink.previewdoc = doclinks.documentdata;
                        if (Base64Files.GetFileExtension(selectedDoclink.documentdata) == "pdf")
                        {
                            isImage = false;
                            isPdf = true;
                            selectedDoclink.previewdoc = Base64Files.pdfimage;
                        }
                        else
                        {
                            isImage = true;
                            isPdf = false;
                            selectedDoclink.previewdoc = selectedDoclink.documentdata;
                        }
                        await sqldoc.UpdateDoclinks(selectedDoclink);
                        await UserDialogs.Instance.AlertAsync("Attachment Successfully Downloaded", "Success!", "Ok");
                    }
                }
                else
                {
                    UserDialogs.Instance.HideHud();
                    await UserDialogs.Instance.AlertAsync("Failed to download attachment, please try again later!", "Warning!", "Ok");
                }
            }
            catch(Exception e)
            {
                await UserDialogs.Instance.AlertAsync($"Error : {e.Message}", "Warning", "OK");
                UserDialogs.Instance.HideHud();
            }
        }


        async void loadAttachment()
        {

            /*//for updating the documents count on testformtype
            if(chcktype !=null)
            {
                chcktype.totalAttachment = (int)await sqldoc.CountDoclinks(workorder.id, "WORKORDER", chcktype.tnbwochecklisttypeid.ToString());
                Debug.WriteLine("Checklist Attachment : chcktype" + chcktype);
            }
            else
            {

                workorder.doclinks = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER", false, wochecklisttypeid.ToString());
            }*/
            workorder.doclinks = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER", false, wochecklisttypeid.ToString());
            UserDialogs.Instance.ShowLoading("Loading...");
            if(wochecklisttypeid == 0)
            {
                workorder.doclinks = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER",false,"*0");

               /* Debug.WriteLine("wochecklisttypeid is Zero : "+ wochecklisttypeid);*/
            }
            else
            {
                workorder.doclinks = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER", false, wochecklisttypeid.ToString());
                /*Debug.WriteLine("wochecklisttypeid is not  : " + wochecklisttypeid);*/

            }

           


            AttachmentLisCount = workorder.doclinks.Count;
            if (wochecklisttypeid != 0)

                chcktypelist.totalAttachment = AttachmentLisCount;
            
            
            //testmodel = new TestFormViewModel(workorder, Navigation);
            // testmodel.SelectedPSIChecklistType(chcktype);


            if (workorder.doclinks.Count < 1)
            {
                NoAttachment = true;
            }
            else
            {
                NoAttachment = false;
                foreach(Doclinks doc in workorder.doclinks)
                {
                    if (doc.pendingupload)
                    {
                        doc.badgeicon = Syncfusion.Maui.Core.BadgeIcon.Away;
                    }
                    else
                    {
                        doc.badgeicon = Syncfusion.Maui.Core.BadgeIcon.None;
                    }
                    if (doc.documentdata == null)
                    {
                        doc.previewdoc = Base64Files.downloadimage;
                    }
                    else
                    {
                        if (Base64Files.GetFileExtension(doc.documentdata) == "pdf")
                        {
                            doc.previewdoc = Base64Files.pdfimage;
                        }
                        else
                        {
                            doc.previewdoc = doc.documentdata;
                        }
                    }
                    
                }
                AttachmentLisCount = workorder.doclinks.Count;
                

            }
            UserDialogs.Instance.HideHud();
        }

        private void backPage()
        {
            Navigation.PopModalAsync();
           loadAttachment();
        }

        async Task PopActionSheet()
        {
            var choices = new[] { "Take Photo", "Choose Photo From Gallery", "Choose PDF" };

            var choice = await UserDialogs.Instance.ActionSheetAsync("Option", "Cancel", null, null, choices);
            if (choice == "Take Photo")
            {
                await takePictureFromCamera();
            }
            else if (choice == "Choose Photo From Gallery")
            {
                await takePictureFromDevices();
            }
            else if (choice == "Choose PDF")
            {
                await takeFile();
            }
        }

        private async Task takeFile()
        {
            // Do Something when 'Choose From File' Button is pressed
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
                { DevicePlatform.Android, new[] { "application/pdf" } },
                { DevicePlatform.UWP, new[] { ".pdf" } },
                { DevicePlatform.Tizen, new[] { "*/*" } },
                { DevicePlatform.macOS, new[] { "pdf" } }, // or general UTType values
            });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Pick a pdf"
            });


            if (pickResult != null)
            {
                string filename = "xamwo-" + workorder.wonum + DateTimeOffset.Now.ToUnixTimeSeconds();
                isImage = false;
                isPdf = true;
                Stream tempstream = await pickResult.OpenReadAsync();
                var bytes = new Byte[(int)tempstream.Length];
                tempstream.Seek(0, SeekOrigin.Begin);
                tempstream.Read(bytes, 0, (int)tempstream.Length);

                if (wochecklisttypeid != 0)
                {
                    selectedDoclink = new Doclinks
                    {
                        document = chcktypelist.tnbtype,
                        description = chcktypelist.tnbtype,
                        fileName = filename + ".pdf",
                        title = filename,
                        refid = workorder.id,
                        ownertable = "WORKORDER",
                        reference = wochecklisttypeid.ToString(),
                        documentdata = Convert.ToBase64String(bytes),
                        urlname = filename + ".pdf",
                        urltype = "FILE",
                        doctype = "Attachments",
                        createdate = DateTime.Now.ToLocalTime(),
                        modified = DateTime.Now.ToLocalTime(),
                        previewdoc = Base64Files.pdfimage
                    };
                }
                else
                {
                    selectedDoclink = new Doclinks
                    {
                        fileName = filename + ".pdf",
                        title = filename,
                        refid = workorder.id,
                        ownertable = "WORKORDER",
                        reference = wochecklisttypeid.ToString(),
                        documentdata = Convert.ToBase64String(bytes),
                        urlname = filename + ".pdf",
                        urltype = "FILE",
                        doctype = "Attachments",
                        createdate = DateTime.Now.ToLocalTime(),
                        modified = DateTime.Now.ToLocalTime(),
                        previewdoc = Base64Files.pdfimage
                    };
                }
                
                isDownload = true;
                isNoDownload = false;
                isAdd = true;
                isDelete = false;
                subtitle = "Add Attachment";
                await Navigation.PushModalAsync(new AddAttachment(this));

            }
        }

        private async Task takePictureFromCamera()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await UserDialogs.Instance.AlertAsync(":(", "Camera Not Available", "Ok");
                    return;
                }

                string filename = "xamwo-" + workorder.wonum + DateTimeOffset.Now.ToUnixTimeSeconds();

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    AllowCropping = false,
                    Directory = "MCMS",
                    Name = filename,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    CompressionQuality = 92,
                    SaveToAlbum = true
                }) ;

                if (file == null)
                {
                    return;
                }

                byte[] b = File.ReadAllBytes(file.Path);
                String Base64File = Convert.ToBase64String(b);
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(folderPath, filename + ".jpg");
                File.WriteAllBytes(filePath, b);

                isImage = true;
                isPdf = false;

                if (wochecklisttypeid != 0)
                {
                    selectedDoclink = new Doclinks
                    {
                        document = chcktypelist.tnbtype,
                        description = chcktypelist.tnbtype,
                        fileName = filename + ".jpg",
                        title = filename,
                        refid = workorder.id,
                        reference = wochecklisttypeid.ToString(),
                        ownertable = "WORKORDER",
                        documentdata = Base64File,
                        urlname = filename + ".jpg",
                        urltype = "FILE",
                        doctype = "Attachments",
                        createdate = DateTime.Now.ToLocalTime(),
                        modified = DateTime.Now.ToLocalTime(),
                        previewdoc = Base64File
                    };
                }
                else
                {
                    selectedDoclink = new Doclinks
                    {
                        fileName = filename + ".jpg",
                        title = filename,
                        refid = workorder.id,
                        reference = wochecklisttypeid.ToString(),
                        ownertable = "WORKORDER",
                        documentdata = Base64File,
                        urlname = filename + ".jpg",
                        urltype = "FILE",
                        doctype = "Attachments",
                        createdate = DateTime.Now.ToLocalTime(),
                        modified = DateTime.Now.ToLocalTime(),
                        previewdoc = Base64File
                    };
                }
                isDownload = true;
                isNoDownload = false;
                isAdd = true;
                isDelete = false;
                subtitle = "Add Attachment";
                await Navigation.PushModalAsync(new AddAttachment(this));

            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("Please Check Camera Permission, Close App And Open Again", "Alert!", "Ok");
            }
        }

        //Picture choose from device    
        private async Task takePictureFromDevices()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await UserDialogs.Instance.AlertAsync("No Gallery Detected");
                    return;
                }
                else
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                    });

                    if (file == null)
                    {
                        return;
                    }
                    string filename = "xamwo-" + workorder.wonum + DateTimeOffset.Now.ToUnixTimeSeconds();
                    byte[] b = File.ReadAllBytes(file.Path);
                    String Base64File = Convert.ToBase64String(b);
                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var filePath = Path.Combine(folderPath, filename + ".jpg");
                    File.WriteAllBytes(filePath, b);

                    isImage = true;
                    isPdf = false;

                    if(wochecklisttypeid != 0)
                    {
                        selectedDoclink = new Doclinks
                        {
                            document = chcktypelist.tnbtype,
                            description = chcktypelist.tnbtype,
                            fileName = filename + ".jpg",
                            title = filename,
                            reference = wochecklisttypeid.ToString(),
                            refid = workorder.id,
                            ownertable = "WORKORDER",
                            documentdata = Base64File,
                            urlname = filename + ".jpg",
                            urltype = "FILE",
                            doctype = "Attachments",
                            createdate = DateTime.Now.ToLocalTime(),
                            modified = DateTime.Now.ToLocalTime(),
                            previewdoc = Base64File
                        };
                    }
                    else
                    {
                        selectedDoclink = new Doclinks
                        {
                            fileName = filename + ".jpg",
                            title = filename,
                            reference = wochecklisttypeid.ToString(),
                            refid = workorder.id,
                            ownertable = "WORKORDER",
                            documentdata = Base64File,
                            urlname = filename + ".jpg",
                            urltype = "FILE",
                            doctype = "Attachments",
                            createdate = DateTime.Now.ToLocalTime(),
                            modified = DateTime.Now.ToLocalTime(),
                            previewdoc = Base64File
                        };
                    }
                    
                    isDownload = true;
                    isNoDownload = false;
                    isAdd = true;
                    isDelete = false;
                    subtitle = "Add Attachment";
                    await Navigation.PushModalAsync(new AddAttachment(this));
                }
            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("Please Check Permission to Pick From Galery, Close App And Open Again", "Alert!", "Ok");
            }

        }

        //Delete
        async void deleteAttachment()
        {
            UserDialogs.Instance.ShowLoading("Deleting Attachment...");
            if (selectedDoclink.docinfoid == 0 || selectedDoclink.docinfoid == null)
            {
                UserDialogs.Instance.HideHud();
                selectedDoclink.pendingupload = false;
                await sqldoc.DeleteDoclinks(selectedDoclink);
            }
            else
            {
                workorder.changedate = DateTime.Now.ToLocalTime();
                workorder.reportdate = null;
                workorder.statusdate = null;
                workorder.doclinks = new List<Doclinks>();
                selectedDoclink._action = "Delete";
                workorder.doclinks.Add(selectedDoclink);

                Workorder delDoclink = await maxrest.UpdateWO(workorder, "workorderid");
                UserDialogs.Instance.HideHud();
                if (delDoclink == null)
                {
                    selectedDoclink.isdelete = true;
                    await sqldoc.UpdateDoclinks(selectedDoclink);
                    await UserDialogs.Instance.AlertAsync("Attachment Successfully Deleted but Pending upload to Maximo", "Success!", "Ok");

                }
                else if (delDoclink.Error != null)
                {
                    await UserDialogs.Instance.AlertAsync(delDoclink.Error.message, "Failed!", "Ok");
                    if (delDoclink.Error.reasonCode == "BMXAA9549E")
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
                    await sqldoc.DeleteDoclinks(selectedDoclink);
                    await UserDialogs.Instance.AlertAsync("Attachment Successfully Deleted", "Success!", "Ok");
                }
            }
            loadAttachment();

            //for progress update
            if (progressBarUpdate != null)
            {


                Debug.WriteLine("I attachment save method insdie AttachmentWoViewModel.cs");

                await progressBarUpdate.LoadProgress(workorder);
            }
            await Navigation.PopModalAsync();
        }

        // Save
        async void saveAttachment()
        {
            try
            {
                isErrorAttachmentNameEntry = false;
                isErrorAttachmentDescriptionEntry = false;

                if (selectedDoclink.title.Length == 0)
                {
                    ErrorAttachmentNameEntryMessage = "Attachment name cannot be empty!";
                    isErrorAttachmentNameEntry = true;
                }

                /*if (selectedDoclink.description.Length == 0)
                {
                    ErrorAttachmentDescriptionEntryMessage = "Attachment description cannot be empty!";
                    isErrorAttachmentDescriptionEntry = true;
                }*/

                if (isErrorAttachmentNameEntry == false && isErrorAttachmentDescriptionEntry == false)
                {
                    UserDialogs.Instance.ShowLoading("Uploading Attachment...");
                    await Task.Delay(1000);
                    bool check = await maxrest.Ping();
                    if (check)
                    {
                        Workorder updatewo = new Workorder();
                        updatewo.doclinks = new List<Doclinks>();
                        updatewo.doclinks.Add(selectedDoclink);
                        updatewo.workorderid = workorder.workorderid;
                        Workorder uploadDoclink = await maxrest.UpdateWO(updatewo, "doclinks");
                        UserDialogs.Instance.HideHud();
                        if (uploadDoclink == null)
                        {
                            selectedDoclink.pendingupload = true;
                            await sqldoc.AddDoclinks(selectedDoclink);
                            await UserDialogs.Instance.AlertAsync("Attachment Successfully Added but Pending upload to Maximo", "Success!", "Ok");
                        }
                        else if (uploadDoclink.Error != null)
                        {
                            await UserDialogs.Instance.AlertAsync(uploadDoclink.Error.message, "Failed!", "Ok");
                            if (uploadDoclink.Error.reasonCode == "BMXAA9549E")
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

                            List<Doclinks> doclinks = await sqldoc.GetDoclinksFilter(workorder.id, "WORKORDER");
                            foreach (Doclinks doc in doclinks)
                            {
                                await sqldoc.DeleteDoclinks(doc);
                            }
                            foreach (Doclinks doc in uploadDoclink.doclinks)
                            {
                                doc.refid = workorder.id;
                                await sqldoc.AddDoclinks(doc);
                            }
                            await UserDialogs.Instance.AlertAsync("Attachment Successfully Uploaded to Maximo", "Success!", "Ok");
                            
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.HideHud();
                        selectedDoclink.pendingupload = true;
                        await sqldoc.AddDoclinks(selectedDoclink);
                        await UserDialogs.Instance.AlertAsync("Attachment Successfully Added but Pending upload to Maximo", "Success!", "Ok");
                        
                    }
                    loadAttachment();


                    if(progressBarUpdate !=null)
                    {


                        Debug.WriteLine("I attachment save method insdie AttachmentWoViewModel.cs");

                       await progressBarUpdate.LoadProgress(workorder);
                    }


                    await Navigation.PopModalAsync();
                }
            }
            catch(Exception e)
            {
                UserDialogs.Instance.HideHud();
                await UserDialogs.Instance.AlertAsync($"{e.Message}, Please Try Again", "Error!", "Ok");
            }
            
        }

        public async Task downloadAttachment()
        {

            bool answer = await UserDialogs.Instance.ConfirmAsync("Want to save it to your gallery?", "Attention", "Yes", "No");
            if (answer)
            {
                /*byte[] _fromBase64String = Convert.FromBase64String(selectedDoclink.documentdata);
                string path = savefile.SaveBinaryPdf(selectedDoclink.fileName, _fromBase64String);
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(path),
                    PresentationSourceBounds = DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Tablet
                        ? new System.Drawing.Rectangle(0, 20, 0, 0)
                        : System.Drawing.Rectangle.Empty
                });*/
                /* await UserDialogs.Instance.AlertAsync("Attachment Successfully Downloaded", "Success!", "Ok");*/

                if (Base64Files.GetFileExtension(selectedDoclink.documentdata) == "pdf")
                {
                    byte[] _fromBase64String = Convert.FromBase64String(selectedDoclink.documentdata);
                    string filename = "xam-attachment-pdf" + selectedDoclink.document;
                    string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                }
                else
                {
                    byte[] _fromBase64String = Convert.FromBase64String(selectedDoclink.documentdata);
                    string filename = "xam-attachment-image" + selectedDoclink.document;
                    string path = savefile.SaveBinaryImg(filename, _fromBase64String);
                }

                await UserDialogs.Instance.AlertAsync("Attachment successfully saved", "Success!", "Ok");
            }
        }

    }
}
