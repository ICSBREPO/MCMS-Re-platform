using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace McmsApp.Models
{
    public class Format
    {
        public string label { get; set; }
        public string href { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class Doclinks : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? id { get; set; }
        public int refid { get; set; }
        public int? osid { get; set; }
        public string errormessage { get; set; }
        public bool isdelete { get; set; }
        public bool pendingupload { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        public string previewdoc { get; set; }
        public string iconextension { get; set; }
        //original
        public int? docinfoid { get; set; }
        public int? doclinksid { get; set; }
        public string reference { get; set; }
        public string doctype { get; set; }
        public string urltype { get; set; }
        public string title { get; set; }
        public DateTime? createdate { get; set; }
        public string description { get; set; }
        public string ownertable { get; set; }
        public string documentdata { get; set; }
        public string document { get; set; }
        public string urlname { get; set; }
        public string username { get; set; }
        public string createby { get; set; }
        public int? ownerid { get; set; }
        public int attachmentSize { get; set; }
        public DateTime modified { get; set; }
        public string fileName { get; set; }
        public string identifier { get; set; }
        public string _action { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MemberDoc
    {
        public string href { get; set; }
        public string localref { get; set; }
        public Doclinks describedBy { get; set; }
    }

    public class DoclinkRest
    {
        public string href { get; set; }
        public List<MemberDoc> member { get; set; }
    }
}
