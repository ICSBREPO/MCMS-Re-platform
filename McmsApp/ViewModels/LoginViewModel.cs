using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Controls.UserDialogs.Maui;
using McmsApp.ApiServices;
using McmsApp.AppLayout;
using McmsApp.Models;
using McmsApp.Persistence;
using McmsApp.Views.Tabbed;
using Plugin.Multilingual;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace McmsApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        //API
        IMaximoApi maxrest { get; set; }

        //SQLITE
        IUserProfile sqlprofile = new SQLiteUserprofile();

        //attribute
        public string UsernameEntry { get; set; }
        public string PasswordEntry { get; set; }
        public string HostnameEntry { get; set; }

        //message handler
        public bool isErrorUsername { get; set; }
        public bool isErrorPassword { get; set; }
        public bool isErrorHostname { get; set; }
        public string ErrorUsernameMessage { get; set; }
        public string ErrorPasswordMessage { get; set; }
        public string ErrorHostnameMessage { get; set; }

        //command
        public ICommand LoginActionCommand { get; set; }

        //Languages
        public ObservableCollection<string> Languages { get; set; }

        //open picker
        public ICommand PickerActionCommand { get; set; }
        public bool pickerIsOpen { get; set; }
        public ICommand pickerSetValueCommand { get; set; }
        public string pickerSetLanguage { set; get; }
        public string SelectedLanguage { get; set; }

        public LoginViewModel()
        {
            _ = loadhostname();
            UsernameEntry = PasswordEntry= "";
            LoginActionCommand = new Command(async () => await loginAction());
            PickerActionCommand = new Command( OpenPicker );
            SelectedLanguage = "English";
            //set value list languages
            Languages = new ObservableCollection<string>();
            Languages.Add("English");
            Languages.Add("Malay");
        }

        async Task loadhostname()
        {
            HostnameEntry = await SecureStorage.GetAsync("Hostname");
            System.Diagnostics.Debug.WriteLine("ini hostname" + HostnameEntry);
        }

        public void OpenPicker()
        {
            pickerIsOpen = !pickerIsOpen;
        }

        private bool IsValidUrl(string url)
        {
            return (Uri.IsWellFormedUriString(url, UriKind.Absolute));
        }

        public async Task loginAction()
        {
            maxrest = new MaximoApi();
            UserDialogs.Instance.ShowLoading("Loading...");
            try
            {
                isErrorHostname = isErrorUsername = isErrorPassword = false;

                if (UsernameEntry.Length == 0)
                {
                    ErrorUsernameMessage = "Username cannot be empty!";
                    isErrorUsername = true;
                }
                if (PasswordEntry.Length == 0)
                {
                    ErrorPasswordMessage = "Password cannot be empty!";
                    isErrorPassword = true;
                }

                if (HostnameEntry.Length == 0)
                {
                    ErrorHostnameMessage = "Hostname cannot be empty!";
                    isErrorHostname = true;
                }


                if (!IsValidUrl(HostnameEntry))
                {
                    ErrorHostnameMessage = "Hostname not correct!";
                    isErrorHostname = true;
                }

                if (isErrorUsername == false && isErrorPassword == false && isErrorHostname == false)
                {
                    await SecureStorage.SetAsync("Hostname", HostnameEntry);
                    System.Diagnostics.Debug.WriteLine("Hostname");
                    bool ping = await maxrest.Ping();
                    if (ping)
                    {
                        Apikey ResApikey = new Apikey();
                        ResApikey = await maxrest.Login(UsernameEntry, PasswordEntry);
                        if (ResApikey.apikey != null)
                        {
                            await SecureStorage.SetAsync("apikey", ResApikey.apikey);
                            await SecureStorage.SetAsync("username", UsernameEntry);
                            await SecureStorage.SetAsync("password", PasswordEntry);
                            await SecureStorage.SetAsync("chooseLanguage", SelectedLanguage);

                            if (SelectedLanguage == "English")
                            {
                                CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("English"));
                                AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                            }
                            else
                            {
                                CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("Malay"));
                                AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                            }
                            
                            UserProfile userProfileData = await maxrest.whoAmI();
                            await SecureStorage.SetAsync("displayname", userProfileData.displayname);

                            if (userProfileData.email == "" || userProfileData.email == null)
                            {
                                await SecureStorage.SetAsync("email", "-");
                            }
                            else
                            {
                                await SecureStorage.SetAsync("email", userProfileData.email);
                            }

                            if (userProfileData != null)
                            {
                                if (userProfileData._imagelibref != null)
                                {
                                    string id = userProfileData._imagelibref.Split('/').Last();
                                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                                    var filePath = Path.Combine(folderPath, $"profile{id}.png");
                                    try
                                    {
                                        File.WriteAllBytes(filePath, await maxrest.DownloadImage(id));
                                    }
                                    catch(Exception e)
                                    {

                                    }
                                    userProfileData.picture = filePath;
                                }
                                userProfileData.password = PasswordEntry;
                                UserProfile user = await sqlprofile.GetUserProfile(userProfileData.personuid);
                                await SecureStorage.SetAsync("personid", userProfileData.personid);

                                if (user != null)
                                {
                                    userProfileData.id = user.id;
                                    if (user.themeChoose == null || user.themeChoose == 0)
                                    {
                                        userProfileData.themeChoose = 1;
                                    }
                                    else
                                    {
                                        userProfileData.themeChoose = user.themeChoose;
                                    }

                                    AppSettings.Instance.SelectedThemeColor = user.themeChoose;
                                    await sqlprofile.UpdateUserProfile(userProfileData);
                                }
                                else
                                {
                                    AppSettings.Instance.SelectedThemeColor = 1;
                                    userProfileData.themeChoose = 1;
                                    await sqlprofile.AddUserProfile(userProfileData);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Need to discuss");
                            }
                            await UserDialogs.Instance.AlertAsync($"Login Successfully", "Success!!", "OK");
                            Application.Current.MainPage = new NavigationPage(new MainPage());
                        }
                        else if (ResApikey.Error != null)
                        {
                            //await Navigation.PushModalAsync(new MainPage());
                            await UserDialogs.Instance.AlertAsync($"{ResApikey.Error.message}", "Error!!", "OK");
                        }
                        else
                        {
                            //await Navigation.PushModalAsync(new MainPage());
                            await UserDialogs.Instance.AlertAsync($"Time Out Error", "Error!!", "OK");
                        }
                    }
                    else
                    {
                        UserProfile user = await sqlprofile.LoginOffline(UsernameEntry, PasswordEntry);
                        if (user != null)
                        {
                            AppSettings.Instance.SelectedThemeColor = user.themeChoose;
                            await SecureStorage.SetAsync("apikey", "autogetapikey");
                            await SecureStorage.SetAsync("username", UsernameEntry);
                            await SecureStorage.SetAsync("password", PasswordEntry);
                            await SecureStorage.SetAsync("displayname", user.displayname);
                            await SecureStorage.SetAsync("chooseLanguage", SelectedLanguage);

                            if (SelectedLanguage == "English")
                            {
                                CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("English"));
                                AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                            }
                            else
                            {
                                CrossMultilingual.Current.CurrentCultureInfo = CrossMultilingual.Current.NeutralCultureInfoList.ToList().First(element => element.EnglishName.Contains("Malay"));
                                AppResources.Culture = CrossMultilingual.Current.CurrentCultureInfo;
                            }

                            await UserDialogs.Instance.AlertAsync($"Login Successfully \n Your Login Offline", "Success!!", "OK");
                            Application.Current.MainPage = new NavigationPage(new MainPage());
                        }
                        else
                        {
                            await UserDialogs.Instance.AlertAsync($"Login Failed Please Check Your Internet Connection!", "Error!!", "OK");
                        }
                        UserDialogs.Instance.HideLoading();
                    }
                    
                }

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}
