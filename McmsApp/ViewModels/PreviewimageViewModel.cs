using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace mcms.ViewModels
{
    public class PreviewimageViewModel : BaseViewModel
    {
        public string Base64Image { set; get; }
        public ICommand BackPage { get; set; }
        INavigation Navigation;

        public PreviewimageViewModel(INavigation navigation, String base64Image)
        {
            Navigation = navigation;
            Base64Image = base64Image;
            BackPage = new Command(BackPopPage);
        }

        public void BackPopPage()
        {
            Navigation.PopModalAsync();
        }
    }
}
