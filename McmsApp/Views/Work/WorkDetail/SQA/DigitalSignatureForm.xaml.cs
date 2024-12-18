using System;
using System.Collections.Generic;
using mcms.AppLayout;
using System.IO;
using mcms.ViewModels;
using Xamarin.Forms;
using System.Threading.Tasks;
using SignaturePad.Forms;

namespace mcms.Views.Work.WorkDetail.SQA
{
    public partial class DigitalSignatureForm : ContentPage
    {
        private double width;
        private double height;

        SqaViewModel sqamodel { get; set; }

        public DigitalSignatureForm(SqaViewModel sqaviewmodel)
        {
            sqamodel = sqaviewmodel;
            InitializeComponent();
            BindingContext = sqaviewmodel;
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
            sqamodel.signvisible = true;
            sqamodel.imgvisible = false;
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
                sqamodel.SelectedSignature.signature = Convert.ToBase64String(ImageSourceToBytes(stream));
            }
            await sqamodel.SaveDigitalSignature();
        }
    }
}
