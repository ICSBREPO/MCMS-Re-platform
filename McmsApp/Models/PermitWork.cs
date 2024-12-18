using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Syncfusion.Maui.Core;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class PermitWork : INotifyPropertyChanged
    {
        [ForeignKey(typeof(Workorder))]
        //custom attribute
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string username { get; set; }
        public bool pendingupload { get; set; }
        public string _action { get; set; }
        public int workorderid { get; set; }
        public BadgeIcon badgeicon { get; set; }
        public string tnbsafety_description { get; set; }
        public int? plusgpermitworkid { get; set; }
        public bool historyflag { get; set; }
        public bool workcompleted { get; set; }
        public string localref { get; set; }
        public bool crossareacountsig { get; set; }
        public bool isolationsremoved { get; set; }
        public DateTime? createdate { get; set; }
        public bool closureenterpmtrgt { get; set; }
        public string tnbppe { get; set; }
        public bool attacheddoc { get; set; }
        public bool selfisolation { get; set; }
        public string tnbhazard_description { get; set; }
        public string href { get; set; }
        public bool closuresitecleared { get; set; }
        public string tnbcreatedby { get; set; }
        public string status_description { get; set; }
        public bool riskcopiedfromwo { get; set; }
        public bool thirdparty { get; set; }
        public string orgid { get; set; }
        public string tnbcp { get; set; }
        public string tnbap { get; set; }
        public string tnbstate { get; set; }
        public string tnbsubzone { get; set; }
        public string tnbpermittype { get; set; }
        public string tnbbusinessarea { get; set; }
        public string tnbunit { get; set; }
        public string tnbissuedby { get; set; }
        public string tnbissuedto { get; set; }
        public string tnbzone { get; set; }
        public string tnbstatus { get; set; }
        public string tnbppe_description { get; set; }
        public DateTime? tnbissuedate { get; set; }
        public bool inhibitsremoved { get; set; }
        public string status { get; set; }
        public bool template { get; set; }
        public bool riskassessreq { get; set; }
        public bool safecondition { get; set; }
        public bool production { get; set; }
        public string description { get; set; }
        public string tnbsafety { get; set; }
        public string tnbhazard { get; set; }
        public bool closureauditrequired { get; set; }
        public string permitworknum { get; set; }
        public bool isocert { get; set; }
        public bool riskrevised { get; set; }
        public bool contgastestreqd { get; set; }
        public bool longtermiso { get; set; }
        public string ptwclass { get; set; }
        public string ptwclass_description { get; set; }
        public string requestor { get; set; }
        public string siteid { get; set; }
        public int extnum { get; set; }
        public DateTime? statusdate { get; set; }
        public bool sitechecked { get; set; }
        public bool masterpermit { get; set; }
        public bool tbtcompleted { get; set; }
        public string tnbstatus_description { get; set; }
        public string tnbwonum { get; set; }
        public bool closurelessonslearned { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Doclinks> doclinks { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
