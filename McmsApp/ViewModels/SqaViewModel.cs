using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.General;
using McmsApp.Helpers;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Home;
using McmsApp.Views.ImagePreview;
using McmsApp.Views.Login;
using McmsApp.Views.Work;
using McmsApp.Views.Work.WorkDetail.SQA;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.TabView;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Media;

namespace McmsApp.ViewModels
{
    public class SqaViewModel : BaseViewModel
    {
        Global global = Global.Instance;
        //Rest API
        IMaximoApi maxrest = new MaximoApi();
        //SQLite
        public IPlusgaudit sqlsqa = new SQLitePlusgaudit();
        public IPlusgaudline sqlplusgaud = new SQLitePlusgaudline();
        public IDoclinks sqldocs = new SQLiteDoclinks();
        public ITnbsignature sqlsignature = new SQLiteTnbsignature();

        //Command
        public ICommand SelectionSignatureCommand { get; set; }
        public ICommand DeleteSignatureCommand { get; set; }
        public ICommand SaveSignatureCommand { get; set; }
        public ICommand AddSqaCommand { get; set; }
        public ICommand SaveSqaCommand { get; set; }
        public ICommand BackPopAsyncCommand { get; set; }
        public ICommand ChangeSqaStatusCommand { get; set; }
        public ICommand SaveStatusSQACommand { get; set; }
        public ICommand NewSQAStatusCommand { get; set; }
        public ICommand SelectedSQAListCommand { get; set; }
        public ICommand PersonLookupCommand { get; set; }
        public ICommand DownloadDocumentData { get; set; }
        public ICommand OptionAttachments { get; set; }
        public ICommand ViewAttachmentsCommand { get; set; }
        public ICommand ViewDigitalSignatureCommand { get; set; }
        public ICommand AddDigitalSignatureCommand { get; set; }
        public ICommand SelectedDoclinksListCommand { get; set; }
        public ICommand AttachDoclinksCommand { get; set; }
        public ICommand SaveDoclinksCommand { get; private set; }
        public ICommand DeleteDoclinksCommand { get; private set; }
        public ICommand BackToSqaListCommand { get; set; }
        public ICommand BackModalCommand { get; set; }
        public List<Plusgaudit> SQAListData { get; private set; }
        public ObservableCollection<Doclinks> AttachmentListData { get; set; }
        public string AttachmentsCount { get; set; }
        public ObservableCollection<string> StatusType { get; set; }
        public ObservableCollection<string> SQAStatus { get; set; }
        public List<Plusgaudit> TemplateSQAData { get; set; }
        public Plusgaudit SelectedSQA { get; set; }
        public string QuestionsCount { get; set; }
        public string SelectedSQAStatus { get; set; }
        public ICommand PreviewImage { get; set; }
        public Tnbsignature SelectedSignature { get; set; }

        //View Binding
        public Workorder workorder { get; set; }
        public int HeightQuestionSQA { get; private set; }
        public int sqacount { get; set; }
        public bool pickerSqaOpen { get; set; }
        public bool NoSQA { get; set; }
        public bool isSave { get; set; }
        public bool signvisible { get; set; }
        public bool imgvisible { get; set; }
        public bool hidesavesignature { get; set; }
        public double SQATotalScore { get; set; }
        public double SQATotalPercentage { get; set; }
        public Doclinks TempAttachment { get; set; }
        public string selectedIndicator { get; set; }
        public bool isPdf { get; set; }
        public bool isImage { get; set; }
        public Stream m_pdfDocumentStream { get; set; }

        //Navigation From Parent
        INavigation Navigation { get; set; }
        public bool isDownload { get; private set; }
        public bool isNoDownload { get; private set; }
        public ProgressBarUpdate progressBarUpdate;

        // pdf creator
        public ISaveFile savefile = new SaveFile();
        public HomeViewModel homeviewmodel;
        SfTabView sftabview;
        public ICommand DownloadAttachment { get; set; }
        public string base64Attachment { get; set; }

        public SqaViewModel(Workorder wo, INavigation navigation, SfTabView sftabView, ProgressBarUpdate progressBarUpdate)
        {
            SQAStatus = new ObservableCollection<string>();
            SQAStatus.Add("DRAFT");
            SQAStatus.Add("ENTRY");

            SelectedSQAStatus = "DRAFT";

            Navigation = navigation;
            workorder = wo;
            this.progressBarUpdate = progressBarUpdate;
            ChangeSqaStatusCommand = new Command(SqaStatus);
            SaveStatusSQACommand = new Command(SaveSqaStatus);
            NewSQAStatusCommand = new Command(async () => await SqaStatusView());
            AddSqaCommand = new Command(SqaTemplateView);
            SaveSqaCommand = new Command(SaveSQA);
            BackPopAsyncCommand = new Command(BackPopAsync);
            BackToSqaListCommand = new Command(BackToSqaList);
            BackModalCommand = new Command(BackModal);
            SelectedSQAListCommand = new Command(SelectedSQAList);
            PersonLookupCommand = new Command(lookupPerson);
            //Attachment
            DownloadDocumentData = new Command(downloadDocumentData);
            OptionAttachments = new Command(async () => await PopActionSheet());
            ViewAttachmentsCommand = new Command(viewAttachments);
            ViewDigitalSignatureCommand = new Command(viewDigitalSignature);
            AddDigitalSignatureCommand = new Command(addDigitalSignature);
            SelectionSignatureCommand = new Command(SelectedSignatureDetail);
            SaveSignatureCommand = new Command(async () => await SaveDigitalSignature());
            DeleteSignatureCommand = new Command(DeleteSignature);
            SelectedDoclinksListCommand = new Command(SelectedDoclinks);
            AttachDoclinksCommand = new Command(async () => await takePictureFromDevices());
            SaveDoclinksCommand = new Command(async () => await SaveAttachment());
            DeleteDoclinksCommand = new Command(async () => await DeleteAttachment());
            sftabview = sftabView;
            DownloadAttachment = new Command(async () => await downloadAttachment());
            base64Attachment = "";
            _ = LoadSQA();
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
                            string filename = TempAttachment.fileName; 
                            string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                            await Task.Delay(1000);
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
            string param = "person";
            if (selectedIndicator == "tnblabel")
            {
                param = "tnblabel";
            }
            await Navigation.PushModalAsync(new LookupUserModal(this, "sqa", Navigation,param));
        }

