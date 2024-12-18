using System;
using System.Collections.Generic;
using mcms.AppLayout;
using System.IO;
using mcms.ViewModels;
using Xamarin.Forms;
using System.Threading.Tasks;
using SignaturePad.Forms;
using Syncfusion.XForms.Buttons;
using System.Diagnostics;

namespace mcms.Views.Work.WorkDetail.Testform
{
    public partial class DigitalSignatureForm : ContentPage
    {
        private double width;
        private double height;

        TestFormViewModel testmodel { get; set; }

        public DigitalSignatureForm(TestFormViewModel testformviewmodel)
        {
            testmodel = testformviewmodel;
            InitializeComponent();
            BindingContext = testformviewmodel;
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
        void ClearBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            Signature.Clear();
            testmodel.signvisible = true;
            testmodel.imgvisible = false;
        }

        public byte[] ImageSourceToBytes(Stream stream)
        {
            byte[] bytesAvailable = new byte[stream.Length];
            stream.Read(bytesAvailable, 0, bytesAvailable.Length);
            return bytesAvailable;
        }


        async void SaveBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            Stream stream = await Signature.GetImageStreamAsync(SignatureImageFormat.Png);
            if (stream != null)
            {
                testmodel.SelectedSignature.signature = Convert.ToBase64String(ImageSourceToBytes(stream));
            }
            await testmodel.SaveDigitalSignature();
        }

        private async void SfCheckBox_StateChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {

            SfCheckBox cek = sender as SfCheckBox;
            Debug.WriteLine("Checked valus is : " + cek.IsChecked);
           await testmodel.ChangedSignatureAsync((bool)cek.IsChecked);


            //used to clear the signature
            if((bool)cek.IsChecked)
            {
                Signature.Clear();
                testmodel.signvisible = false;
                testmodel.imgvisible = false;
                testmodel.clearsignature = false;
            }
            else
            {
                testmodel.signvisible = true;
                testmodel.imgvisible = true;
                testmodel.clearsignature = true;
            }

        }
    }
}
