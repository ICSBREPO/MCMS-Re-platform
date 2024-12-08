using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PropertyChanged;
using Xamarin.Forms.Internals;

namespace mcms.Models
{
    [Preserve(AllMembers = true)]

    [AddINotifyPropertyChangedInterface]
    public class WODummy : INotifyPropertyChanged
    {
        public string wonum { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string startdate { get; set; }
        public string description { get; set;  }
        public string status_color { get; set; }
        public string status_description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ChartDisplay
    {
        public string status { get; set; }
        public int Count { get; set; }
    }

    public class Filter
    {
        public string type { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class NotifDummy : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReceivedTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
