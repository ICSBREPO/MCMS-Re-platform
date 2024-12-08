using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using mcms.Models;
using mcms.Persistence;
using Syncfusion.ListView.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.ViewModels
{
    public class LookupViewModel : BaseViewModel
    {
        IUserProfile sqluser = new SQLiteUserprofile();
        ICompanies sqlcomp = new SQLiteCompanies(); 
        IMxDomain sqldom = new SQLiteMxDomain();
        public List<MxDomain> ListData { get; set; }
        
        INavigation Navigation { get; set; }

        public ICommand SelectedDataCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public object viewModelParent { get; set; }

        public string selectedPage { get; set; }

        public int datacount { get; set; }

        public string Type { get; set; }

        public LookupViewModel(object viewModels,string page,INavigation navigation,string type="person")
        {
            viewModelParent = viewModels;
            selectedPage = page;
            Navigation = navigation;

            Type = type;

            loadData();

            SelectedDataCommand = new Command(SelectedData);
            BackCommand = new Command(Back);
        }

        async void loadData()
        {
            ListData = new List<MxDomain>();
            if(Type == "person")
            {
                List<UserProfile> persons = await sqluser.GetUserProfileAsync();
                foreach(UserProfile person in persons)
                {
                    ListData.Add(new MxDomain
                    {
                        value = person.personid,
                        description = person.displayname
                    });
                    
                }
            }
            else if(Type =="companies")
            {
                List<Companies> companies = await sqlcomp.GetCompaniesAsync();
                foreach (Companies comp in companies)
                {
                    ListData.Add(new MxDomain
                    {
                        value = comp.company,
                        description = comp.name
                    });
                }
            }
            else if (Type == "newstatus")
            {
                ListData.Add(new MxDomain
                {
                    value = "ASGN",
                    description = "Assigned"
                });
                ListData.Add(new MxDomain
                {
                    value = "INPRGS",
                    description = "Work In Progress"
                });
                ListData.Add(new MxDomain
                {
                    value = "WATINST",
                    description = "Waiting for Installation"
                });
                ListData.Add(new MxDomain
                {
                    value = "APINST",
                    description = "Installation Completed"
                });
                ListData.Add(new MxDomain
                {
                    value = "WTEST",
                    description = "Work In Progress for Testing"
                });
                ListData.Add(new MxDomain
                {
                    value = "TESTCOMP",
                    description = "Testing Completed"
                });
                ListData.Add(new MxDomain
                {
                    value = "WMPIAT",
                    description = "Waiting for Mobile PIAT"
                });
                ListData.Add(new MxDomain
                {
                    value = "PIATFAIL",
                    description = "PIAT Failed"
                });
                ListData.Add(new MxDomain
                {
                    value = "TECO",
                    description = "Technically Completed"
                });
                ListData.Add(new MxDomain
                {
                    value = "MPIATCOMP",
                    description = "Mobile PIAT Completed"
                });
                ListData.Add(new MxDomain
                {
                    value = "MSITERDCOMP",
                    description = "Mobile Site Ready Complete"
                });
                ListData.Add(new MxDomain
                {
                    value = "COMP",
                    description = "Completed"
                });
            }
            else if (Type == "brand")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBBRAND");
            }
            else if (Type == "modelnum")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBMODELNUM");
            }
            else if (Type == "country")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBMANUFCOUNTRY");
            }
            else if (Type == "voltagelevel")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBVOLTAGELEVEL");
            }
            else if (Type == "assetcondition")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBASSETCONDITION");
            }
            else if (Type == "tnblabel")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBCHECKLISTLABEL");
            }
            else if (Type == "tnbpermittype")
            {
                ListData = await sqldom.GetMxDomainAsync("TNBPERMITTYPE");
            }

            datacount = ListData.Count;
        }

        async void SelectedData(object obj)
        {
            SfListView selected = obj as SfListView;
            MxDomain selecteddata = selected.SelectedItem as MxDomain;
            if (Type == "person")
            {
                if (selectedPage == "checklist")
                {
                    TestFormViewModel testFormViewModel = viewModelParent as TestFormViewModel;
                    var propertyinfo = testFormViewModel.wochecklisthdr.GetType().GetProperty(testFormViewModel.selectedIndicator);
                    propertyinfo.SetValue(testFormViewModel.wochecklisthdr, selecteddata.value);
                }
                else if (selectedPage == "sqa")
                {
                    SqaViewModel sqaViewModel = viewModelParent as SqaViewModel;
                    var propertyinfo = sqaViewModel.SelectedSQA.GetType().GetProperty(sqaViewModel.selectedIndicator);
                    propertyinfo.SetValue(sqaViewModel.SelectedSQA, selecteddata.value);
                }
                else if (selectedPage == "permit")
                {
                    PermitViewModel permitViewModel = viewModelParent as PermitViewModel;
                    var propertyinfo = permitViewModel.permit.GetType().GetProperty(permitViewModel.selectedIndicator);
                    propertyinfo.SetValue(permitViewModel.permit, selecteddata.value);
                }
            }
            else if (Type == "tnblabel")
            {
                if (selectedPage == "sqa")
                {
                    SqaViewModel sqaViewModel = viewModelParent as SqaViewModel;
                    sqaViewModel.SelectedSignature.tnblabel = selecteddata.value;
                    /*Debug.WriteLine("Value : " + selecteddata.valueid);
                    Debug.WriteLine("Value of user id is : " + selecteddata.valueid);
                    sqaViewModel.SelectedSignature.tnbstaffid = "Some Text";*/
                    string userId = await SecureStorage.GetAsync("username");
                    if (userId != null)
                    {
                        Debug.WriteLine("Selected value is : " + selecteddata.value);
                        if (selecteddata.value.Equals("Disediakan oleh"))
                            sqaViewModel.SelectedSignature.tnbstaffid = userId;
                        else
                            sqaViewModel.SelectedSignature.tnbstaffid = "";

                    }

                }
                else
                {

                    TestFormViewModel testFormViewModel = viewModelParent as TestFormViewModel;
                    testFormViewModel.SelectedSignature.tnblabel = selecteddata.value;
                    string userId = await SecureStorage.GetAsync("username");
                    if(userId != null)
                    {
                        Debug.WriteLine("Selected value is : " + selecteddata.value);
                        if(selecteddata.value.Equals("Disediakan oleh")) 
                            testFormViewModel.SelectedSignature.tnbstaffid = userId;
                        else
                            testFormViewModel.SelectedSignature.tnbstaffid = "";

                    }



                }
            }
            else if (Type == "newstatus")
            {
                WoDetailViewModel woDetailViewModel = viewModelParent as WoDetailViewModel;
                woDetailViewModel.SelectedNewStatus = selecteddata.value;
                woDetailViewModel.SelectedNewStatusDescription = selecteddata.description;
            }
            else if (Type == "brand")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].tnbbrand = selecteddata.value;
            }
            else if (Type == "modelnum")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].tnbmodelnum = selecteddata.value;
            }
            else if (Type == "country")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].tnbmanufacturingcountry = selecteddata.value;
            }
            else if (Type == "companies")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].manufacturer = selecteddata.value;
            }
            else if (Type == "voltagelevel")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].tnbvoltagelevel = selecteddata.value;
            }
            else if (Type == "assetcondition")
            {
                AssetViewModel assetviewmodel = viewModelParent as AssetViewModel;
                assetviewmodel.workorder.asset[0].tnbassetcondition = selecteddata.value;
            }
            else if (Type == "tnbpermittype")
            {
                PermitViewModel permitViewModel = viewModelParent as PermitViewModel;
                permitViewModel.permit.tnbpermittype = selecteddata.value;
            }

            selected.SelectedItem = null;

            await Navigation.PopModalAsync();
        }

        async void Back()
        {
            await Navigation.PopModalAsync();
        }
    }
}
