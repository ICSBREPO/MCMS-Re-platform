using mcms.AppLayout;
using mcms.Models;
using mcms.ViewModels;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mcms.Views.Work.WorkDetail.Testform
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestformDetail : ContentPage
    {
        private double width;
        private double height;

        TestFormViewModel testmodel { get; set; }
        public TestformDetail(TestFormViewModel testformviewmodel)
        {
            testmodel = testformviewmodel;
            InitializeComponent();
            BindingContext = testformviewmodel;

            checklistDetail.ItemSelected += (sender,e) =>
            {
                if (checklistDetail.SelectedItem == null)
                    return;
                var item = (Tnbwochecklist)e.SelectedItem;
                checklistDetail.SelectedItem = null;   
                item.xamvisible = !item.xamvisible;
                Debug.WriteLine("index" + e.SelectedItemIndex);
            };    

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if ((Device.RuntimePlatform == "iOS" || Device.RuntimePlatform == "Android") &&
                AppSettings.Instance.IsSafeAreaEnabled)
            {
                if (width != this.width || height != this.height)
                {
                    var safeAreaHeight = AppSettings.Instance.SafeAreaHeight;
                    this.width = width;
                    this.height = height;

                    if (width < height)
                    {
                        iOSSafeArea.Height = safeAreaHeight;
                    }
                    else   
                    { 
                        iOSSafeArea.Height = 0;   
                    }
                }       
            }
        }

        void SfPicker_OkButtonClicked(System.Object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            Debug.WriteLine("test1 " + e.NewValue);
            Debug.WriteLine("test2 " + e.OldValue);
            if(e.NewValue != null)
            {
                foreach (Tnbwochecklist chk in testmodel.wochecklisttype.tnbwochecklist)
                {
                    if(testmodel.checklist.tnbwochecklistid == chk.tnbwochecklistid)
                    {
                        chk.tnbvalue = e.NewValue.ToString();
                    }
                }
            }
            else
            {

            }
            //Tnbwochecklist tnbwochecklist = (Tnbwochecklist)sender;
            //Tnbwochecklist tnbwochecklist = sender as testmodel.wochecklisttype.tnbwochecklist;
            //tnbwochecklist.tnbvalue = e.NewValue.ToString();
            //testmodel.wochecklisttype.tnbwochecklist.tnbvalue = e.NewValue.ToString();
            //checklistDetail.SelectedItem
        }

        void SfRadioButton_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            testmodel.calculatePercentage(); 
        }

        void SfCheckBox_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            SfCheckBox cek = sender as SfCheckBox;
            //Console.WriteLine(cek.AutomationId + " - " + cek.StyleId);
            //testmodel.SetCheckBox(Convert.ToInt32(cek.AutomationId), cek.StyleId);
            testmodel.calculatePercentage();  
        }

        void SfSwitch_StateChanged(System.Object sender, Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs e)
        {
            if (e.NewValue.Value)
            {
                foreach (Tnbwochecklist chk in testmodel.wochecklisttype.tnbwochecklist)
                {
                    chk.xamvisible = true;
                } 
            }
            else
            {
                foreach (Tnbwochecklist chk in testmodel.wochecklisttype.tnbwochecklist)
                {
                    chk.xamvisible = false;  
                }
            }
            
        }

        
    }
}