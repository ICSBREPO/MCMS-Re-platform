using System.Collections.ObjectModel;
using System.Windows.Input;
using McmsApp.Models;
using Controls.UserDialogs.Maui;
using McmsApp.Views.Login;
using McmsApp.Persistence;
using McmsApp.ApiServices;
using Microsoft.Maui.Media;
using McmsApp.Views.Setting;
using McmsApp.AppLayout;
using McmsApp.Views.Tabbed;
using McmsApp.Views.Setting.ChangeProfile;
using System.Globalization;
using System.Diagnostics;

namespace McmsApp.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        // Command
        public ICommand LogoutCommand { get; set; }
        public ICommand EditProfileCommand { get; set; }
        public ICommand BackToSettingCommand { get; set; }
        public ICommand PickerActionCommand { get; set; }
        public ICommand TakePictureFromCameraCommand { get; set; }
        public ICommand TakePictureFromGaleryCommand { get; set; }
        public ICommand DeletePictureCommand { get; set; }
        public ICommand GoToFaqs { get; set; }
        public ICommand GoToTermOfService { get; set; }
        public ICommand GoToContentHelp { get; set; }
        public ICommand OpenEmail { get; set; }
        public ICommand OpenCallDial { get; set; }
        public ICommand OpenPrivacyPolicy { get; set; }

        // To Store Username session to this varable
        public string Username { get; set; }
        INavigation Navigation;

        //API
        IMaximoApi maxrest = new MaximoApi();

        // User Binding
        public UserProfile UserBinding { get; set; }

        // Photo Picker
        public ObservableCollection<string> PhotoPickers { get; set; }
        public bool pickerIsOpen { get; set; }

        // Image
        private MediaFile _mediaFile;
        public bool imageSource { get; set; }

        // Theme
        public ICommand goToTheme { get; set; }
        ITheme sqltheme = new SQLiteTheme();
        public ObservableCollection<Theme> ThemeSources { get; set; }
        public Command<Theme> ChooseTheme { get; set; }

        // User
        IUserProfile sqlprofile = new SQLiteUserprofile();

        // Change Langugage
        public ICommand GoToChangeLanguage { get; set; }
        public Command<String> LanguagePicker { get; }
        public List<Language> ListLanguages { get; set; }
        public string chooseLanguage;
        public ICommand SaveLanguage { get; set; }

        // Faqs Binding
        ObservableCollection<FaqsModel> faqslistdata = new ObservableCollection<FaqsModel>();
        public ObservableCollection<FaqsModel> FaqsListData { get { return faqslistdata; } }

        // Term And Condition Binding
        public string termAndConditionDesc { get; set; }
        public string privacyPolicyDesc { get; set; }

        // Content Help Binding
        public struct helpStruct
        {
            public int id { get; set; }
            public string title { get; set; }
            public string descrption { get; set; }
        }

        public List<helpStruct> HelpListSource { get; set; }

        [Obsolete]
        public SettingViewModel(INavigation navigation)
        {
            Navigation = navigation;
            PhotoPickers = new ObservableCollection<string>();
            PhotoPickers.Add("Camera");
            PhotoPickers.Add("Gallery");
            PickerActionCommand = new Command(OpenPicker);
            LogoutCommand = new Command(LogoutAction);
            EditProfileCommand = new Command(EditPhotoAction);
            BackToSettingCommand = new Command(BackToSetting);
            DeletePictureCommand = new Command(deleteProfilePicture);
            TakePictureFromCameraCommand = new Command(async () => await takePictureFromCamera());
            TakePictureFromGaleryCommand = new Command(async () => await takePictureFromDevices());

            goToTheme = new Command(ShowTheme);
            ChooseTheme = new Command<Theme>(ChoosedTheme);
            GoToChangeLanguage = new Command(goToChangeLanguage);
            SaveLanguage = new Command(saveLanguage);
            GoToFaqs = new Command(ShowFaqs);
            GoToTermOfService = new Command(goToTermOfService);
            GoToContentHelp = new Command(goToContententHelp);
            OpenEmail = new Command(async () => await openEmail());
            OpenCallDial = new Command(async () => await openDialAsync());
            OpenPrivacyPolicy = new Command(goToPrivacyPolicy);

            termAndConditionDesc =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."+
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            _ = SaveUserLocation();
            _ = LoadUserInfoAsync();
            loadLanguage();
            _ = loadThemeAsync();
            loadFaqsData();
        }

        private async Task openEmail()
        {
            List<string> recipients = new List<string>();
            recipients.Add("mcms-support@tnb.com.my");
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Dear Customer Care",
                    Body = "Some need help you",
                    To = recipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
                //Console.WriteLine(fbsEx.Message);
            }
            catch (Exception ex)
            {
                // Some other exception occurred
                //Console.WriteLine(ex.Message);
            }

        }

        private async Task openDialAsync()
        {
            try
            {
                PhoneDialer.Open("1300885454");
            }
            catch (ArgumentNullException anEx)
            {
                //Console.WriteLine(anEx.Message);
            }
            catch (FeatureNotSupportedException ex)
            {
                //Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }


        private async Task loadThemeAsync()
        {
            string userName = await SecureStorage.GetAsync("username");
            UserProfile usr = await sqlprofile.GetUserProfileByUserName(userName);

            ThemeSources = new ObservableCollection<Theme>();
            bool themeChooseOne;
            if (usr.themeChoose == 1)
            {
                themeChooseOne = false;
            }
            else
            {
                themeChooseOne = true;
            }

            ThemeSources.Add(new Theme
            {
                id = 1,
                title = "Light Theme",
                description = "Make your eyes cry for happiness!",
                colorOne = "#F54E5E",
                colorTwo = "#2F80ED",
                colorThree = "#FFFFFF",
                colorFour = "#C4C4C4",
                applied = themeChooseOne
            });


            bool themeChooseTwo;
            if (usr.themeChoose == 2)
            {
                themeChooseTwo = false;
            }
            else
            {
                themeChooseTwo = true;
            }

            ThemeSources.Add(new Theme
            {
                id = 2,
                title = "Dark Theme",
                description = "Professional look for your screens.",
                colorOne = "#56CCF2",
                colorTwo = "#7EE5F2",
                colorThree = "#000000",
                colorFour = "#949494",
                applied = themeChooseTwo,
            });

            bool themeChooseThree;
            if (usr.themeChoose == 3)
            {
                themeChooseThree = false;
            }
            else
            {
                themeChooseThree = true;
            }

            ThemeSources.Add(new Theme
            {
                id = 3,
                title = "Blackboard Theme",
                description = "Get a clean view of Grial layouts.",
                colorOne = "#4592D0",
                colorTwo = "#C4C4C4",
                colorThree = "#000000",
                colorFour = "#FFFFFF",
                applied = themeChooseThree,
            });

            bool themeChooseFour;
            if (usr.themeChoose == 4)
            {
                themeChooseFour = false;
            }
            else
            {
                themeChooseFour = true;
            }

            ThemeSources.Add(new Theme
            {
                id = 4,
                title = "Haze Theme",
                description = "Natural look of Grial layout",
                colorOne = "#44C868",
                colorTwo = "#FFDB56",
                colorThree = "#FFFFFF",
                colorFour = "#E1E1E1",
                applied = themeChooseFour
            });

            // Languages
            bool isCheckedEnglish = false;
            bool isCheckedMalaysia = false;
            if (chooseLanguage == "English")
            {
                isCheckedEnglish = true;
                isCheckedMalaysia = false;
            }
            else
            {
                isCheckedEnglish = false;
                isCheckedMalaysia = true;
            }

            ListLanguages = new List<Language>()
            {
                new Language(){Item = "English" , IsChecked = isCheckedEnglish },
                new Language(){Item = "Bahasa Malaysia" , IsChecked = isCheckedMalaysia }
            };

        }

        public void OpenPicker()
        {
            pickerIsOpen = !pickerIsOpen;
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

                string filename = "xam" + DateTime.Now.ToBinary();

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "MCMS",
                    Name = filename,
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 92,
                    SaveToAlbum = true
                });

                if (file == null)
                    return;
                byte[] b = File.ReadAllBytes(file.Path);
                bool ping = await maxrest.Ping();
                if (ping)
                {
                    UserBinding.base64string = Convert.ToBase64String(b);
                    UserProfile res = await maxrest.AddImglibPerson(UserBinding);
                    if (res != null)
                    {
                        UserBinding._imagelibref = res._imagelibref;
                    }
                }
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(folderPath, filename + ".jpg");
                File.WriteAllBytes(filePath, b);
                UserBinding.picture = filePath;
                await sqlprofile.UpdateUserProfile(UserBinding);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
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
                        PhotoSize = PhotoSize.Medium,

                    });
                    if (file == null)
                        return;
                    
                    string filename = "xam" + DateTime.Now.ToBinary();
                    byte[] b = File.ReadAllBytes(file.Path);
                    bool ping = await maxrest.Ping();
                    if (ping)
                    {
                        UserBinding.base64string = Convert.ToBase64String(b);
                        UserProfile res = await maxrest.AddImglibPerson(UserBinding);
                        if (res != null)
                        {
                            UserBinding._imagelibref = res._imagelibref;
                        }
                    }
                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var filePath = Path.Combine(folderPath, filename + ".jpg");
                    File.WriteAllBytes(filePath, b);
                    UserBinding.picture = filePath;
                    await sqlprofile.UpdateUserProfile(UserBinding);
                }
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                await UserDialogs.Instance.AlertAsync("Please Check Permission to Pick From Galery, Close App And Open Again", "Alert!", "Ok");
            }
            
        }

        async void deleteProfilePicture()
        {
            UserBinding.picture = "dpDefault.png";
            await sqlprofile.UpdateUserProfile(UserBinding);
        }

        public async Task LoadUserInfoAsync()
        {
            try
            {
                string username = await SecureStorage.GetAsync("username");
                UserBinding = await sqlprofile.GetUserProfileByUserName(username);
                if (UserBinding.picture == null || UserBinding.picture == "")
                {
                    UserBinding.picture = "dpDefault.png";
                }
                Debug.WriteLine($"ini eror error  : {0}", UserBinding.displayname);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ini eror debug error  : {0}", ex.Message);
            }
        }

        async void LogoutAction()
        {
            bool answer = await UserDialogs.Instance.ConfirmAsync("Are you sure want to log out?", "Alert!", "Yes", "No");
            if (answer)
            {
                string hostname = await SecureStorage.GetAsync("Hostname");
                SecureStorage.RemoveAll();
                AppSettings.Instance.SelectedThemeColor = 1;
                await SecureStorage.SetAsync("Hostname", hostname);
                await Navigation.PushModalAsync(new LoginPage());
            }
        }

        async void EditPhotoAction()
        {
            await Navigation.PushModalAsync(new ChangeProfilePage(this));
        }

        async void ShowTheme()
        {
            await Navigation.PushModalAsync(new ListTheme());
        }

        async void ShowFaqs()
        {
            await Navigation.PushModalAsync(new Faqs(this));
        }

        public async void BackToSetting()
        {
            LoadUserInfoAsync();
            await Navigation.PopModalAsync();
        }

        private async void ChoosedTheme(Theme themeChoose)
        {
            var item = themeChoose as Theme;

            if (!item.applied)
            {
                await UserDialogs.Instance.AlertAsync("Already on this theme");
                return;
            }
            else
            {
                bool answer = await UserDialogs.Instance.ConfirmAsync("Are you sure want to change theme?", "Alert!", "Yes", "No");
                if (answer)
                {
                    AppSettings.Instance.SelectedThemeColor = item.id;
                    string userName = await SecureStorage.GetAsync("username");
                    UserProfile usr = await sqlprofile.GetUserProfileByUserName(userName);
                    usr.themeChoose = item.id;
                    await sqlprofile.UpdateUserProfile(usr);

                    UserProfile usrr = await sqlprofile.GetUserProfileByUserName(userName);
                    Console.WriteLine(usrr.themeChoose);
                    App.Current.MainPage = new NavigationPage(new MainPage());
                }
            }
        }

        async void loadLanguage()
        {
            chooseLanguage = await SecureStorage.GetAsync("chooseLanguage");
        }
        
        async void goToChangeLanguage()
        {
            await Navigation.PushModalAsync(new ChangeLanguage());
        }

        async void saveLanguage()
        {            
            bool restart = await UserDialogs.Instance.ConfirmAsync("Language Switched. Please restart the application. Restart now?", "Alert!", "Yes", "Cancel");

            if (restart)
            {
                var tempLanguage = await SecureStorage.GetAsync("chooseLanguageTemp");
                await SecureStorage.SetAsync("chooseLanguage", tempLanguage);
                if (tempLanguage == "English")
                {
                    CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("English"));
                    AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                }
                else
                {
                    CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("Malay"));
                    AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                }

                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                BackToSetting();
            }
        }

        async void loadFaqsData()
        {
            FaqsListData.Add(new FaqsModel
            {
                id = 1,
                title = "Why do i have verify my phone number upon registration?",
                description = "Phone number verification is a security feature for us to verify that you are the account holder using the app and conducting transactions"
            });

            FaqsListData.Add(new FaqsModel
            {
                id = 2,
                title = "Why do i have verify my phone number upon registration?",
                description = "Phone number verification is a security feature for us to verify that you are the account holder using the app and conducting transactions"
            });

            FaqsListData.Add(new FaqsModel
            {
                id = 3,
                title = "Why do i have verify my phone number upon registration?",
                description = "Phone number verification is a security feature for us to verify that you are the account holder using the app and conducting transactions"
            });
        }

        async void goToTermOfService()
        {
            await Navigation.PushModalAsync(new TermOfService(this));
        }

        async void goToContententHelp()
        {
            await Navigation.PushModalAsync(new ContentHelp(this));
        }

        async void goToPrivacyPolicy()
        {
            await Navigation.PushModalAsync(new PrivacyPolicy(this));
        }

        public async Task SaveUserLocation()
        {
            try
            {
                await Task.Yield();
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Console.WriteLine(location.Latitude);

                    Lbslocation body = new Lbslocation { latitude = location.Latitude, longitude = location.Longitude };
                    List<Lbslocation> getlbs = await maxrest.Updatelbslocation(body);
                    if (getlbs != null)
                    {
                        foreach (var lbs in getlbs)
                        {

                            Console.WriteLine(lbs.href);
                        }
                    }
                    

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"error  : {0}", ex.Message);
            }
        }
    }
}