        async void viewAttachments()
        {
            await Navigation.PushAsync(new SQAAttachment(this));
        }

        async void viewDigitalSignature()
        {
            await LoadSignatureList();
            await Navigation.PushAsync(new DigitalSignatureList(this));
        }

        async void addDigitalSignature()
        {
            SelectedSignature = new Tnbsignature();
            SelectedSignature.tnbsignatureid = null;
            SelectedSignature.tnbownerid = SelectedSQA.plusgauditid;
            SelectedSignature.refid = SelectedSQA.id;
            SelectedSignature.tnbownertable = "PLUSGAUDIT";
            SelectedSignature.tnbsigdate = DateTime.Now;
            hidesavesignature = true;
            signvisible = true;
            imgvisible = false;
            await Navigation.PushAsync(new DigitalSignatureForm(this));
        }

        async void SelectedSignatureDetail(object obj)
        {
            SfListView selected = obj as SfListView;
            SelectedSignature = selected.SelectedItem as Tnbsignature;
            selected.SelectedItem = null;
            if (SelectedSignature.signature != null && SelectedSignature.signature != "" && SelectedSignature.pendingupdate == false)
            {
                hidesavesignature = false;
                signvisible = false;
                imgvisible = true;

            }
            else
            {
                hidesavesignature = true;
                signvisible = true;
                imgvisible = false;
            }
            await Navigation.PushAsync(new DigitalSignatureForm(this));
        }

        async Task PopActionSheet()
        {
            var choices = new[] { "Take Photo", "Choose Photo From Gallery", "Choose PDF" };

            var choice = await UserDialogs.Instance.ActionSheetAsync("Option", "Cancel", null, null, choices);
            if(choice == "Take Photo")
            {
                await takePictureFromCamera();
            }
            else if(choice == "Choose Photo From Gallery")
            {
                await takePictureFromDevices();
            }
            else if(choice == "Choose PDF")
            {
                await takeFileFromFolder();
            }
        }

        async void SelectedSQAList(object obj)
        {
            Debug.WriteLine("Item template is Clicked");
            
            


            SfListView selected = obj as SfListView;
            Plusgaudit sqa = selected.SelectedItem as Plusgaudit;
            selected.SelectedItem = null;
            SelectedSQA = sqa;
            if (SelectedSQA.id == 0)
            {
                int? lastid = await sqlsqa.LastSQA();
                SelectedSQA.id = (int)lastid + 1;
            }
            SelectedSQA.tnbsignature = new List<Tnbsignature>();
            QuestionsCount = "("+sqa.plusgaudline.Count+")";
            CalculateScore();
            AttachmentListData = new ObservableCollection<Doclinks>();
            sqa.doclinks = await sqldocs.GetDoclinksFilter(sqa.id, "PLUSGAUDIT");
            
            foreach (Doclinks doc in sqa.doclinks)
            {
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
                        doc.previewdoc = doc.documentdata;
                        isImage = true;
                        isPdf = false;
                    }
                }

