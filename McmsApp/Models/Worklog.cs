using System.ComponentModel;
using PropertyChanged;
using SQLite;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Worklog : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int workorderid { get; set; }
        public string tnbremarks { get; set; }
        public int worklogid { get; set; }
        public string wonum { get; set; }
        public bool isupload { get; set; }
        public bool pendingupload { get; set; }
        public string username { get; set; }
        public string anywhererefid { get; set; }
        public DateTime modifydate { get; set; }
        public string tnbstatus { get; set; }
        public string assignreplocid { get; set; }
        public string createby { get; set; }
        public string logtype { get; set; }
        public DateTime createdate { get; set; }
        public string logtype_description { get; set; }
        public string modifyby { get; set; }
        public string localref { get; set; }
        public string tnbstatus_description { get; set; }
        public string orgid { get; set; }
        public string description { get; set; }
        public string recordkey { get; set; }
        public string tnbcompletedby { get; set; }
        public string description_longdescription { get; set; }
        public bool clientviewable { get; set; }
        public DateTime tnbcompleteddate { get; set; }
        public string href { get; set; }
        public string ProtectionName { get; internal set; }
        public int sequence { get; set; }
        public Syncfusion.Maui.Core.BadgeIcon badgeicon { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
