using System.ComponentModel;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Syncfusion.Maui.Core;

namespace McmsApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Distinct : INotifyPropertyChanged
    {
        public string value { get; set; }
        public string favourite { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    [AddINotifyPropertyChangedInterface]
    public class SelectItems : INotifyPropertyChanged
    {
        public string selectstatus { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class WOChart
    {
        public double Count { get; set; }
        public string Name { get; set; }

    }

    public class DataLabel
    {
        public bool labelSQA { get; set; }
    }

    public class CountWO : INotifyPropertyChanged
    {
        public int woid { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public int? count { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    [AddINotifyPropertyChangedInterface]
    public class Workorder : INotifyPropertyChanged
    {
        //custom attr
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string textColor { get; set; }

        public string assetnum { get; set; }
        public string textImage { get; set; }
        public bool isFav { get; set; }
        public string username { get; set; }
        public double tnb_latitude2 { get; set; }
        public double tnb_longitude2 { get; set; }
        public bool pendingupdate { get; set; }
        public int sequence { get; set; }
        public BadgeIcon badgeicon { get; set; }
        public bool checkedpending { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<CountWO> countwo { get; set; }
        //original attr
        public bool plusgqualitycritical { get; set; }
        public string parent { get; set; }
        public bool aos { get; set; }
        public double estservcost { get; set; }
        public bool pluscismobile { get; set; }
        public bool tnbsaidi50req { get; set; }
        public bool plusgcopyrevactrsk { get; set; }
        public bool istask { get; set; }
        public string plusgaudit_collectionref { get; set; }
        public double estatapprmatcost { get; set; }
        public bool tnbpiatrequiredapp { get; set; }
        public string tnbsupplyengclerk { get; set; }
        public bool tnbancillaryreq { get; set; }
        public string worklog_collectionref { get; set; }
        public bool pluscloop { get; set; }
        public double actmatcost { get; set; }
        public bool tnbtestingrequired { get; set; }
        public bool nestedjpinprocess { get; set; }
        public bool plusdisprereq { get; set; }
        public double estatapprtoolcost { get; set; }
        public bool hasfollowupwork { get; set; }
        public bool flowactionassist { get; set; }
        public bool ignorediavail { get; set; }
        public string fincntrlid { get; set; }
        public bool pluspporeq { get; set; }
        public bool pluspallowquote { get; set; }
        public bool tnbflprojwarrantyexists { get; set; }
        public bool plusgtestcert { get; set; }
        public string tnbsupplyengineer { get; set; }
        public string tnbsapintegrationstatus { get; set; }
        public double estlabhrs { get; set; }
        public bool plusgimpreqd { get; set; }
        public bool tnbassetprojwarrantyexists { get; set; }
        public bool plusglongterm { get; set; }
        public double estoutlabcost { get; set; }
        public string tnbsubzone { get; set; }
        public string tnbconcatenatestatus { get; set; }
        public bool disabled { get; set; }
        public bool plusgaarrequired { get; set; }
        public double estdur { get; set; }
        public string fctaskid { get; set; }
        public string fcprojectid { get; set; }
        public bool plusgactcompleted { get; set; }
        public string tnbisgovernment_description { get; set; }
        public string tnbstate_description { get; set; }
        public double estatapprintlabhrs { get; set; }
        public DateTime? statusdate { get; set; }
        public string tnbworkscope_description { get; set; }
        public bool tnbpermitkhazrequired { get; set; }
        public double acttoolcost { get; set; }
        public double actlabcost { get; set; }
        public double estatapprlabhrs { get; set; }
        public string tnbsubvertical { get; set; }
        public bool plusglesslearned { get; set; }
        public double estatapprlabcost { get; set; }
        public string tnbforeman { get; set; }
        public bool plusgauditreq { get; set; }
        public bool plusgcomcrit { get; set; }
        public int tnbassetprojwarrantyperiod { get; set; }
        public bool plusghighrisk { get; set; }
        public bool tnbshowallsntask { get; set; }
        public string siteid { get; set; }
        public bool suspendflow { get; set; }
        public string status_description { get; set; }
        public double actintlabhrs { get; set; }
        public bool plusgpostactcomp { get; set; }
        public bool tnbisreplenishworkorder { get; set; }
        public bool chargestore { get; set; }
        public double outlabcost { get; set; }
        public string orgid { get; set; }
        public bool plusgactrequired { get; set; }
        public bool tnbsqacompliant { get; set; }
        public bool tnbcandard { get; set; }
        public bool plusgreviewed { get; set; }
        public bool plusgscheddev { get; set; }
        public bool tnbcantecopd { get; set; }
        public bool inctasksinsched { get; set; }
        public string tnbstation { get; set; }
        public string tnbrecommender { get; set; }
        public bool plusdsabotage { get; set; }
        public bool flowcontrolled { get; set; }
        public bool tnbtrustfund { get; set; }
        public string description { get; set; }
        public bool plusgenvcrit { get; set; }
        public double estatapproutlabcost { get; set; }
        public bool tnbisinfrastructure { get; set; }
        public string plusgpermitwork_collectionref { get; set; }
        public string djpapplied { get; set; }
        public string tnbstate { get; set; }
        public string tnbprojectphase_description { get; set; }
        public double estoutlabhrs { get; set; }
        public bool tnbmosrequired { get; set; }
        public bool plusgfailfix { get; set; }
        public double actintlabcost { get; set; }
        public string tnbprojectphase { get; set; }
        public string tnbprojectnumber { get; set; }
        public bool plusgreviewreq { get; set; }
        public bool statusiface { get; set; }
        public bool isturnkey { get; set; }
        public bool tnbbulkreq { get; set; }
        public string wogroup { get; set; }
        public bool tnbchildwocomplete { get; set; }
        public string location { get; set; }
        public bool plusglock { get; set; }
        public bool haschildren { get; set; }
        public bool apptrequired { get; set; }
        public bool historyflag { get; set; }
        public bool plusgassactivityimp { get; set; }
        public int tnbmaterialdocno { get; set; }
        public bool plusgccf { get; set; }
        public bool parentchgsstatus { get; set; }
        public bool tnbmeterchange { get; set; }
        public bool lms { get; set; }
        public bool tnbshowallactivities { get; set; }
        public string tnbsupplytechnician { get; set; }
        public string href { get; set; }

        //added start and finish date

        public DateTime? schedstart { get; set; }
        public DateTime? schedfinish { get; set; }
        public bool tnbge15req { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbpsiwochecklisttype> tnbpsiwochecklisttype { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Tnbwometergroup> tnbwometergroup { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Plusgaudit> plusgaudit { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Workorderspec> workorderspec { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PermitWork> plusgpermitwork { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Doclinks> doclinks { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Worklog> worklog { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Asset> asset { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Locations> locations { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TnbWODPMS> tnbwodpms { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<AssetSpec> assetspec { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LocationSpec> locationspec { get; set; }
        public bool plusgdefmeasreq { get; set; }
        public bool plusgdocsattached { get; set; }
        public bool plusgtemp { get; set; }
        public double actlabhrs { get; set; }
        public bool tnbchildwometer { get; set; }
        public bool plusgmpoverride { get; set; }
        public double actservcost { get; set; }
        public bool tnbsinglecustomer { get; set; }
        public bool reqasstdwntime { get; set; }
        public bool tnbshowallletter { get; set; }
        public double estmatcost { get; set; }
        public string status { get; set; }
        public bool template { get; set; }
        public string tnbwochecklisthdr_collectionref { get; set; }
        public string tnbplannergroup { get; set; }
        public string tnbworkscope { get; set; }
        public double esttoolcost { get; set; }
        public string reportedby { get; set; }
        public bool los { get; set; }
        public string tnbadpmo { get; set; }
        public string persongroup { get; set; }
        public bool tnbpiatrequiredad { get; set; }
        public double outmatcost { get; set; }
        public bool tnbptwcancel { get; set; }
        public string tnbsubvertical_description { get; set; }
        public bool plusgcrittoqual { get; set; }
        public string changeby { get; set; }
        public bool interruptible { get; set; }
        public double estlabcost { get; set; }
        public string lead { get; set; }
        public string wonum { get; set; }
        public bool downtime { get; set; }
        public string tnbzone { get; set; }
        public double tnbcountdown { get; set; }
        public int workorderid { get; set; }
        public bool tnbcanrelpd { get; set; }
        public bool tnbankguarantee { get; set; }
        public string tnbvertical { get; set; }
        public double actoutlabcost { get; set; }
        public string tnbvertical_description { get; set; }
        public double estatapprservcost { get; set; }
        public string tnbmask { get; set; }
        public bool pluspallowquotelabor { get; set; }
        public bool ignoresrmavail { get; set; }
        public bool plusgmigrationreq { get; set; }
        public double outtoolcost { get; set; }
        public double estatapproutlabhrs { get; set; }
        public double estatapprintlabcost { get; set; }
        public string firstapprstatus { get; set; }
        public bool plusgcontinplan { get; set; }
        public bool tnbshowallcpp { get; set; }
        public bool plusgassactivity { get; set; }
        public bool plusgcapex { get; set; }
        public bool woisswap { get; set; }
        public bool plusghassafetycrit { get; set; }
        public bool woacceptscharges { get; set; }
        public bool repairlocflag { get; set; }
        public DateTime? changedate { get; set; }
        public string woclass_description { get; set; }
        public bool plusgactexternal { get; set; }
        public bool pluspbillapproved { get; set; }
        public string woclass { get; set; }
        public double actoutlabhrs { get; set; }
        public string tnbbusinessarea { get; set; }
        public bool plusgprepactiv { get; set; }
        public bool plusgtgtdatemod { get; set; }
        public bool ams { get; set; }
        public DateTime? reportdate { get; set; }
        public bool tnbhandoverrequired { get; set; }
        public string newchildclass { get; set; }
        public bool pluspallowquotematerial { get; set; }
        public bool plusgpass { get; set; }
        public bool tnbhandoverrequiredapp { get; set; }
        public bool tnbslareq { get; set; }
        public int tnbflprojwarrantyperiod { get; set; }
        public string tnbisgovernment { get; set; }
        public bool pluspallowquoteservice { get; set; }
        public bool tnbshutdownrequired { get; set; }
        public string worktype { get; set; }
        public string _action { get; set; }
        public string tnbvoltagelevel { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ErrorResponse Error { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ResponseInfo
    {
        public string href { get; set; }
    }

    public class RespDownloadWorkorder
    {
        public List<Workorder> member { get; set; }
        public string href { get; set; }
        public ResponseInfo responseInfo { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class TnbWODPMS
    {
        public int? tnbwodpmsid { get; set; }
        public string tnbwonum { get; set; }
        public int tnbkvt { get; set; }
        public int tnbsavt { get; set; }
        public int tnbkvr { get; set; }
        public int tnbsavr { get; set; }
        public int tnbpe { get; set; }
        public int tnbjtg { get; set; }
        public int tnbhdd { get; set; }
        public int tnbsk5 { get; set; }
        public int tnbsvc { get; set; }
        public int tnbmp { get; set; }
        public bool tnbkvtna { get; set; }
        public bool tnbsavtna { get; set; }
        public bool tnbkvrna { get; set; }
        public bool tnbsavrna { get; set; }
        public bool tnbpena { get; set; }
        public bool tnbjtgna { get; set; }
        public bool tnbhddna { get; set; }
        public bool tnbsk5na { get; set; }
        public bool tnbsvcna { get; set; }
        public bool tnbmpna { get; set; }
    }

    public class ChildWORest
    {
        public List<Workorder> member { get; set; }
    }

    public class SubmenuInfo : INotifyPropertyChanged
    {
        private string menuTitle;
        private string menuImage;
        private string slugMenu;
        private int menuBadge;
        private string background;
        private string textColor;

        public string MenuTitle
        {
            get { return menuTitle; }
            set
            {
                menuTitle = value;
                OnPropertyChanged("MenuTitle");
            }
        }

        public string MenuImage
        {
            get { return menuImage; }
            set
            {
                menuImage = value;
                OnPropertyChanged("MenuImage");
            }
        }

        public string SlugMenu
        {
            get { return slugMenu; }
            set
            {
                slugMenu = value;
                OnPropertyChanged("SlugMenu");
            }
        }

        public int MenuBadge
        {
            get { return menuBadge; }
            set
            {
                menuBadge = value;
                OnPropertyChanged("MenuBadge");
            }
        }

        public string Background
        {
            get { return background; }
            set
            {
                background = value;
                OnPropertyChanged("Background");
            }
        }

        public string TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                OnPropertyChanged("TextColor");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

}