                AttachmentListData.Add(doc);
            }
            if (sqa.status == "DRAFT")
            {
                isSave = true;
            }
            else
            {
                isSave = false;
            }
            AttachmentsCount = AttachmentListData.Count.ToString();
            await LoadSignatureList();
            await Navigation.PushAsync(new SQAForm(this));
        }

        async Task LoadSQA()
        {
            SQAListData = await sqlsqa.GetPlusgauditByWO(workorder.wonum);
            if (SQAListData.Count < 1)
            {
                NoSQA = true;
            }
            else
            {
                NoSQA = false;
                foreach (Plusgaudit sqa in SQAListData)
                {
                    if (sqa.pendingupload)
                    {
                        sqa.badgeicon = Syncfusion.Maui.Core.BadgeIcon.Away;
                    }
                    else
                    {
                        sqa.badgeicon = Syncfusion.Maui.Core.BadgeIcon.None;
                    }

                    sqa.plusgaudline = await sqlplusgaud.GetPlusgaudlineBySQA(sqa.id);
                    sqacount = SQAListData.Count;
                    HeightQuestionSQA = (150 * sqa.plusgaudline.Count) + 50;
                }
            }
            
        }

        async Task LoadSignatureList()
        {

            if(SelectedSQA.plusgauditid !=null)
                SelectedSQA.tnbsignature = await sqlsignature.GetTnbsignaturefromPlusgauditList((int)SelectedSQA.plusgauditid,false);
            else
                SelectedSQA.tnbsignature = await sqlsignature.GetTnbsignatureByGroup(SelectedSQA.id,"PLUSGAUDIT");

            foreach (Tnbsignature tnbsignature in SelectedSQA.tnbsignature)
            {
                if (tnbsignature.pendingupdate)
                {
                    tnbsignature.badgeicon = Syncfusion.Maui.Core.BadgeIcon.Away;
                }
                else
                {
                    tnbsignature.badgeicon = Syncfusion.Maui.Core.BadgeIcon.None;
                }
            }
             SelectedSQA.totalSignature = SelectedSQA.tnbsignature.Count;
        }

        async void SqaTemplateView()
        {
            TemplateSQAData = await sqlsqa.GetPlusgauditAsync(true);
            foreach (Plusgaudit sqa in TemplateSQAData)
            {
                sqa.plusgaudline = await sqlplusgaud.GetPlusgaudlineBySQA(sqa.id);
                sqa.approvedbydate = null;
                sqa.createdbydate = null;
                sqa.statusdate = null;
                sqa.status = "DRAFT";
                sqa.plusgauditid = null;
                sqa.auditnum = null;
                sqa.createdbyid = null;
                sqa.id = 0;

                foreach (Plusgaudline sqaline in sqa.plusgaudline)
                {
                    sqaline.plusgauditid = null;
                    sqaline.plusgaudlineid = null;
                    sqaline.template = false;
                    sqaline.linenum = null;
                    sqaline.linescore = 0;
                }
                HeightQuestionSQA = (150 * sqa.plusgaudline.Count) + 50;
            }

            if (TemplateSQAData.Count > 0)
            {
                await Navigation.PushAsync(new SQATemplate(this));
            }
            else
            {
                bool answer = await UserDialogs.Instance.ConfirmAsync("Please download sqa template by sync all master data first", "Alert!", "Yes", "No");
                if (answer)
                {
                    backtoHome();
                }
            }
            
        }

        public void CalculateScore()
        {
            foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
            {
                if (qsqa.yes)
                {
                    qsqa.linescore = 1;
                }
                else if (qsqa.notapplicable)
                {
                    qsqa.linescore = 0;
                }
            }
            double yes = SelectedSQA.plusgaudline.Where(x => x.yes).Count();
            if (yes > 0)
            {
                SelectedSQA.totalscore = SelectedSQA.plusgaudline.Sum(x => x.linescore);
                double percentage = (double)((double)(yes / (double)SelectedSQA.totalscore) * 100);
                SelectedSQA.tnbpercentage = (int?)Math.Round(percentage, 0);
            }
            else
            {
                SelectedSQA.totalscore = 0;
                SelectedSQA.tnbpercentage = 0;
            }
        }

        async Task SqaStatusView()
        {
            pickerSqaOpen = true;
        }

        async void SaveSqaStatus()
        {
            SelectedSQA.status = SelectedSQAStatus;

            await Navigation.PopAsync();
        }

        async void SqaStatus()
        {
            await Navigation.PushAsync(new ChangeSqaStatus(this));
        }

        async void BackPopAsync()
        {
            await Navigation.PopAsync();
        }

        async void BackToSqaList()
        {
            await Navigation.PopAsync();
        }

        // Start Attachment

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
            await Navigation.PushModalAsync(new DetailAttachments(this));
        }

        async void BackModal()
        {
            await Navigation.PopModalAsync();
        }

        //private async Task takePictureFromCamera()
        //{
        //    try
        //    {
        //        var status = await Permissions.RequestAsync<Permissions.Camera>();
        //        await CrossMedia.Current.Initialize();

        //        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        //        {
        //            await UserDialogs.Instance.AlertAsync(":(", "Camera Not Available", "Ok");
        //            return;
        //        }

        //        string filename = "filesqa" + DateTimeOffset.Now.ToUnixTimeSeconds();

        //        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
        //        {

        //            Directory = "MCMS",
        //            Name = filename,
        //            PhotoSize = PhotoSize.MaxWidthHeight,
        //            CompressionQuality = 92,
        //            AllowCropping = false
        //        });

        //        if (file == null)
        //            return;
        //        byte[] b = File.ReadAllBytes(file.Path);
        //        String Base64File = Convert.ToBase64String(b);

        //        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //        var filePath = Path.Combine(folderPath, filename + ".jpg");
        //        File.WriteAllBytes(filePath, b);

        //        isImage = true;
        //        isPdf = false;

        //        TempAttachment = new Doclinks
        //        {
        //            fileName = filename + ".jpg",
        //            title = filename,
        //            ownertable = "PLUSGAUDIT",
        //            documentdata = Base64File,
        //            previewdoc = Base64File,
        //            urlname = filename + ".jpg",
        //            urltype = "FILE",
        //            doctype = "Attachments",
        //            modified = DateTime.Now.ToLocalTime(),
        //        };
        //        await Navigation.PushModalAsync(new AddAttachments(this));
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        await UserDialogs.Instance.AlertAsync("Please Check Camera Permission, Close App And Open Again", "Alert!", "Ok");
        //    }
        //}

        private async Task TakePictureFromCamera()
        {
            try
            {
                
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await UserDialogs.Instance.AlertAsync("Camera permission denied", "Permission Error", "Ok");
                    return;
                }

               
                if (!MediaPicker.IsCaptureSupported)
                {
                    await UserDialogs.Instance.AlertAsync(":(", "Camera Not Available", "Ok");
                    return;
                }

                //  filename Name
                string filename = "filesqa" + DateTimeOffset.Now.ToUnixTimeSeconds();

               
                var fileResult = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {

                    //Need to check later for the sizing and quality of the photo as default MAUI libaray do not support : check the using SkiaSharp;
                    //PhotoSize = PhotoSize.Medium, // You can adjust the photo size
                    //CompressionQuality = 92
                });

                if (fileResult == null)
                    return;

                
                byte[] b = await File.ReadAllBytesAsync(fileResult.FullPath);
                string base64File = Convert.ToBase64String(b);

                
                var folderPath = FileSystem.AppDataDirectory;
                var filePath = Path.Combine(folderPath, filename + ".jpg");
                await File.WriteAllBytesAsync(filePath, b);

                
                isImage = true;
                isPdf = false;

                
                TempAttachment = new Doclinks
                {
                    fileName = filename + ".jpg",
                    title = filename,
                    ownertable = "PLUSGAUDIT",
                    documentdata = base64File,
                    previewdoc = base64File,
                    urlname = filename + ".jpg",
                    urltype = "FILE",
                    doctype = "Attachments",
                    modified = DateTime.Now.ToLocalTime(),
                };

                // Navigate to the AddAttachments page
                await Navigation.PushModalAsync(new AddAttachments(this));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await UserDialogs.Instance.AlertAsync("Please Check Camera Permission, Close App And Open Again", "Alert!", "Ok");
            }
        }

        //private async Task takePictureFromDevices()
        //{
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
        //            {
        //                return;
        //            }
        //            string filename = "filesqa" + +DateTimeOffset.Now.ToUnixTimeSeconds();
        //            byte[] b = File.ReadAllBytes(file.Path);
        //            String Base64File = Convert.ToBase64String(b);
        //            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //            var filePath = Path.Combine(folderPath, filename + ".jpg");
        //            File.WriteAllBytes(filePath, b);

        //            isImage = true;
        //            isPdf = false;

        //            TempAttachment = new Doclinks
        //            {
        //                fileName = filename + ".jpg",
        //                title = filename,
        //                ownertable = "PLUSGAUDIT",
        //                previewdoc = Base64File,
        //                documentdata = Base64File,
        //                urlname = filename + ".jpg",
        //                urltype = "FILE",
        //                doctype = "Attachments",
        //                modified = DateTime.Now.ToLocalTime(),
        //            };
        //            await Navigation.PushModalAsync(new AddAttachments(this));
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        await UserDialogs.Instance.AlertAsync("Please Check Permission to Pick From Galery, Close App And Open Again", "Alert!", "Ok");
        //    }

        //}
        private async Task TakePictureFromDevices()
        {
            try
            {
               
                var status = await Permissions.RequestAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    await UserDialogs.Instance.AlertAsync("Gallery permission denied", "Permission Error", "Ok");
                    return;
                }

                
                var fileResult = await MediaPicker.PickPhotoAsync();

                if (fileResult == null)
                {
                    await UserDialogs.Instance.AlertAsync("No photo selected", "Error", "Ok");
                    return;
                }

                
                string filename = "filesqa" + DateTimeOffset.Now.ToUnixTimeSeconds();

               
                byte[] b = await File.ReadAllBytesAsync(fileResult.FullPath);
                string base64File = Convert.ToBase64String(b);

                
                var folderPath = FileSystem.AppDataDirectory;
                var filePath = Path.Combine(folderPath, filename + ".jpg");
                await File.WriteAllBytesAsync(filePath, b);

               
                isImage = true;
                isPdf = false;

                // Create Doclinks object for the attachment
                TempAttachment = new Doclinks
                {
                    fileName = filename + ".jpg",
                    title = filename,
                    ownertable = "PLUSGAUDIT",
                    previewdoc = base64File,
                    documentdata = base64File,
                    urlname = filename + ".jpg",
                    urltype = "FILE",
                    doctype = "Attachments",
                    modified = DateTime.Now.ToLocalTime(),
                };

               
                await Navigation.PushModalAsync(new AddAttachments(this));
            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("Please Check Permission to Pick From Gallery, Close App And Open Again", "Alert!", "Ok");
            }
        }
        //private async Task takeFileFromFolder()
        //{
        //    try
        //    {
                
        //        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        //        {
        //            { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
        //            { DevicePlatform.Android, new[] { "application/pdf" } },
        //            { DevicePlatform.UWP, new[] { ".pdf" } },
        //            { DevicePlatform.Tizen, new[] { "*/*" } },
        //            { DevicePlatform.macOS, new[] { "pdf" } }, // or general UTType values
        //        });

        //        var pickResult = await FilePicker.PickAsync(new PickOptions
        //        {
        //            FileTypes = customFileType,
        //            PickerTitle = "Pick a pdf"
        //        });

        //        if (pickResult != null)
        //        {
        //            string filename = "filesqa" + DateTimeOffset.Now.ToUnixTimeSeconds();

        //            isImage = false;
        //            isPdf = true;
        //            System.Diagnostics.Debug.WriteLine(pickResult.FullPath);
        //            m_pdfDocumentStream = await pickResult.OpenReadAsync();
        //            var bytes = new Byte[(int)m_pdfDocumentStream.Length];

        //            m_pdfDocumentStream.Seek(0, SeekOrigin.Begin);
        //            m_pdfDocumentStream.Read(bytes, 0, (int)m_pdfDocumentStream.Length);
        //            TempAttachment = new Doclinks
        //            {
        //                fileName = filename+".pdf",
        //                title = filename + ".pdf",
        //                ownertable = "PLUSGAUDIT",
        //                previewdoc = Base64Files.pdfimage,
        //                documentdata = Convert.ToBase64String(bytes),
        //                urltype = "FILE",
        //                doctype = "Attachments",
        //                modified = DateTime.Now.ToLocalTime(),
        //                createdate = DateTime.Now.ToLocalTime(),
        //            };

        //            await Navigation.PushModalAsync(new AddAttachments(this));

        //        }
        //        else
        //        {
        //            await UserDialogs.Instance.AlertAsync("Opps! Looks like something wrong with your file", "Alert!", "Ok");
        //        }
        //    }
        //    catch (Exception g)
        //    {
        //        Console.WriteLine("===ERROR: " + g.Message + "===");
        //    }
        //}

        


