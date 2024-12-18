using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Tnbwochecklistsignature : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? id { get; set; }
        public int? tnbwochecklistsignatureid { get; set; }
        public int tnbownerid { get; set; }
        public string tnblabel { get; set; }
        public bool isinternal { get; set; }
        public string tnbcomp { get; set; }
        public string tnbname { get; set; }
        public string tnbphone { get; set; }
        public string signature { get; set; }
        public string tnbcomments { get; set; }
        public bool tnbsigned { get; set; }
        public bool tnbIsTNBStuff { get; set; }
        public string tnbsignedby { get; set; }
        public DateTime? tnbsigneddate { get; set; }
        public string tnbsignedbycontractor { get; set; }
        public string tnbstaffid { get; set; }
        public string _imagelibref { get; set; }
       // public DateTime tnbsigdate { get; set; }
        public string updateby { get; set; }
        public DateTime tnbexecdate { get; set; }
        public bool tnbpass { get; set; }
        public bool tnbfail { get; set; }
        public bool tnbpartpass { get; set; }
        public string _action { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Imglib> imglib { get; set; }
        public bool pendingupdate { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Tnbsignature : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? id { get; set; }
        public int refid { get; set; }
        public int? tnbsignatureid { get; set; }
        public int? tnbownerid { get; set; }
        public string tnbownertable { get; set; }
        public string tnblabel { get; set; }
        public bool isinternal { get; set; }
        public string tnbcomp { get; set; }
        public string tnbname { get; set; }
        public string tnbphone { get; set; }
        public string signature { get; set; }
        public string tnbcomments { get; set; }
        public string tnbstaffid { get; set; }
        public string _imagelibref { get; set; }
        public DateTime tnbsigdate { get; set; }
        public string updateby { get; set; }
        public DateTime tnbexecdate { get; set; }
        public bool tnbpass { get; set; }
        public bool tnbfail { get; set; }
        public bool tnbpartpass { get; set; }
        public string _action { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Imglib> imglib { get; set; }
        public bool pendingupdate { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AddINotifyPropertyChangedInterface]
    public class Imglib : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string image { get; set; }
        public string imagename { get; set; }
        public int? imglibid { get; set; }
        public int? refobjectid { get; set; }
        public string mimetype { get; set; }
        public string refobject { get; set; }
        public string _action { get; set; }

        public bool pendingupdate { get; set; }
        public Syncfusion.XForms.BadgeView.BadgeIcon badgeicon { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
