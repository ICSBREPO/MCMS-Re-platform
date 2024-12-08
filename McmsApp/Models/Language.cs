using System;
using System.ComponentModel;
using System.Windows.Input;
using mcms.Views.Tabbed;
using Syncfusion.XForms.Buttons;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mcms.Models
{
    public class Language : INotifyPropertyChanged
    {

        private bool isChecked;
        private ICommand stateChangedCommand;
        public string Item { get; set; }
        public string chooseLanguage;
        public ICommand StateChangedCommand
        {
            get { return stateChangedCommand; }
            set
            {
                stateChangedCommand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StateChangedCommand"));
            }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public Language()
        {
            StateChangedCommand = new Command(OutputValue);

        }

        async void OutputValue(object person)
        {
            if (IsChecked)
            {
                String message = (person as SfRadioButton).Text;
                await SecureStorage.SetAsync("chooseLanguageTemp", message);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}