private async Task TakeFileFromFolder()
    {
        try
        {
            // Define custom file types (e.g., PDF files) based on the platform
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
            { DevicePlatform.Android, new[] { "application/pdf" } },
            { DevicePlatform.UWP, new[] { ".pdf" } },
            { DevicePlatform.Tizen, new[] { "*/*" } },
            { DevicePlatform.macOS, new[] { "pdf" } },
        });

           
            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Pick a PDF"
            });

            
            if (pickResult != null)
            {
                string filename = "filesqa" + DateTimeOffset.Now.ToUnixTimeSeconds();
                isImage = false;
                isPdf = true;

                
                using var fileStream = await pickResult.OpenReadAsync();
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length);

                
                TempAttachment = new Doclinks
                {
                    fileName = filename + ".pdf",
                    title = filename + ".pdf",
                    ownertable = "PLUSGAUDIT",
                    previewdoc = Base64Files.pdfimage, 
                    documentdata = Convert.ToBase64String(bytes),
                    urltype = "FILE",
                    doctype = "Attachments",
                    modified = DateTime.Now.ToLocalTime(),
                    createdate = DateTime.Now.ToLocalTime(),
                };

               
                await Navigation.PushModalAsync(new AddAttachments(this));
            }
            else
            {
                await UserDialogs.Instance.AlertAsync("Oops! Looks like something went wrong with your file.", "Alert!", "Ok");
            }
        }
        catch (Exception g)
        {
            Console.WriteLine("===ERROR: " + g.Message + "===");
            await UserDialogs.Instance.AlertAsync("An error occurred. Please try again.", "Error", "Ok");
        }
    }

    private async Task SaveAttachment()
        {
            
            //global.isRefreshWorkDetail = true;
            //Minus Add to sqlite after save sqa
            TempAttachment.document = TempAttachment.description;
            SelectedSQA.doclinks.Add(TempAttachment);
            AttachmentListData.Add(TempAttachment);
            AttachmentsCount = AttachmentListData.Count.ToString();
            await UserDialogs.Instance.AlertAsync("Your Attachment has been store in this SQA", "Success!", "Ok");
            await Navigation.PopModalAsync();
            Debug.WriteLine("I am called");
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
                SelectedSQA.doclinks.Remove(TempAttachment);
            }

            AttachmentListData.Remove(TempAttachment);
            AttachmentsCount = AttachmentListData.Count.ToString();
            await UserDialogs.Instance.AlertAsync("Your Attachment has been delete in this SQA", "Success!", "Ok");
            await Navigation.PopModalAsync();
        }

        public void backtoHome()
        {

            Navigation.PopToRootAsync();
            //Navigation.PopAsync();

            SfTabItem tabItem = sftabview.Items[0];
            sftabview.SelectedIndex = 0;
            //wolistPage.woListViewModel.issync = true;
            //tabItem.Content = new WorkorderSync(wolistPage.woListViewModel).Content;
            //homePage.tabview.SelectedIndex = 1;
        }
        
        public async Task downloadAttachment()
        {

            bool answer = await UserDialogs.Instance.ConfirmAsync("Want to save it to your gallery?", "Attention", "Yes", "No");
            if (answer)
            {
                if (Base64Files.GetFileExtension(TempAttachment.documentdata) == "pdf")
                {
                    byte[] _fromBase64String = Convert.FromBase64String(TempAttachment.documentdata);
                    string filename = "sqa-xam-attachment-pdf" + TempAttachment.document;
                    string path = savefile.SaveBinaryPdf(filename, _fromBase64String);
                }
                else
                {
                    byte[] _fromBase64String = Convert.FromBase64String(TempAttachment.documentdata);
                    string filename = "sqa-xam-attachment-image" + TempAttachment.document;
                    string path = savefile.SaveBinaryImg(filename, _fromBase64String);
                    //byte[] _fromBase64String = Convert.FromBase64String(selectedDoclink.documentdata);
                    //string filename = "xam-attachment-image" + workorder.wonum;
                    //byte[] b = File.ReadAllBytes(file.Path);
                    //String Base64File = Convert.ToBase64String(b);
                    //var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    //var filePath = Path.Combine(folderPath, filename + ".jpg");
                    //File.WriteAllBytes(filePath, b);
                }

                await UserDialogs.Instance.AlertAsync("Attachment successfully saved", "Success!", "Ok");
            }
        }

        async void SaveSQA()
        {
           // global.isRefreshWorkDetail = true;

            UserDialogs.Instance.ShowLoading("Uploading SQA...");

            await Task.Delay(1000);
            string username = await SecureStorage.GetAsync("username");

            SelectedSQA.createdby = username;
            SelectedSQA.template = false;
            SelectedSQA.description = $"SQA for Workorder {workorder.wonum}";
            SelectedSQA.tnbwonum = workorder.wonum;

            Debug.WriteLine("Current ID of : " + SelectedSQA.id);
            Debug.WriteLine("value : is : " + SelectedSQA.plusgauditid);
            //DO RESTORE DOCLINKS
            if (!SelectedSQA.plusgauditid.HasValue)
            {
                Debug.WriteLine("Selected.Plusgauditid Not null");
                //Check Ping
                bool ping = await maxrest.Ping();
                if (ping)
                {
                    //DO ADD ACTION
                    SelectedSQA._action = "Add";
                    Plusgaudit responseAdd = await maxrest.AddSQA(SelectedSQA);
                    //Debug.WriteLine("Send data to : " + responseAdd.auditnum);
                    UserDialogs.Instance.HideHud();
                    if (responseAdd.Error != null)
                    {
                        await UserDialogs.Instance.AlertAsync($"{responseAdd.Error.message}", "Error", "Ok");
                        if (responseAdd.Error.reasonCode == "BMXAA9549E")
                        {
                            var hostname = await SecureStorage.GetAsync("Hostname");
                            SecureStorage.RemoveAll();
                            await SecureStorage.SetAsync("Hostname", hostname);
                            await Navigation.PushModalAsync(new LoginPage());
                            return;
                        }
                    }
                    else if (responseAdd.auditnum == null)
                    {
                        //IF CANNOT CONNECT TO INTERNET OR SERVER
                        SelectedSQA._action = "Add";
                        if (!global.isNewSQA)
                        {
                            //IF ADD ALREADY SAVE IN SQLITE
                            SelectedSQA.pendingupload = true;
                            await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                            foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                            {
                                qsqa.alertcolor = "Transparent";
                                await sqlplusgaud.UpdatePlusgaudline(qsqa);
                            }
                            //Add Doclink not yet save to sqlite
                            var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                            foreach (Doclinks doc in doclinks)
                            {
                                doc.refid = SelectedSQA.id;
                                await sqldocs.AddDoclinks(doc);
                            }
                        }
                        else
                        {
                            //IF ADD NOT YET SAVE TO SQLITE
                            SelectedSQA.pendingupload = true;
                            int id = await sqlsqa.AddPlusgaudit(SelectedSQA);
                            foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                            {
                                qsqa.alertcolor = "Transparent";
                                qsqa.sqaid = id;
                                await sqlplusgaud.AddPlusgaudline(qsqa);
                            }
                            //Add Doclink not yet save to sqlite
                            var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                            foreach (Doclinks doc in doclinks)
                            {
                                doc.refid = id;
                                await sqldocs.AddDoclinks(doc);
                            }
                            //Navigation.RemovePage(Navigation.NavigationStack[2]);
                            await Navigation.PopAsync();
                        }

                        await UserDialogs.Instance.AlertAsync("Cannot Reach Server, Data Already Saved Locally, you can try again by click save button", "Error", "Ok");
                        await LoadSQA();
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        //IF UPLOAD SQA SUCCESS
                        SelectedSQA.plusgauditid = responseAdd.plusgauditid;

                        DateTime statsdate = Convert.ToDateTime(responseAdd.statusdate);
                        statsdate = statsdate.ToLocalTime();
                        responseAdd.statusdate = statsdate;

                        SelectedSQA.pendingupload = false;
                        Debug.WriteLine("Audit Number is : " + SelectedSQA.auditnum);
                        if (!global.isNewSQA && SelectedSQA.auditnum == null)
                        {

                            Debug.WriteLine("Null Number : Record is updated");
                            //IF ALREADY SAVE TO SQLITE
                            SelectedSQA.auditnum = responseAdd.auditnum;
                            await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                            foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                            {
                                qsqa.alertcolor = "Transparent";
                                await sqlplusgaud.UpdatePlusgaudline(qsqa);
                            }
                            //Delete Doclink From SQLite
                            List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(SelectedSQA.id, "PLUSGAUDIT");
                            foreach (Doclinks doc in doclinks)
                            {
                                await sqldocs.DeleteDoclinks(doc);
                            }
                            //Add Doclink from response save to sqlite
                            if (responseAdd.doclinks != null)
                            {
                                foreach (Doclinks doc in responseAdd.doclinks)
                                {
                                    doc.refid = SelectedSQA.id;
                                    await sqldocs.AddDoclinks(doc);
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Null Number : Record is Created");
                            //IF NOT YET SAVE TO SQLITE
                            int id = await sqlsqa.AddPlusgaudit(responseAdd);
                            foreach (Plusgaudline qsqa in responseAdd.plusgaudline)
                            {
                                qsqa.alertcolor = "Transparent";
                                qsqa.sqaid = id;
                                await sqlplusgaud.AddPlusgaudline(qsqa);
                            }
                            //Add Doclink not yet save to sqlite
                            if (responseAdd.doclinks != null)
                            {
                                foreach (Doclinks doc in responseAdd.doclinks)
                                {
                                    Console.WriteLine("Add Doclink", SelectedSQA.id);
                                    doc.refid = SelectedSQA.id;
                                    await sqldocs.AddDoclinks(doc);
                                }

                                //remove the doclinks 

                                


                                //Add Doclink from response save to sqlite
                                /*foreach (Doclinks doc in responseAdd.doclinks)
                                {
                                    doc.refid = id;
                                    await sqldocs.AddDoclinks(doc);
                                }*/
                            }
                            // Navigation.RemovePage(Navigation.NavigationStack[2]);
                            await Navigation.PopAsync();

                        }

                        //Upload Pending Signature
                        foreach (Tnbsignature sig in SelectedSQA.tnbsignature.Where(x => x.pendingupdate))
                        {
                            SelectedSignature = sig;
                            await SaveDigitalSignature(false);
                        }
                        SelectedSignature = null;
                        await UserDialogs.Instance.AlertAsync("Add SQA Success", "Success", "Ok");
                        await LoadSQA();
                        await LoadSignatureList();
                        await Navigation.PopAsync();
                    }
                }
                else
                {

                    UserDialogs.Instance.HideHud();
                    //IF CANNOT CONNECT TO INTERNET OR SERVER
                    Debug.WriteLine("value of SQA is : " + SelectedSQA.status);
                    if (!global.isNewSQA)
                    {

                        //IF ADD ALREADY SAVE IN SQLITE
                        SelectedSQA.pendingupload = true;
                        await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                        foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                        {
                            Debug.WriteLine("New SQA and line : " + qsqa.tnbsqaremarks);
                            qsqa.alertcolor = "Transparent";
                            await sqlplusgaud.UpdatePlusgaudline(qsqa);
                        }
                        //Add Doclink not yet save to sqlite
                        var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                        foreach (Doclinks doc in doclinks)
                        {

                            doc.refid = SelectedSQA.id;
                            await sqldocs.AddDoclinks(doc);
                        }
                    }
                    else
                    {
                        //IF ADD NOT YET SAVE TO SQLITE
                        SelectedSQA.pendingupload = true;
                        int id = await sqlsqa.AddPlusgaudit(SelectedSQA);
                        foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                        {
                            Debug.WriteLine("update SQALine : " + qsqa.tnbsqaremarks);
                            qsqa.alertcolor = "Transparent";
                            qsqa.sqaid = id;
                            qsqa.id = null;
                            await sqlplusgaud.AddPlusgaudline(qsqa);
                        }
                        //Add Doclink not yet save to sqlite
                        var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                        foreach (Doclinks doc in doclinks)
                        {
                            doc.refid = id;
                            await sqldocs.AddDoclinks(doc);
                        }

                        //Navigation.RemovePage(Navigation.NavigationStack[2]);
                        await Navigation.PopAsync();
                    }

                    global.isNewSQA = false;

                    await UserDialogs.Instance.AlertAsync("Cannot Reach Server, Data Already Saved Locally, you can try again by click save button", "Error", "Ok");
                    await LoadSQA();
                    await Navigation.PopAsync();
                }


            }
            else
            {
                Debug.WriteLine("Selected.Plusgauditid id is null");
                //REMOVE DOCLINK IF NO ACTION
                List<Doclinks> DelDoc = new List<Doclinks>(SelectedSQA.doclinks);
                var remdoc = SelectedSQA.doclinks.Where(x => x.docinfoid != null && x.isdelete == false);
                foreach (Doclinks doc in remdoc)
                {
                    DelDoc.Remove(doc);
                }
                SelectedSQA.doclinks = DelDoc;

                //Check Ping
                bool ping = await maxrest.Ping();
                if (ping)
                {
                    //DO UPDATE ACTION
                    Plusgaudit responseUpdate = await maxrest.UpdateSQA(SelectedSQA);
                    UserDialogs.Instance.HideHud();

                    if (responseUpdate.Error != null)
                    {
                        await UserDialogs.Instance.AlertAsync($"{responseUpdate.Error.message}", "Error", "Ok");
                        if (responseUpdate.Error.reasonCode == "BMXAA9549E")
                        {
                            var hostname = await SecureStorage.GetAsync("Hostname");
                            SecureStorage.RemoveAll();
                            _ = SecureStorage.SetAsync("Hostname", hostname);
                            await Navigation.PushModalAsync(new LoginPage());
                            return;
                        }
                    }
                    else if (responseUpdate == null)
                    {
                        //IF CANNOT REACH INTERNET OR SERVER
                        SelectedSQA.pendingupload = true;
                        await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                        foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                        {
                            qsqa.alertcolor = "Transparent";
                            await sqlplusgaud.UpdatePlusgaudline(qsqa);
                        }
                        //Add Doclink not yet save to sqlite
                        var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                        foreach (Doclinks doc in doclinks)
                        {
                            doc.refid = SelectedSQA.id;
                            await sqldocs.AddDoclinks(doc);
                        }
                        //Update Doclink need to be delete
                        foreach (Doclinks doc in DelDoc.Where(x => x.isdelete))
                        {
                            await sqldocs.UpdateDoclinks(doc);
                        }
                        await UserDialogs.Instance.AlertAsync("Cannot Reach Server, Data Already Saved Locally, you can try again by click save button", "Error", "Ok");
                        await LoadSQA();
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        //IF SUCCESSFULY CONNECT TO INTERNET CONNECTION
                        SelectedSQA.pendingupload = false;
                        await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                        foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                        {
                            qsqa.alertcolor = "Transparent";
                            await sqlplusgaud.UpdatePlusgaudline(qsqa);
                        }
                        //Delete Doclink From SQLite
                        List<Doclinks> doclinks = await sqldocs.GetDoclinksFilter(SelectedSQA.id, "PLUSGAUDIT");
                        foreach (Doclinks doc in doclinks)
                        {
                            await sqldocs.DeleteDoclinks(doc);
                        }
                        //Add Doclink from response save to sqlite
                        if (responseUpdate.doclinks != null)
                        {
                            foreach (Doclinks doc in responseUpdate.doclinks)
                            {
                                Console.WriteLine("Ini Doclink" + doc.document);
                                doc.refid = SelectedSQA.id;
                                await sqldocs.AddDoclinks(doc);
                            }
                        }
                        await UserDialogs.Instance.AlertAsync("Update SQA Success", "Success", "Ok");
                        await LoadSQA();
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    UserDialogs.Instance.HideHud();
                    //IF CANNOT REACH INTERNET OR SERVER
                    Debug.WriteLine("I am offline SQA Creater ELSE");
                    SelectedSQA.pendingupload = true;
                    await sqlsqa.UpdatePlusgaudit(SelectedSQA);
                    foreach (Plusgaudline qsqa in SelectedSQA.plusgaudline)
                    {
                        qsqa.alertcolor = "Transparent";
                        await sqlplusgaud.UpdatePlusgaudline(qsqa);
                    }
                    //Add Doclink not yet save to sqlite
                    var doclinks = SelectedSQA.doclinks.Where(x => x.id == null || x.id == 0);
                    foreach (Doclinks doc in doclinks)
                    {
                        doc.refid = SelectedSQA.id;
                        await sqldocs.AddDoclinks(doc);
                    }
                    //Update Doclink need to be delete
                    foreach (Doclinks doc in DelDoc.Where(x => x.isdelete))
                    {
                        await sqldocs.UpdateDoclinks(doc);
                    }
                    await UserDialogs.Instance.AlertAsync("Cannot Reach Server, Data Already Saved Locally, you can try again by click save button", "Error", "Ok");
                    await LoadSQA();
                    await Navigation.PopAsync();
                }


            }
            await progressBarUpdate.LoadProgress(workorder);
        }



        public async Task SaveDigitalSignature(bool islist=true)
        {
            UserDialogs.Instance.ShowLoading("Uploading Signature...");
            await Task.Delay(1000);
            
            SelectedSignature.tnbexecdate = DateTime.Now;
            SelectedSignature.updateby = await SecureStorage.GetAsync("username");
            SelectedSignature.refid = SelectedSQA.id;
            bool check = await maxrest.Ping();
            if (check && SelectedSQA.plusgauditid>0)
            {
                Tnbsignature res = new Tnbsignature();
                if (SelectedSignature.tnbsignatureid != 0 && SelectedSignature.tnbsignatureid != null)
                {
                    res = await maxrest.UpdateSignatures(SelectedSignature);
                }
                else
                {
                    res = await maxrest.AddSignatures(SelectedSignature);
                }
                UserDialogs.Instance.HideHud();

                if (res == null)
                {
                    SelectedSignature.pendingupdate = true;
                    SelectedSQA.pendingupload = true;
                    SQLitePlusgaudit plusgaudit = new SQLitePlusgaudit();
                   await plusgaudit.UpdatePlusgaudit(SelectedSQA);


                    if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                    {
                        
                        await sqlsignature.UpdateTnbsignature(SelectedSignature);
                    }
                    else
                    {
                        await sqlsignature.AddTnbsignature(SelectedSignature);
                    }
                    await UserDialogs.Instance.AlertAsync("Signature Save Locally in this device, please upload again if you have better connection", "Success", "Ok");
                    if (islist)
                    {
                        await LoadSignatureList();
                        await Navigation.PopAsync();
                    }
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
                    SelectedSignature.pendingupdate = false;
                    if (res._imagelibref == null && SelectedSignature.signature != null)
                    {
                        SelectedSignature.tnbsignatureid = res.tnbsignatureid;
                        Tnbsignature sglib = await maxrest.AddImglibSignatures(SelectedSignature);
                        if (sglib == null || sglib.Error != null)
                        {
                            await UserDialogs.Instance.AlertAsync("Signature Cannot Save to IMAGELIB, Please try again!", "Error!", "Ok");
                            SelectedSignature.pendingupdate = true;
                            SelectedSQA.pendingupload = true;
                            SQLitePlusgaudit plusgaudit = new SQLitePlusgaudit();
                            await plusgaudit.UpdatePlusgaudit(SelectedSQA);
                        }
                        else
                        {
                            SelectedSignature._imagelibref = sglib._imagelibref;
                        }
                    }
                    if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                    {
                        await sqlsignature.UpdateTnbsignature(SelectedSignature);
                    }
                    else
                    {
                        SelectedSignature.tnbsignatureid = res.tnbsignatureid;
                        await sqlsignature.AddTnbsignature(SelectedSignature);
                    }
                    await UserDialogs.Instance.AlertAsync("Signature Save to Maximo", "Success", "Ok");
                    if (islist)
                    {
                        await LoadSignatureList();
                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                UserDialogs.Instance.HideHud();
                SelectedSignature.pendingupdate = true;
                SelectedSQA.pendingupload = true;
                SQLitePlusgaudit plusgaudit = new SQLitePlusgaudit();
                await plusgaudit.UpdatePlusgaudit(SelectedSQA);

                if (SelectedSignature.id != 0 && SelectedSignature.id != null)
                {
                    await sqlsignature.UpdateTnbsignature(SelectedSignature);
                }
                else
                {
                    SelectedSignature.tnbsigdate = DateTime.Now.ToLocalTime();
                    await sqlsignature.AddTnbsignature(SelectedSignature);
                }
                await UserDialogs.Instance.AlertAsync("Signature Save Locally in this device, please upload again if you have better connection", "Success", "Ok");
                if (islist)
                {
                    await LoadSignatureList();
                    await Navigation.PopAsync();
                }
            }

        }

        public async void DeleteSignature(object obj)
        {
            UserDialogs.Instance.ShowLoading("Delete Signature...");
            await Task.Delay(1000);

            SelectedSignature = obj as Tnbsignature;
            if(SelectedSignature.tnbsignatureid == 0 || SelectedSignature.tnbsignatureid == null)
            {
                await sqlsignature.DeleteTnbsignature(SelectedSignature);
                UserDialogs.Instance.HideHud();
                await LoadSignatureList();
                await UserDialogs.Instance.AlertAsync("Signature Deleted", "Success", "Ok");
                return;
            }
            bool check = await maxrest.Ping();
            if (check)
            {
                SelectedSignature._action = "Delete";
                Tnbsignature res = await maxrest.UpdateSignatures(SelectedSignature);
                UserDialogs.Instance.HideHud();

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
                    await sqlsignature.DeleteTnbsignature(SelectedSignature);
                    await LoadSignatureList();
                    await UserDialogs.Instance.AlertAsync("Signature Delete in Maximo", "Success", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.HideHud();
                await UserDialogs.Instance.AlertAsync("Please check your internet connection and try again!", "Failed!", "Ok");
            }
        }

    }
}
